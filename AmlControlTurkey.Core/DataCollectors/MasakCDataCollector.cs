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
    public class MasakCDataCollector() : IDataCollector
    {
        public async Task<List<AmlDataModel>> ProcessData(AmlDataEnum aml)
        {
            var list = HttpRequestUtil.ReadCsvFromUrl<MasakCsvModel>("https://ms.hmb.gov.tr/uploads/sites/12/2025/01/6415-SAYILI-KANUN-7.csv", ';');
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

        public class MasakCsvModel
        {
            [Column("GERÇEK/TÜZEL KİŞİ/KURULUŞ/ORGANİZASYON ADI SOYADI ÜNVANI")]
            public string NameTitle { get; set; }
            [Column("KULLANDIĞI BİLİNEN DİĞER İSİMLERİ")]
            public string OtherNameTitle { get; set; }
            [Column("TCKN/VKN/GKN/PASAPORT NO")]
            public string IdentityNumber { get; set; }
            [Column("UYRUĞU")]
            public string Nationality { get; set; }
            [Column("MVD YAPTIRIM TÜRÜ")]
            public string SanctionType { get; set; }
            [Column("ANNE ADI")]
            public string MotherName { get; set; }
            [Column("BABA ADI")]
            public string FatherName { get; set; }
            [Column("DOĞUM TARİHİ/KURULUŞ")]
            public string BirthDate { get; set; }
            [Column("DOĞUM YERİ")]
            public string BirthPlace { get; set; }
            [Column("ÖRGÜTÜ")]
            public string Organization { get; set; }
            [Column("KARAR TARİH-SAYISI")]
            public string DecisionDateNumber { get; set; }
            [Column("RESMİ GAZETE TARİH SAYISI")]
            public string OfficialNewspaperDateIssue { get; set; }
            [Column("TABİ OLDUĞU DİĞER UYRUK")]
            public string SecondNationality { get; set; }
        }
    }
}
