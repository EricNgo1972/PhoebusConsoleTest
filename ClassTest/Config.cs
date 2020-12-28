using ClassTest.Services;
using SPC.Services.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPC.Helper.Extension;
using Newtonsoft.Json;

namespace ClassTest
{
    class Config
    {

        private async Task SetDBConnectionAsync()
        {

            await SPC.Ctx.AppConfig.GetDefaultConnectionAsync();

        }

     public static void RegisterServices()
        {

            SPC.Services.UI.AlertService.RegisterUIService(new AlertServices_Imp());
            SPC.Services.UI.WaitingService.RegisterUIService(new WaitingServices_Imp());
            SPC.Services.UI.UserInputService.RegisterUIService(new UserInputService_Imp());

            SPC.Services.UI.RunURLService.RegisterUIService(new RunUrlService_Imp());

            SPC.Ctx.AppConfig.RegisterService(new AppConfig_Imp());

            SPC.Ctx.AppConfig.LoadAppInfosAsync().TaskFireAndForget();
                                  

            SPC.Services.FormulaService.RegisterService(new SPC.Services.FormulaServiceImp());
            //SPC.Services.COM.MessageServices.RegisterService(new SPC.Commands.Mail.SendingService());

            SPC.Services.Cloud.Blob.RegisterService(new SPC.Cloud.Blob.BlobService());
            SPC.Services.Cloud.Table.RegisterService(new SPC.Cloud.Table.TableService());

            SPC.Services.LogService.RegisterService(new SPC.Cloud.LogService());

            SPC.Services.Storage.FileStorageService.RegisterService(new SPC.Services.Storage.SQLFileStreamService());
            SPC.Services.Storage.FileStorageService.RegisterService(new SPC.Services.Storage.DocLinkBlobService());

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };




            SPC.BO.ModuleCheckin.CheckIn();
            SPC.UsrMan.ModuleCheckin.CheckIn();
             //SPC.COM.ModuleCheckin.CheckIn();
            // SPC.BO.DM.ModuleCheckin.CheckIn();
            SPC.Cloud.ModuleCheckin.CheckIn();


        }

    }
}
