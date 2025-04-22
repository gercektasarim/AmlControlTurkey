using AmlControlTurkey.Core.Models;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace AmlControlTurkey.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);

            builder.Services.AddHangfire(config => config.UseMemoryStorage());
            builder.Services.AddHangfireServer();

            var indexPath = "path/to/lucene/index";
            builder.Services.AddSingleton(new LuceneSearchService(indexPath));
            builder.Services.AddSingleton(new BsaSearchService());

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services.AddSingleton<DataUpdater>();

            builder.Services.AddControllers();

            // Configure Swagger/OpenAPI
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AmlControlTurkey API",
                    Version = "v1",
                    Description = "API for AML Control Turkey",
                    Contact = new OpenApiContact
                    {
                        Name = "Türker Aktaþ",
                        Email = "turker.aktas81@gmail.com",
                        Url = new Uri("https://github.com/gercektasarim")
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AmlControlTurkey API v1");
                    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
                });
            }

            var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
            var dataUpdater = app.Services.GetRequiredService<DataUpdater>();

            recurringJobManager.AddOrUpdate("MasakADataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.MasakA), "1/20 * * * *");
            recurringJobManager.AddOrUpdate("MasakBDataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.MasakB), "1/20 * * * *");
            recurringJobManager.AddOrUpdate("MasakCDataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.MasakC), "1/20 * * * *");
            recurringJobManager.AddOrUpdate("MasakDDataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.MasakD), "1/20 * * * *");
            recurringJobManager.AddOrUpdate("SpkAdmDataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.SpkAdm), "2/20 * * * *");
            recurringJobManager.AddOrUpdate("SpkPersonDataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.SpkPerson), "2/20 * * * *");
            recurringJobManager.AddOrUpdate("SpkTbcDataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.SpkTbc), "2/20 * * * *");
            recurringJobManager.AddOrUpdate("TerrorismDataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.Terrorism), "3/20 * * * *");
            recurringJobManager.AddOrUpdate("TmsfDataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.Tmsf), "3/20 * * * *");
            
            //recurringJobManager.AddOrUpdate("OpenSectionsDataUpdater", () => dataUpdater.UpdateIndex(AmlDataEnum.OpenSection), "4/20 * * * *");
            RunJobsImmediately(recurringJobManager, dataUpdater);
            app.UseHttpsRedirection();
            app.UseHangfireDashboard();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void RunJobsImmediately(IRecurringJobManager recurringJobManager, DataUpdater dataUpdater)
        {
            recurringJobManager.Trigger("MasakADataUpdater");
            recurringJobManager.Trigger("MasakBDataUpdater");
            recurringJobManager.Trigger("MasakCDataUpdater");
            recurringJobManager.Trigger("MasakDDataUpdater");
            recurringJobManager.Trigger("SpkAdmDataUpdater");
            recurringJobManager.Trigger("SpkPersonDataUpdater");
            recurringJobManager.Trigger("SpkTbcDataUpdater");
            recurringJobManager.Trigger("TerrorismDataUpdater");
            recurringJobManager.Trigger("TmsfDataUpdater");
            //recurringJobManager.Trigger("OpenSectionsDataUpdater");
        }

    }
}
