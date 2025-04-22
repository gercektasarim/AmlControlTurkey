using AmlControlTurkey.Core.Extensions;
using AmlControlTurkey.Core.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

public class LuceneSearchService
{
    private readonly Lucene.Net.Store.Directory _indexDirectory;
    private readonly Analyzer _analyzer;
    private readonly IndexWriter _indexWriter;
    public int version = 1;

    public LuceneSearchService(string indexPath)
    {
        if (!System.IO.Directory.Exists(indexPath))
        {
            System.IO.Directory.CreateDirectory(indexPath);
        }

        _indexDirectory = FSDirectory.Open(new DirectoryInfo(indexPath), new SimpleFSLockFactory());
        _analyzer = new StandardAnalyzer((Lucene.Net.Util.Version.LUCENE_CURRENT)
            //, new TurkishTextReader(new StringReader(string.Empty))
            );
        var indexConfig = (Lucene.Net.Util.Version.LUCENE_CURRENT, _analyzer);
        _indexWriter = new IndexWriter(_indexDirectory, _analyzer, true, new IndexWriter.MaxFieldLength(10000000));
    }

    public void IndexDocuments(List<AmlDataModel> persons, AmlDataEnum dataSource, int version)
    {
        foreach (var person in persons)
        {
            var doc = new Document();
            
            doc.Add(new Field("UniqId", person.UniqId, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("IdentityNumber", person.IdentityNumber ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("NameTitle", person.NameTitle ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Nationality", person.Nationality ?? "", Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("Organization", person.Organization ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("MotherName", person.MotherName ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("BirthDate", person.BirthDate != null ? person.BirthDate.ConvertDates() : "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("BirthPlace", person.BirthPlace ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("FatherName", person.FatherName ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("OtherNameTitle", person.OtherNameTitle ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Addresses", person.Addresses ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Emails", person.Emails ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Phones", person.Phones ?? "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("PhotoUrl", person.PhotoUrl ?? "", Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("Source", dataSource.ToString(), Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("LastUpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), Field.Store.YES, Field.Index.NO));

            doc.Add(new NumericField("Version", Field.Store.YES, true).SetIntValue(version));
            doc.Add(new NumericField("DataSource", Field.Store.YES, true).SetIntValue((int)dataSource));


            _indexWriter.UpdateDocument(new Term("UniqId", person.UniqId), doc);
        }

        _indexWriter.Commit();
        DeleteDocumentsOlderVersionAndDataSource(version, (int)dataSource);
    }

    public List<AmlDataModel> Search(string queryText)
    {

        var searcher = new IndexSearcher(DirectoryReader.Open(_indexDirectory, false));
        var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_CURRENT, new[] { "NameTitle", "OtherNameTitle", "IdentityNumber",  "BirthDate"}, _analyzer);
        var query = parser.Parse($"{queryText}*");

        // Log the parsed query
        Console.WriteLine($"Parsed Query: {query}");

        var hits = searcher.Search(query, 20).ScoreDocs;

        var results = new List<AmlDataModel>();
        foreach (var hit in hits)
        {
            var doc = searcher.Doc(hit.Doc);

            // Log the document fields
            Console.WriteLine($"Document Fields: {doc}");

            var person = new AmlDataModel
            {
                IdentityNumber = doc.Get("IdentityNumber"),
                NameTitle = doc.Get("NameTitle"),
                Nationality = doc.Get("Nationality"),
                Organization = doc.Get("Organization"),
                MotherName = doc.Get("MotherName"),
                BirthDate = (doc.Get("BirthDate")),
                BirthPlace = doc.Get("BirthPlace"),
                FatherName = doc.Get("FatherName"),
                OtherNameTitle = doc.Get("OtherNameTitle"),
                Addresses = doc.Get("Addresses"),
                Emails = doc.Get("Emails"),
                Phones = doc.Get("Phones"),
                Source = doc.Get("Source"),
                PhotoUrl = doc.Get("PhotoUrl"),
                LastUpdateTime = doc.Get("LastUpdateTime"),
                Score = hit.Score,
                UniqId = doc.Get("UniqId"),
            };

            results.Add(person);
        }

        return results;
    }

    public void DeleteDocumentsOlderVersionAndDataSource(int currentVersion, int dataSource)
    {
        var searcher = new IndexSearcher(DirectoryReader.Open(_indexDirectory, false));
        var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_CURRENT, "Version", _analyzer);
        var query = parser.Parse($"Version:[* TO {currentVersion - 1}] AND DataSource:{dataSource}");
        var hits = searcher.Search(query, 1000).ScoreDocs;

        foreach (var hit in hits)
        {
            var doc = searcher.Doc(hit.Doc);
            _indexWriter.DeleteDocuments(new Term("UniqId", doc.Get("UniqId")));
        }

        _indexWriter.Commit();
    }

}