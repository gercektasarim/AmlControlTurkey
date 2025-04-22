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
    public class MasakBDataCollector() : IDataCollector
    {
        public async Task<List<AmlDataModel>> ProcessData(AmlDataEnum aml)
        {
            var list = HttpRequestUtil.ReadCsvFromUrl<MasakCsvModel>("https://ms.hmb.gov.tr/uploads/sites/12/2024/09/B-YABANCI-ULKE-TALEPLERINE-ISTINADEN-MALVARLIKLARI-DONDURULANLAR-6415-SAYILI-KANUN-6.csv", ';');
            return ConvertToAmlDataModels(list.Result, aml);
        }

        private List<AmlDataModel> ConvertToAmlDataModels(List<MasakCsvModel> list, AmlDataEnum aml)
        {
            var amlDataModels = list.Select(data =>
            {
                var amlData = new AmlDataModel();
                data.CopyValues<MasakCsvModel, AmlDataModel>(amlData);
                amlData.UniqId = (int)aml + data.NameTitle + data.IdentityNumber + data.OtherNameTitle + data.BirthDate + data.BirthPlace;
                return amlData;
            }).ToList();

            return amlDataModels;
        }
        private class MasakCsvModel
        {
            [Column("ADI SOYADI-ÜNVANI")]
            public string NameTitle { get; set; }
            [Column("TCKN-VKN-PASAPORT NO")]
            public string IdentityNumber { get; set; }
            [Column("UYRUĞU")]
            public string Nationality { get; set; }
            [Column("MVD YAPTIRIM TÜRÜ")]
            public string SanctionType { get; set; }
            [Column("KULLANDIĞI BİLİNEN DİĞER İSİMLERİ")]
            public string OtherNameTitle { get; set; }
            [Column("ANNE ADI")]
            public string MotherName { get; set; }
            [Column("BABA ADI")]
            public string FatherName { get; set; }
            [Column("DOĞUM TARİHİ")]
            public string BirthDate { get; set; }
            [Column("DOĞUM YERİ")]
            public string BirthPlace { get; set; }
            [Column("TABİ OLDUĞU DİĞER UYRUKLAR")]
            public string SecondNationality { get; set; }
            [Column("RESMİ GAZETE TARİH-SAYISI")]
            public string OfficialNewspaperDateIssue { get; set; }
        }
    }
}
