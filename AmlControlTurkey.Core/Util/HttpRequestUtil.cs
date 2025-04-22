
using RestSharp;
using ClosedXML.Excel;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.Text;
using AmlControlTurkey.Core.Extensions;
using System.Net;
using Newtonsoft.Json;

namespace AmlControlTurkey.Core.Util
{
    public static class HttpRequestUtil
    {
        public static async Task<List<T>> GetExcelData<T>(string url) where T : class, new()
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddHeader("cache-control", "no-cache");

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException($"Failed to download file from {url}. Status code: {response.StatusCode}");
            }

            using var stream = new MemoryStream(response.RawBytes);
            var result = new List<T>();

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            var propertyColumnMappings = typeof(T).GetProperties()
                .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() is ColumnAttribute)
                .ToDictionary(
                    p => ((ColumnAttribute)p.GetCustomAttributes(typeof(ColumnAttribute), false).First()).Name,
                    p => p
                );

            var headers = worksheet.Row(1).CellsUsed().Select(cell => cell.GetValue<string>()).ToList();
            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                var obj = new T();
                foreach (var header in headers)
                {
                    if (propertyColumnMappings.TryGetValue(header, out var property))
                    {
                        var cellValue = row.Cell(headers.IndexOf(header) + 1).Value;
                        var convertedValue = await cellValue.ConvertValue(property.PropertyType);
                        property.SetValue(obj, convertedValue.ToString());
                    }
                }
                result.Add(obj);
            }
            GC.Collect();
            return result;
        }

        public static async Task<T>? Post<T>(string address, Dictionary<string, string> headerParams, object? model,
        string? path = null)
        {
            var client = new RestClient(address);
            var request = new RestRequest(path ?? "", Method.Post);

            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");

            if (headerParams != null)
            {
                foreach (var param in headerParams)
                {
                    request.AddHeader(param.Key, param.Value);
                }
            }


            if (model != null)
                request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);

            var response = client.Execute(request);
            try
            {
                if (response.StatusCode is >= HttpStatusCode.OK and < (HttpStatusCode)300 or HttpStatusCode.Conflict)
                {
                    return JsonConvert.DeserializeObject<T>(response.Content);
                }
                else
                {
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(response.Content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return default;
        }

        public static async Task<T> Get<T>(string address)
        {
            var client = new RestClient(address);
            var request = new RestRequest();
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            var response = client.Execute(request);
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<T>(response.Content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return default;
        }
        public static async Task<List<T>> ReadCsvFromUrl<T>(string csvUrl, char delimiter, Encoding encoding = null) where T : class, new()
        {
            var provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);
            encoding ??= Encoding.GetEncoding("Windows-1254");
            using (var client = new WebClient())
            {
                var data = client.DownloadData(csvUrl);
                var config = new CsvConfiguration(new CultureInfo("tr-TR"))
                {
                    Delimiter = delimiter.ToString(),
                    MissingFieldFound = null
                };

                using (var stream = new MemoryStream(data))
                using (var reader = new StreamReader(stream, encoding))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = new List<T>();
                    var propertyColumnMappings = typeof(T).GetProperties()
                    .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() is ColumnAttribute)
                    .ToDictionary(
                    p => ((ColumnAttribute)p.GetCustomAttributes(typeof(ColumnAttribute), false).First()).Name,
                    p => p
                    );

                    csv.Read();
                    csv.ReadHeader();

                    var headers = csv.Context.Reader.HeaderRecord;
                    while (await csv.ReadAsync())
                    {
                        var obj = new T();
                        bool isEmptyRecord = true;
                        foreach (var header in headers)
                        {
                            if (propertyColumnMappings.TryGetValue(header, out var property))
                            {
                                var cellValue = csv.GetField(header);
                                var convertedValue = await cellValue.ConvertValue(property.PropertyType);
                                property.SetValue(obj, convertedValue);
                                
                                if (!string.IsNullOrWhiteSpace(cellValue))
                                {
                                    isEmptyRecord = false;
                                }
                            }
                        }
                        if (!isEmptyRecord)
                        {
                            records.Add(obj);
                        }
                    }
                    GC.Collect();
                    return records;
                }
            }
        }
    }
}
