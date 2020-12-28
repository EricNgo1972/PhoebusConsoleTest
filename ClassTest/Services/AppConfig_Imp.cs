using SPC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static SPC.Ctx;
using SPC.Helper.Extension;
using System.Data.SqlClient;
using System.Data;
using FlexCel.Core;
using SPC.Services.UI;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ClassTest.Services
{
    class AppConfig_Imp: SPC.Ctx.IAppConfigInfo
    {
        const string Server = "T470-01";
        const string Database = "VSA_DEMO";
        const string UserId = "sa";
        const string Password = "123456";

        private Dictionary<string, string> _appConfig;
        Dictionary<string, string> Ctx.IAppConfigInfo.GetApplicationConfigInfos()
        {
            if (_appConfig == null)
            {
                _appConfig = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            return _appConfig;
        }
        Task Ctx.IAppConfigInfo.LoadAppInfosAsync()
        {
            var dic = ((IAppConfigInfo) this).GetApplicationConfigInfos();

            dic.Add(nameof(Ctx.AppConfig.AppId), "Phoebus Console");

            return Task.CompletedTask;
        }


        async Task Ctx.IAppConfigInfo.SetDefaultConnectionAsync(string ConnnectionCode)
        {

            var cbuilder = new SqlConnectionStringBuilder();
            cbuilder.DataSource = Server;
            cbuilder.InitialCatalog = Database;
            cbuilder.UserID = UserId;
            cbuilder.Password = Password;
            cbuilder.IntegratedSecurity = false;
           
            string cs = cbuilder.ConnectionString;

            WaitingService.Wait("Testing Connection to",$"Server {cbuilder.DataSource}/{cbuilder.InitialCatalog}");

           
            var dic = ((IAppConfigInfo) this).GetApplicationConfigInfos();
            dic.InsertUpdate(Ctx.AppConfig.SQLConnectionString, cs);
            dic.InsertUpdate(nameof(Ctx.AppConfig.DBEngine), "SQL");
            dic.InsertUpdate(nameof(Ctx.AppConfig.UseAppServer), "N");


            using (IDbConnection con = await SPC.Database.ConnectionFactory.GetDBConnectionAsync(true))
            {
                if(con.State == ConnectionState.Open)                
                    AlertService.Alert($"Connected to Server {cbuilder.DataSource}/{cbuilder.InitialCatalog}");                
                else
                    AlertService.Alert($"Failed to connect to Server {cbuilder.DataSource}/{cbuilder.InitialCatalog}");
            }

        }


      

    }
}
