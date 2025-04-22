using AmlControlTurkey.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlControlTurkey.Core.DataCollectors
{
    public class DataCollectorFactory
    {
        public static IDataCollector GetDataCollector(AmlDataEnum dataCollectorType)
        {
            return dataCollectorType switch
            {
                AmlDataEnum.OpenSection => new OpenSectionDataCollector(),
                AmlDataEnum.MasakA => new MasakADataCollector(),
                AmlDataEnum.MasakB => new MasakBDataCollector(),
                AmlDataEnum.MasakC => new MasakCDataCollector(),
                AmlDataEnum.MasakD => new MasakDDataCollector(),
                AmlDataEnum.SpkTbc => new SpkTbcDataCollector(),
                AmlDataEnum.SpkAdm => new SpkAdmDataCollector(),
                AmlDataEnum.SpkPerson => new SpkPersonDataCollector(),
                AmlDataEnum.Terrorism => new TerrorismDataCollector(),
                AmlDataEnum.Tmsf => new TmsfDataCollector(),
                _ => throw new ArgumentException("Invalid DataCollectorType")
            };
        }

    }
}
