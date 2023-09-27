using System.Collections.Concurrent;
using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public class SpecService : ISpecService
{
    readonly ISpecRepository _repository;

    const string REPO_TASK_DATA = "DataTask";
    const string REPO_TASK_SPEC = "SpecTask";
    
    public SpecService( ISpecRepository repository )
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<SpecMetaData>> GetSpecMetaData()
    {
        SpecMetaData? data = await TryGetMetaData();
        return new ServiceResponse<SpecMetaData>( null, false, "" );
    }
    public Task<ServiceResponse<Dictionary<int, List<object>>>> GetSpecLookups() { throw new NotImplementedException(); }

    async Task<SpecMetaData?> TryGetMetaData()
    {
        ConcurrentDictionary<int, SpecDataDescr>? dataTypes = null; //_repository.TryGetDataTypesById();
        ConcurrentDictionary<int, List<int>>? specCategories = null;//_repository.TryGetSpecIdsByCategoryId();
        ConcurrentDictionary<string, int>? specNames = null; //_repository.TryGetSpecIdsByName();
        ConcurrentDictionary<int, Spec>? specs = null;//_repository.TryGetSpecsById();

        var repoTasks = new Dictionary<string, Task>();

        //if ( dataTypes == null )
            //repoTasks.Add( REPO_TASK_DATA, _repository.GetSpecDataDescrs() );
        if ( specCategories == null || specNames == null || specs == null )
            repoTasks.Add( REPO_TASK_SPEC, _repository.GetSpecs() );

        if ( repoTasks.Count > 0 )
        {
            await Task.WhenAll( repoTasks.Values );   
        }



        /*List<Category>? categories = await _cache.GetCategories();

        if ( categories != null )
            return categories;

        categories = await _repository.GetCategories();

        if ( categories == null )
            return null;

        await _cache.CacheCategories( categories );
        return categories;*/
        return null;
    }
}