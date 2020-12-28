using SPC.Services.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClassTest.Services
{
    class UserInputService_Imp: IUserInputService
    {
        Task<string> IUserInputService.GetStringMatchRegexAsync(string RegexRule, string Tips, string question, string defaultValue)
        {
            Console.Write($"{Tips} ({question}) :");
            var theInput = Console.ReadLine();
            if (string.IsNullOrEmpty(theInput) ) 
            {
                theInput = defaultValue;
            }
            return Task.FromResult(theInput);
        }
    }
}
