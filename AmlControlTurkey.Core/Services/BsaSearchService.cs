using Bsa.Search.Core.Common.Entities.Searchers;
using Bsa.Search.Core.Documents;
using Bsa.Search.Core.Indexes.Requests;
using Bsa.Search.Core.Indexes;
using Bsa.Search.Core;
using AmlControlTurkey.Core.Models;
using Bsa.Search.Core.Queries.Helpers;
using System;
using System.Collections.Generic;
using Bsa.Search.Core.Repositories.Entities;
using Bsa.Search.Core.Helpers;
using DocumentFormat.OpenXml.Office2010.Word;
using Bsa.Search.Core.Common.Extensions;

public class BsaSearchService
{
    private readonly MemoryDocumentIndex _documentIndex;
    private readonly SearchServiceEngine _searchService;

    public BsaSearchService()
    {
        _documentIndex = new MemoryDocumentIndex();
        _searchService = new SearchServiceEngine(_documentIndex);
    }




    public void IndexDocuments(List<AmlDataModel> persons, AmlDataEnum openSections, int ticks)
    {
        foreach (var person in persons)
        {
            var existingDocument = _searchService.Search(new SearchQueryRequest
            {
                Query = person.UniqId.Parse("UniqId"),
                Field = "ExternalId",
                Size = 1
            }).Documents.FirstOrDefault();

            if (existingDocument == null)
            {
                var document = new IndexDocument(person.UniqId);
                document.Add(new IndexField("UniqId", person.UniqId ?? "") { IsIndex = true });
                document.Add(new IndexField("NameTitle", person.NameTitle ?? "") { IsIndex = true });
                document.Add(new IndexField("Organization", person.Organization ?? "") { IsIndex = true });
                document.Add(new IndexField("MotherName", person.MotherName ?? "") { IsIndex = false });
                document.Add(new IndexField("BirthDate", person.BirthDate ?? "") { IsIndex = true });
                document.Add(new IndexField("BirthPlace", person.BirthPlace ?? "") { IsIndex = false });
                document.Add(new IndexField("OtherNameTitle", person.OtherNameTitle ?? "") { IsIndex = true });
                document.Add(new IndexField("FatherName", person.FatherName ?? "") { IsIndex = false });
                document.Add(new IndexField("IdentityNumber", person.IdentityNumber ?? "") { IsIndex = true });
                document.Add(new IndexField("Nationality", person.Nationality ?? "") { IsIndex = false });
                document.Add(new IndexField("Addresses", person.Addresses ?? "") { IsIndex = false });
                document.Add(new IndexField("Emails", person.Emails ?? "") { IsIndex = false });
                document.Add(new IndexField("Phones", person.Phones ?? "") { IsIndex = false });

                _searchService.Index(new IndexDocument[] { document });
            }
           
        }
    }



    public List<AmlDataModel> Search(string queryText)
    {
        var field = "*";
        var parsedQuery = queryText.Parse(field);
        var request = new SearchQueryRequest
        {
            Query = parsedQuery,
            Field = field,
            ShowHighlight = true,
            OrderField = SortOrderFields.Relevance,
            Order = SortOrder.Desc,
            Size = 20,
        };

        var results = _searchService.Search(request);
        var persons = new List<AmlDataModel>();

        foreach (var result in results.Documents)
        {
            var person = new AmlDataModel
            {
                NameTitle = result.Fields.FirstOrDefault(x => x.Name == "NameTitle").Value,
                Organization = result.Fields.FirstOrDefault(x => x.Name == "Organization").Value,
                MotherName = result.Fields.FirstOrDefault(x => x.Name == "MotherName").Value,
                BirthDate = result.Fields.FirstOrDefault(x => x.Name == "BirthDate").Value,
                BirthPlace = result.Fields.FirstOrDefault(x => x.Name == "BirthPlace").Value,
                OtherNameTitle = result.Fields.FirstOrDefault(x => x.Name == "OtherNameTitle").Value,
                IdentityNumber = result.Fields.FirstOrDefault(x => x.Name == "IdentityNumber").Value,
                FatherName = result.Fields.FirstOrDefault(x => x.Name == "FatherName").Value,
                Emails = result.Fields.FirstOrDefault(x => x.Name == "Emails").Value,
                Addresses = result.Fields.FirstOrDefault(x => x.Name == "Addresses").Value,
                Phones = result.Fields.FirstOrDefault(x => x.Name == "Phones").Value,
                Nationality = result.Fields.FirstOrDefault(x => x.Name == "Nationality").Value,
                UniqId = result.ExternalId,
            };

            persons.Add(person);
        }

        return persons;
    }
    public void DeleteOldDocuments(DateTime cutoffDate)
    {
        var documentRequest = new DocumentRequest
        {
            TimeStampTo = cutoffDate,
            TimeStampFrom = cutoffDate.AddYears(-1),
        };
        var documentsToDelete = _documentIndex.Get(documentRequest);
        _documentIndex.Delete(documentsToDelete.Select(x => x.Id.ToString()).ToList());
    }

}
