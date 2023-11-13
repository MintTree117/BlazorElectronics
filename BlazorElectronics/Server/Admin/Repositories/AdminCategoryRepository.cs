using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.Categories;
using Dapper;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminCategoryRepository : _AdminRepository, IAdminCategoryRepository
{
    static readonly string[] PROCEDURES_ADD_CATEGORY = { "Add_CategoryPrimary", "Add_CategorySecondary", "Add_CategoryTertiary" };
    static readonly string[] PROCEDURES_UPDATE_CATEGORY = { "Update_CategoryPrimary", "Update_CategorySecondary", "Update_CategoryTertiary" };
    static readonly string[] PROCEDURES_REMOVE_CATEGORY = { "Remove_CategoryPrimary", "Remove_CategorySecondary", "Remove_CategoryTertiary" };
    
    public AdminCategoryRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<bool> AddCategory( AddUpdateCategoryDto dto )
    {
        string procedure = PROCEDURES_ADD_CATEGORY[ dto.CategoryTier ];
        DynamicParameters parameters = GetAddParameters( dto );

        return await ExecuteAdminTransaction( procedure, parameters );
    }
    public async Task<bool> UpdateCategory( AddUpdateCategoryDto dto )
    {
        string procedure = PROCEDURES_UPDATE_CATEGORY[ dto.CategoryTier ];
        DynamicParameters parameters = GetUpdateParameters( dto );

        return await ExecuteAdminTransaction( procedure, parameters );
    }
    public async Task<bool> DeleteCategory( DeleteCategoryDto dto )
    {
        string procedure = PROCEDURES_REMOVE_CATEGORY[ dto.CategoryTier ];
        DynamicParameters parameters = GetDeleteParameters( dto );

        return await ExecuteAdminTransaction( procedure, parameters );
    }

    static DynamicParameters GetAddParameters( AddUpdateCategoryDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_CATEGORY_NAME, dto.CategoryName );
        parameters.Add( PARAM_CATEGORY_API_URL, dto.CategoryApiUrl );
        parameters.Add( PARAM_CATEGORY_IMAGE_URL, dto.CategoryImageUrl );
        parameters.Add( PARAM_CATEGORY_DESCRIPTION, dto.CategoryDescription );

        if ( dto.CategoryTier > 1 )
            parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );

        if ( dto.CategoryTier > 2 )
            parameters.Add( PARAM_CATEGORY_SECONDARY_ID, dto.SecondaryCategoryId );

        return parameters;
    }
    static DynamicParameters GetUpdateParameters( AddUpdateCategoryDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );
        parameters.Add( PARAM_CATEGORY_NAME, dto.CategoryName );
        parameters.Add( PARAM_CATEGORY_API_URL, dto.CategoryApiUrl );
        parameters.Add( PARAM_CATEGORY_IMAGE_URL, dto.CategoryImageUrl );
        parameters.Add( PARAM_CATEGORY_DESCRIPTION, dto.CategoryDescription );

        if ( dto.CategoryTier > 1 )
            parameters.Add( PARAM_CATEGORY_SECONDARY_ID, dto.SecondaryCategoryId );

        if ( dto.CategoryTier > 2 )
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
        
        parameters.Add( paramName );
        return parameters;
    }
}