using AmlControlTurkey.Core.Extensions;
using AmlControlTurkey.Core.Models;
using AmlControlTurkey.Core.Util;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlControlTurkey.Core.DataCollectors
{
    public class TerrorismDataCollector() : IDataCollector
    {
        public async Task<List<AmlDataModel>> ProcessData(AmlDataEnum aml)
        {
            var list = await HttpRequestUtil.Post<TerrorismModel>("https://www.terorarananlar.pol.tr/ISAYWebPart/TArananlar/GetTerorleArananlarList", new Dictionary<string, string>(), null);
            return ConvertToAmlDataModels(list, aml);
        }

        private List<AmlDataModel> ConvertToAmlDataModels(TerrorismModel model, AmlDataEnum aml)
        {
            var list = model.Kirmizi.Select(item => (item, AmlTerrorismTypes.Kirmizi))
                .Concat(model.Turuncu.Select(item => (item, AmlTerrorismTypes.Turuncu)))
                .Concat(model.Sari.Select(item => (item, AmlTerrorismTypes.Sari)))
                .Concat(model.Gri.Select(item => (item, AmlTerrorismTypes.Gri)));
            var amlDataModels = list.AsParallel().Select(data =>
            {
                var amlData = new AmlDataModel
                {
                    NameTitle = data.item.Name + " " + data.item.SureName,
                    IdentityNumber = "",
                    BirthDate = data.item.DateOfBirth,
                    BirthPlace = data.item.Birthplace,
                    Organization = data.item.Description,
                    PhotoUrl = $"https://www.terorarananlar.pol.tr/{data.item.PhotoUrl}"
                };
                amlData.UniqId = (int)aml + amlData.NameTitle + amlData.BirthDate + amlData.BirthPlace;
                return amlData;
            }).ToList();

            return amlDataModels;
        }
        private enum AmlTerrorismTypes
        {
            Kirmizi = 1,
            Turuncu = 2,
            Sari = 3,
            Gri = 4
        }
        private class TerrorismModel
        {
            public List<TerrorismListModel> Kirmizi { get; set; }
            public List<TerrorismListModel> Turuncu { get; set; }
            public List<TerrorismListModel> Sari { get; set; }
            public List<TerrorismListModel> Gri { get; set; }
        }
        private class TerrorismListModel
        {
            [JsonIgnore]
            public int TypeId { get; set; }
            [JsonIgnore]
            public string? SourceId { get; set; }
            [JsonProperty("Adi")]
            public string? Name { get; set; }
            [JsonProperty("Soyadi")]
            public string? SureName { get; set; }
            [JsonProperty("DogumTarihi")]
            public string? DateOfBirth { get; set; }
            [JsonProperty("DogumYeri")]
            public string? Birthplace { get; set; }
            [JsonProperty("TOrgutAdi")]
            public string? Description { get; set; }
            [JsonProperty("IlkGorselURL")]
            public string? PhotoUrl { get; set; }
        }
    }
}
