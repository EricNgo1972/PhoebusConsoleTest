using SPC;
using SPC.CORE;
using SPC.Interfaces;
using SPC.Services.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassTest.Services
{
    class RunUrlService_Imp: IRunURLService
    {
        Task IRunURLService.RunAsync(CmdArg Arg)
        {

            Run(Arg);
            return Task.CompletedTask;
        }


        void Run(CmdArg Arg)
        {
            try
            {

                var pClass = SPCTypes.GetClassType(Arg.GetShortCutSegment(0));
                if (pClass != null)
                {
                    //Runable non UI command
                    if (pClass.GetInterfaces().Contains(typeof(IRunable)))
                    {
                        var Sample = BOFactory.CreateSample(pClass) as IRunable;

                        Task.Run(async () => await Sample.RunAsync(Arg));

                    }

                }
                else
                {
                    var prs = new ProcessStartInfo(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
                    prs.Arguments = Arg.Url.Replace(@" ", "%20");
                    Process.Start(prs);
                }



            }
            catch (Exception ex)
            {
                WaitingService.Done();
                AlertService.ShowError(ex);

            }
        }



    }
}
