using AmlControlTurkey.Core.Extensions;
using AmlControlTurkey.Core.Models;
using AmlControlTurkey.Core.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlControlTurkey.Core.DataCollectors
{
    public class OpenSectionDataCollector() : IDataCollector
    {
        public async Task<List<AmlDataModel>> ProcessData(AmlDataEnum aml)
        {

            var list = HttpRequestUtil.ReadCsvFromUrl<OpensectionCsvModel>(string.Format("https://data.opensanctions.org/datasets/{0}/default/targets.simple.csv", DateTime.UtcNow.ToString("yyyyMMdd")), ',', Encoding.UTF8);
            return ConvertToAmlDataModels(list.Result);
        }
        private List<AmlDataModel> ConvertToAmlDataModels(List<OpensectionCsvModel> list)
        {
            var amlDataModels = list.Select(data =>
            {
                var aml = new AmlDataModel();
                data.CopyValues<OpensectionCsvModel, AmlDataModel>(aml);
                return aml;
            }).ToList();

            return amlDataModels;
        }

        private class OpensectionCsvModel
        {
            [Column("id")]
            public string UniqId { get; set; }
            [Column("name")]
            public string NameTitle { get; set; }
            [Column("aliases")]
            public string OtherNameTitle { get; set; }
            [Column("birth_date")]
            public string BirthDate { get; set; }
            [Column("countries")]
            public string Nationality { get; set; }
            [Column("sanctions")]
            public string Organization { get; set; }
            [Column("identifiers")]
            public string IdentityNumber { get; set; }
            [Column("emails")]
            public string Emails { get; set; }
            [Column("addresses")]
            public string Addresses { get; set; }
            [Column("phones")]
            public string Phones { get; set; }
        }
    }
}
