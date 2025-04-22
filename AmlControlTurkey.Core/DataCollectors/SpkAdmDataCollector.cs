using AmlControlTurkey.Core.Extensions;
using AmlControlTurkey.Core.Models;
using AmlControlTurkey.Core.Util;
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
    public class SpkAdmDataCollector() : IDataCollector
    {
        public async Task<List<AmlDataModel>> ProcessData(AmlDataEnum aml)
        {
            var list = await HttpRequestUtil.Get<List<SpkModel>>("https://ws.spk.gov.tr/IdariYaptirimlar/api/TumIdariParaCezalari");
            return ConvertToAmlDataModels(list, aml);
        }

        private List<AmlDataModel> ConvertToAmlDataModels(List<SpkModel> list, AmlDataEnum aml)
        {
            var amlDataModels = list.Select(data =>
            {
                var amlData = new AmlDataModel();
                data.CopyValues<SpkModel, AmlDataModel>(amlData);
                amlData.UniqId = (int)aml + data.NameTitle + data.IdentityNumber + data.OtherNameTitle;
                return amlData;
            }).ToList();
            return amlDataModels;
        }

        public class SpkModel
        {
            [JsonProperty("mkkSicilNo")]
            public string IdentityNumber { get; set; }
            [JsonProperty("unvan")]
            public string NameTitle { get; set; }
            [JsonProperty("aciklama")]
            public string OtherNameTitle { get; set; }
        }
    }
}
