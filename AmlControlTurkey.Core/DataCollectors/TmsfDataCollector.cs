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
    public class TmsfDataCollector() : IDataCollector
    {
        public async Task<List<AmlDataModel>> ProcessData(AmlDataEnum aml)
        {
            var list = await HttpRequestUtil.GetExcelData<TmsfModel>("https://www.tmsf.org.tr/tr/Sirket/ExportExcel");
            return ConvertToAmlDataModels(list, aml);
        }

        private List<AmlDataModel> ConvertToAmlDataModels(List<TmsfModel> list, AmlDataEnum aml)
        {
            var amlDataModels = list.Select(data =>
            {
                var amlData = new AmlDataModel();
                data.CopyValues<TmsfModel, AmlDataModel>(amlData);
                amlData.UniqId = (int)aml + data.NameTitle + data.IdentityNumber;
                return amlData;
            }).ToList();

            return amlDataModels;
        }

        public class TmsfModel
        {
            [Column("TİCARET SİCİLDEKİ UNVANI")]
            public string NameTitle { get; set; }
            [Column("VERGİ KİMLİK NUMARASI")]
            public string IdentityNumber { get; set; }
        }
    }
}
