using BlazorElectronics.Server.DbContext;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecRepository : DapperRepository, ISpecRepository
{
    const string STORED_PROCEDURE_GET_ALL_DESCRS = "Get_SpecDescrs";
    const string STORED_PROCEDURE_GET_DESCRS_BY_CATEGORY = "Get_SpecDescrsByCategory";
    const string STORED_PROCEDURE_GET_ALL_LOOKUPS = "Get_SpecLookups";
    const string STORED_PROCEDURE_GET_LOOKUPS_BY_CATEGORY = "Get_SpecLookupsByCategory";

    const string LOOKUP_ID_COLUMN = "LookupId";
    const string COLUMN_NAME_SPEC_ID = "SpecId";
    const string COLUMN_NAME_SPEC_CATEGORY_ID = "SpecCategoryId";

    public SpecRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
}