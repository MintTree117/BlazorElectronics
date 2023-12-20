using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Promos;
using Dapper;

namespace BlazorElectronics.Server.Data.Repositories;

public class PromoRepository : DapperRepository, IPromoRepository
{
    const string PROCEDURE_GET_VIEW = "Get_PromoView";
    const string PROCEDURE_GET_EDIT = "Get_PromoEdit";
    const string PROCEDURE_INSERT = "Insert_Promo";
    const string PROCEDURE_UPDATE = "Update_Promo";
    const string PROCEDURE_DELETE = "Delete_Promo";
    
    public PromoRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<IEnumerable<PromoModel>?> GetView()
    {
        return await TryQueryAsync( Query<PromoModel>, null, PROCEDURE_GET_VIEW );
    }
    public async Task<PromoModel?> GetEdit( int promoId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PROMO_ID, promoId );
        return await TryQueryAsync( QuerySingleOrDefault<PromoModel?>, p, PROCEDURE_GET_EDIT );
    }
    public async Task<int> Insert( PromoModel promo )
    {
        DynamicParameters p = GetInsertParams( promo );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( PromoModel promo )
    {
        DynamicParameters p = GetUpdateParams( promo );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE );
    }
    public async Task<bool> Delete( int promoId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PROMO_ID, promoId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }

    static DynamicParameters GetInsertParams( PromoModel promo )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PROMO_CODE, promo.PromoCode );
        p.Add( PARAM_PROMO_DISCOUNT, promo.PromoDiscount );

        return p;
    }
    static DynamicParameters GetUpdateParams( PromoModel promo )
    {
        DynamicParameters p = GetInsertParams( promo );
        p.Add( PARAM_PROMO_ID, promo.PromoId );

        return p;
    }
}