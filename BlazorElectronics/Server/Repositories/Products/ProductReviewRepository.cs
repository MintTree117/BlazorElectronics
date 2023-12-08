using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using Dapper;

namespace BlazorElectronics.Server.Repositories.Products;

public sealed class ProductReviewRepository : DapperRepository, IProductReviewRepository
{
    const string PROCEDURE_GET_FOR_PRODUCT = "Get_ProductReviews";
    const string PROCEDURE_GET_FOR_USER = "Get_UserProductReviews";
    const string PROCEDURE_GET_EDIT = "Get_ProductReviewEdit";
    const string PROCEDURE_INSERT = "Insert_ProductReview";
    const string PROCEDURE_UPDATE = "Update_ProductReview";
    const string PROCEDURE_DELETE = "Delete_ProductReview";

    public ProductReviewRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<IEnumerable<ProductReview>?> GetForProduct( int productId, int rows, int offset )
    {
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<ProductReview>?> GetForUser( int userId, int rows, int offset )
    {
        throw new NotImplementedException();
    }
    public async Task<ProductReview?> GetEdit( int reviewId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_REVIEW_ID, reviewId );
        return await TryQueryAsync( QuerySingleOrDefault<ProductReview?>, p, PROCEDURE_GET_EDIT );
    }
    public async Task<int> Insert( ProductReview review )
    {
        DynamicParameters p = GetInsertParams( review );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( ProductReview review )
    {
        DynamicParameters p = GetUpdateParams( review );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE );
    }
    public async Task<bool> Delete( int reviewId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_REVIEW_ID, reviewId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }

    static DynamicParameters GetInsertParams( ProductReview model )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, model.ProductId );
        p.Add( PARAM_USER_ID, model.UserId );
        p.Add( PARAM_PRODUCT_RATING, model.Rating );
        p.Add( PARAM_PRODUCT_REVIEW, model.Review );
        return p;
    }
    static DynamicParameters GetUpdateParams( ProductReview model )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_REVIEW_ID, model.ReviewId );
        p.Add( PARAM_PRODUCT_RATING, model.Rating );
        p.Add( PARAM_PRODUCT_REVIEW, model.Review );
        return p;
    }
}