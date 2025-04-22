using AmlControlTurkey.Core.Extensions;
using AmlControlTurkey.Core.Models;
using AmlControlTurkey.Core.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AmlControlTurkey.Core.DataCollectors.MasakCDataCollector;

namespace AmlControlTurkey.Core.DataCollectors
{
    public class MasakDDataCollector() : IDataCollector
    {
        public async Task<List<AmlDataModel>> ProcessData(AmlDataEnum aml)
        {
            var list = HttpRequestUtil.ReadCsvFromUrl<MasakCsvModel>("https://ms.hmb.gov.tr/uploads/sites/12/2024/12/D-7262-SAYILI-KANUN-3.A-VE-3.B-MADDELERI.csv", ';');
            return ConvertToAmlDataModels(list.Result, aml);
        }

        private List<AmlDataModel> ConvertToAmlDataModels(List<MasakCsvModel> list, AmlDataEnum aml)
        {
            var amlDataModels = list.Select(data =>
            {
                var amlData = new AmlDataModel();
                data.CopyValues<MasakCsvModel, AmlDataModel>(amlData);
                amlData.UniqId = (int)aml + data.NameTitle + data.IdentityNumber + data.OtherNameTitle + data.BirthDate;
                return amlData;
            }).ToList();

            return amlDataModels;
        }

        private class MasakCsvModel
        {
            [Column("Gerçek Kişi Soyadı Ünvanı ")]
            public string NameTitle { get; set; }
            [Column("Eski Adı")]
            public string NameTitle2 { get; set; }
            [Column("\nPasaport No/ Diğer Muhtelif  Bilgiler \n")]
            public string IdentityNumber { get; set; }
            [Column("Örgütü")]
            public string Organization { get; set; }
            [Column("Kullandığı Bilinen Diğer İsmler")]
            public string OtherNameTitle { get; set; }
            [Column("Diğer Bilgiler")]
            public string OtherInformation { get; set; }
            [Column("Görevi")]
            public string Mission { get; set; }
            [Column("Adres")]
            public string Addresses { get; set; }
            [Column("Uyruğu")]
            public string Nationality { get; set; }
            [Column("Listeye Alınma Tarihi")]
            public string RecordDate { get; set; }
            [Column("Anne Adı")]
            public string MotherName { get; set; }
            [Column("Baba Adı")]
            public string FatherName { get; set; }
            [Column("Doğum Tarihi")]
            public string BirthDate { get; set; }
            [Column("R.Gazete Tarih Sayı")]
            public string OfficialNewspaperDateIssue { get; set; }
            [Column("BKK-CBK Karar Tarih ve Sayısı")]
            public string DecisionDateNumber { get; set; }
        }
    }
}
