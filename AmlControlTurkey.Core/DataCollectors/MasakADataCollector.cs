using AmlControlTurkey.Core.Extensions;
using AmlControlTurkey.Core.Models;
using AmlControlTurkey.Core.Util;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlControlTurkey.Core.DataCollectors
{
    public class MasakADataCollector() : IDataCollector
    {
        public async Task<List<AmlDataModel>> ProcessData(AmlDataEnum aml)
        {
            var list = HttpRequestUtil.ReadCsvFromUrl<MasakCsvModel>("https://ms.hmb.gov.tr/uploads/sites/12/2025/03/A-6415-SAYILI-KANUN-5.-MADDE-2-3556173a5e2fad15.csv", ';');
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
            [Column("AD-SOYAD/ÜNVANI")]
            public string NameTitle { get; set; }
            [Column("TCKN/VKN/YKN/\nGKN/PASAPORT/ULUSAL  KİMLİK  NO")]
            public string IdentityNumber { get; set; }
            [Column("ADRES")]
            public string Addresses { get; set; }
            [Column("KULLANDIĞI BİLİNEN DİĞER İSİMLERİ")]
            public string OtherNameTitle { get; set; }
            [Column("Adının Orijinal Dilde Yazımı")]
            public string OtherInformation { get; set; }
            [Column("UYRUĞU")]
            public string Nationality { get; set; }
            [Column("MVD YAPTIRIM TÜRÜ")]
            public string SanctionType { get; set; }
            [Column("ANNE ADI")]
            public string MotherName { get; set; }
            [Column("BABA ADI")]
            public string FatherName { get; set; }
            [Column("DOĞUM TARİHİ")]
            public string BirthDate { get; set; }
            [Column("DOĞUM YERİ")]
            public string BirthPlace { get; set; }
            [Column("BAĞLANTILI OLDUĞU ÖRGÜT")]
            public string Organization { get; set; }
            [Column("RESMİ GAZETE TARİH- SAYISI")]
            public string OfficialNewspaperDateIssue { get; set; }
            [Column("KARAR SAYISI")]
            public string DecisionDateNumber { get; set; }
        }
    }
}
