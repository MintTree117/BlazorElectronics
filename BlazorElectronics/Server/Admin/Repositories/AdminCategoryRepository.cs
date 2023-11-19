using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.Categories;
using Dapper;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminCategoryRepository : _AdminRepository, IAdminCategoryRepository
{
    static readonly string[] PROCEDURES_GET_CATEGORY = { "Get_CategoryPrimaryEdit", "Get_CategorySecondaryEdit", "Get_CategoryTertiaryEdit" };
    static readonly string[] PROCEDURES_ADD_CATEGORY = { "Add_CategoryPrimary", "Add_CategorySecondary", "Add_CategoryTertiary" };
    static readonly string[] PROCEDURES_UPDATE_CATEGORY = { "Update_CategoryPrimary", "Update_CategorySecondary", "Update_CategoryTertiary" };
    static readonly string[] PROCEDURES_REMOVE_CATEGORY = { "Remove_CategoryPrimary", "Remove_CategorySecondary", "Remove_CategoryTertiary" };
    
    public AdminCategoryRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<AddUpdateCategoryDto?> GetEditCategory( GetCategoryEditRequest dto )
    {
        string procedure = GetProcedure( PROCEDURES_GET_CATEGORY, dto.CategoryTier );
        DynamicParameters parameters = GetEditParameters( dto );

        return await TryAdminQuerySingle<AddUpdateCategoryDto>( procedure, parameters );
    }
    public async Task<AddUpdateCategoryDto?> AddCategory( AddUpdateCategoryDto dto )
    {
        string procedure = GetProcedure( PROCEDURES_ADD_CATEGORY, dto.Tier );
        DynamicParameters parameters = GetAddParameters( dto );

        return await TryAdminQueryTransaction<AddUpdateCategoryDto>( procedure, parameters );
    }
    public async Task<bool> UpdateCategory( AddUpdateCategoryDto dto )
    {
        string procedure = GetProcedure( PROCEDURES_UPDATE_CATEGORY, dto.Tier );
        DynamicParameters parameters = GetUpdateParameters( dto );

        return await TryAdminTransaction( procedure, parameters );
    }
    public async Task<bool> DeleteCategory( DeleteCategoryDto dto )
    {
        string procedure = GetProcedure( PROCEDURES_REMOVE_CATEGORY, dto.CategoryTier );
        DynamicParameters parameters = GetDeleteParameters( dto );
        
        return await TryAdminTransaction( procedure, parameters );
    }

    static string GetProcedure( string[] procedures, int tier )
    {
        return procedures[ tier - 1 ];
    }
    static DynamicParameters GetEditParameters( GetCategoryEditRequest dto )
    {
        var parameters = new DynamicParameters();

        string categoryIdName = dto.CategoryTier switch
        {
            1 => PARAM_CATEGORY_PRIMARY_ID,
            2 => PARAM_CATEGORY_SECONDARY_ID,
            3 => PARAM_CATEGORY_TERTIARY_ID,
            _ => throw new ServiceException( "Invalid category tier!", null )
        };

        parameters.Add( categoryIdName, dto.CategoryId );
        return parameters;
    }
    static DynamicParameters GetAddParameters( AddUpdateCategoryDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_CATEGORY_NAME, dto.Name );
        parameters.Add( PARAM_CATEGORY_API_URL, dto.ApiUrl );
        parameters.Add( PARAM_CATEGORY_IMAGE_URL, dto.ImageUrl );
        parameters.Add( PARAM_CATEGORY_DESCRIPTION, dto.Description );

        if ( dto.Tier > 1 )
            parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );

        if ( dto.Tier > 2 )
            parameters.Add( PARAM_CATEGORY_SECONDARY_ID, dto.SecondaryCategoryId );

        return parameters;
    }
    static DynamicParameters GetUpdateParameters( AddUpdateCategoryDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );
        parameters.Add( PARAM_CATEGORY_NAME, dto.Name );
        parameters.Add( PARAM_CATEGORY_API_URL, dto.ApiUrl );
        parameters.Add( PARAM_CATEGORY_IMAGE_URL, dto.ImageUrl );
        parameters.Add( PARAM_CATEGORY_DESCRIPTION, dto.Description );

        if ( dto.Tier > 1 )
            parameters.Add( PARAM_CATEGORY_SECONDARY_ID, dto.SecondaryCategoryId );

        if ( dto.Tier > 2 )
            parameters.Add( PARAM_CATEGORY_TERTIARY_ID, dto.TertiaryCategoryId );

        return parameters;
    }
    static DynamicParameters GetDeleteParameters( DeleteCategoryDto dto )
    {
        var parameters = new DynamicParameters();
        
        string paramName = dto.CategoryTier switch
        {
            1 => PARAM_CATEGORY_PRIMARY_ID,
            2 => PARAM_CATEGORY_SECONDARY_ID,
            3 => PARAM_CATEGORY_TERTIARY_ID,
            _ => throw new ServiceException( "Invalid category tier!", null )
        };
        
        parameters.Add( paramName, dto.CategoryId );
        return parameters;
    }
}