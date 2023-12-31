using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Server.Core.Models.Reviews;
using BlazorElectronics.Shared.Reviews;
using Dapper;

namespace BlazorElectronics.Server.Data.Repositories;

public sealed class ReviewRepository : DapperRepository, IReviewRepository
{
    const string PROCEDURE_GET_FOR_PRODUCT = "Get_ProductReviews";
    const string PROCEDURE_GET_FOR_USER = "Get_UserProductReviews";
    const string PROCEDURE_GET_EDIT = "Get_ReviewEdit";
    const string PROCEDURE_INSERT = "Insert_Review";
    const string PROCEDURE_UPDATE = "Update_Review";
    const string PROCEDURE_DELETE = "Delete_Review";

    public ReviewRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<IEnumerable<ReviewModel>?> GetForProduct( ProductReviewsGetDto dto )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, dto.ProductId );
        p.Add( PARAM_ROWS, dto.Rows );
        p.Add( PARAM_OFFSET, dto.Rows * ( dto.Page - 1 ) );
        p.Add( PARAM_REVIEW_SORT_TYPE, dto.SortType );

        return await TryQueryAsync( Query<ReviewModel>, p, PROCEDURE_GET_FOR_PRODUCT );
    }
    public async Task<IEnumerable<ReviewModel>?> GetForUser( int userId, int rows, int page )
    {
        throw new NotImplementedException();
    }
    public async Task<ReviewModel?> GetEdit( int reviewId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_REVIEW_ID, reviewId );
        return await TryQueryAsync( QuerySingleOrDefault<ReviewModel?>, p, PROCEDURE_GET_EDIT );
    }
    public async Task<int> Insert( ReviewModel reviewModel )
    {
        DynamicParameters p = GetInsertParams( reviewModel );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( ReviewModel reviewModel )
    {
        DynamicParameters p = GetUpdateParams( reviewModel );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE );
    }
    public async Task<bool> Delete( int reviewId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_REVIEW_ID, reviewId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }

    static DynamicParameters GetInsertParams( ReviewModel model )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, model.ProductId );
        p.Add( PARAM_USER_ID, model.UserId );
        p.Add( PARAM_PRODUCT_RATING, model.Rating );
        p.Add( PARAM_PRODUCT_REVIEW, model.Review );
        return p;
    }
    static DynamicParameters GetUpdateParams( ReviewModel model )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_REVIEW_ID, model.ReviewId );
        p.Add( PARAM_PRODUCT_RATING, model.Rating );
        p.Add( PARAM_PRODUCT_REVIEW, model.Review );
        return p;
    }
}