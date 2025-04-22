using AmlControlTurkey.Core.Models;
using AmlControlTurkey.Core.DataCollectors;

public class DataUpdater
{
    private readonly LuceneSearchService _luceneSearchService;
    private readonly BsaSearchService _bsaSearchService;

    public DataUpdater(LuceneSearchService luceneSearchService, BsaSearchService bsaSearchService)
    {
        _luceneSearchService = luceneSearchService;
        _bsaSearchService = bsaSearchService;
    }

    public async Task UpdateIndex(AmlDataEnum aml)
    {
        IDataCollector dataCollector = DataCollectorFactory.GetDataCollector(aml);
        DataCollectorContext context = new DataCollectorContext(dataCollector);
        var amlDataList = await context.Execute(aml);
        _bsaSearchService.IndexDocuments(amlDataList, aml, (int)DateTime.UtcNow.Ticks);
        _luceneSearchService.IndexDocuments(amlDataList, aml, (int)DateTime.UtcNow.Ticks);
    }
}
