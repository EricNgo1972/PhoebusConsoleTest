using SPC.Services.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassTest.Services
{
    class AlertServices_Imp: IAlert
    {
        void IAlert.Alert(string pFormatString, params object[] ParamArray) => Console.WriteLine($".:| {pFormatString}.", ParamArray);
        void IAlert.ShowError(Exception ex) => Console.WriteLine($"!!!Exception: {ex}");
        void IAlert.Toast(string pFormatString, params object[] ParamArray) => Console.WriteLine($".:| {pFormatString}.", ParamArray);
    }
}
