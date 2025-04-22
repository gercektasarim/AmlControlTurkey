using AmlControlTurkey.Core.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlControlTurkey.Core.DataCollectors
{
    public interface IDataCollector
    {
        Task<List<AmlDataModel>> ProcessData(AmlDataEnum aml);
    }

    public class DataCollectorContext
    {
        private IDataCollector _dataCollector;
        public DataCollectorContext(IDataCollector dataCollector)
        {
            _dataCollector = dataCollector;
        }

        public void SetStrategy(IDataCollector dataCollector)
        {
            _dataCollector = dataCollector;
        }

        public async Task<List<AmlDataModel>> Execute(AmlDataEnum aml)
        {
            return await _dataCollector.ProcessData(aml);
        }
    }
}
