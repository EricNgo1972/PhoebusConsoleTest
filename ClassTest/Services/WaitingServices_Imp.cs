using SPC.Services.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassTest.Services
{
    class WaitingServices_Imp : IWaitingPanel
    {
        void IWaitingPanel.Done() => Console.WriteLine("Done");
        void IWaitingPanel.Wait(string Title, string Message) => Console.WriteLine($".:| {Title} : {Message} ...");
    }
}
