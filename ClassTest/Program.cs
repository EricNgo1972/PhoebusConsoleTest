using Microsoft.Extensions.Configuration;
using SPC;
using SPC.Services.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SPC.Helper.Extension;
using System.Collections;
using System.Xml.Linq;
using SPC.BO;
using SPC.Services;
using System.ComponentModel.Design;
using SPC.CORE;
using System.Linq;
using SPC.Interfaces;

namespace ClassTest
{
    class Program
    {
        async static Task Main(string[] args)
        {
            try
            {


                Console.WriteLine("Phoebus Console");
                Console.WriteLine("--------------------");
                Config.RegisterServices();

                await SPC.Security.AzureLogin.LoginWithSubscriberId("local@spc-technology.com", "123");

                await SPC.Ctx.AppConfig.SetDefaultConnectionAsync("Local");

                await SPC.BO.PS.DB.SwitchToAsync("VSA");

                string theclassName = "";
                do
                {
                
                    theclassName  = await SPC.Services.UI.UserInputService.GetStringMatchRegexAsync("", "Enter the Full Name of the Editable Class:", "(Ex: SPC.BO.LA.NA)");

                    var theType = SPCTypes.GetClassType(theclassName, true);
                    if (theType != null)
                    {
                        if (theType.GetInterfaces().Contains(typeof(ISingleton)))
                            await TestSingleton(theType);
                        else if (theType.GetInterfaces().Contains(typeof(SPC.Interfaces.IEditable)))                        
                            await TestEditable(theType);
                    }

                } while (string.IsNullOrEmpty(theclassName));


            }
            catch (Exception ex)
            {
                AlertService.ShowError(ex);
            }
            Console.ReadKey();
            WaitingService.Done();

        }


        #region Testing

        private async static Task TestSingleton(Type pType)
        {
            string editable = pType.ToString();
            var filters = new Dictionary<string, string>();
            object theobj = null;

            if (await BOFactory.IsBOExistsAsync(editable, filters))
            {
                Console.WriteLine("Found runable {0} : {1}", editable, filters.ToParametersString());
                theobj = await BOFactory.GetBOAsync(editable, filters);

            }
            else
            {
                Console.WriteLine("object {0}:{1} does not exists", editable, filters.ToParametersString());
                theobj = await BOFactory.NewBOAsync(editable, filters);
            }


            var jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(theobj);
            Console.WriteLine(jsonstr);
        }

        private async static Task TestEditable(Type pType)
        {
            string editable = pType.ToString();

            //await CommandRouter.RunCommand("SPC.HR.Commands.LRQ.Approve", param);
            Console.WriteLine($"InfoList {pType.ToString()}-------------");

            //IList list1 = await BOFactory.GetInfoListAsync(editable, new Dictionary<string, string> { ["SubscriberId"] = "dinhlasunhn@gmail.com" }) as IList;

            IList list1 = await BOFactory.GetInfoListAsync(editable, null) as IList;

            var jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(list1);
            Console.WriteLine(jsonstr);

            Console.WriteLine("Total {0} records", ((IEnumerable) list1).Count());

            Console.WriteLine("Editable. Enter the record index: ");

            var index = Console.ReadLine().ToInteger();

            var onerec = list1[index];

            var filters = BOFactory.GetObjectCriteria(onerec);

            if (await BOFactory.IsBOExistsAsync(editable, filters))
            {
                Console.WriteLine("Found editable {0} : {1}", editable, filters.ToParametersString());
                var theobj = await BOFactory.GetBOAsync(editable, filters);

                //((SPC.BO.HR.EAW) theobj).Concurrent = "Y";

                jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(theobj);
                Console.WriteLine(jsonstr);

                try
                {
                    var wf = theobj as PFD;
                    if (wf != null)
                    {
                        var theData = wf.Data.Decompress();

                        var formXele = XElement.Parse(theData);

                        var theMaster = formXele.Element("master");
                        Console.WriteLine("ClassName : {0} ", theMaster.GetStringAttribute("ClassName"));
                        //      Console.WriteLine("Data : {0} ", SPC.Services.Base64Serializer.DeserializeObject<object>( theMaster.Value.Decompress()));

                        var theProperties = formXele.Element("properties").Value;
                        var theDic = theProperties.Base64DeSerializer() as Dictionary<string, string>;
                        jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(theDic);
                        Console.WriteLine(jsonstr);
                    }

                }
                catch (Exception)
                {

                }


                // Console.WriteLine("Convert ToXML: {0}", (theobj as DLS).ToXml().ToString());

                Console.WriteLine("Copying selected record . Enter the new ID: ");
                var newId = Console.ReadLine();

                if (!string.IsNullOrEmpty(newId) && newId != editable.ToString())
                {

                    var thecopy = await BOFactory.CopyBOAsync(theobj, newId);

                    Console.WriteLine($"Saving the copy of {theobj.GetType().ToString()} to database with new Id :{newId}");

                    var saved = await (thecopy as SPC.Interfaces.IEditable).SaveBOAsync();

                    Console.WriteLine("New object Saved: ");

                    jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(saved);
                    Console.WriteLine(jsonstr);

                    Console.WriteLine($"Test deleting BO-------{saved.GetType().ToString()} : {saved.ToString()}--------");

                    var delete = await BOFactory.DeleteBOAsync(pType.ToString(), saved.ToString());

                    Console.WriteLine("Object has been deleted: ");

                    jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(delete);
                    Console.WriteLine(jsonstr);

                }

            }
            else
                Console.WriteLine("object {0}:{1} does not exists", editable, filters.ToParametersString());


        }

        private async static Task<string> TestNewEditable(Type pType, Dictionary<string, string> Data)
        {
            try
            {
                Console.WriteLine("Test creating a new  BO");
                var obj = await BOFactory.NewBOAsync(pType.ToString(), Data);

                BOFactory.ApplyPreset(obj, Data);

                var saved = await (obj as SPC.Interfaces.IEditable).SaveBOAsync();

                Console.WriteLine("New object Saved: ");

                var jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(saved);
                Console.WriteLine(jsonstr);


                return saved.ToString();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }

        private async static Task TestUpdateEditable(Type pType, Dictionary<string, string> Data)
        {
            try
            {
                Console.WriteLine("Test update existing  BO");

                var obj = await BOFactory.GetBOAsync(pType.ToString(), Data);

                BOFactory.ApplyPreset(obj, Data);

                var saved = await (obj as SPC.Interfaces.IEditable).SaveBOAsync();

                Console.WriteLine("Object has been amended: ");

                var jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(saved);
                Console.WriteLine(jsonstr);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

        }

        private async static Task TestDeleteEditable(Type pType, Dictionary<string, string> Data)
        {
            try
            {

                Console.WriteLine("Test deleting BO");

                var isExists = await BOFactory.IsBOExistsAsync(pType.ToString(), Data);

                if (isExists)
                {
                    var delete = await BOFactory.DeleteBOAsync(pType.ToString(), Data);

                    Console.WriteLine("Object has been deleted: ");

                    var jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(delete);
                    Console.WriteLine(jsonstr);
                }
                else
                    Console.WriteLine("Object you wnat to delete does not exists");


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

        }


        #endregion

    }
}
