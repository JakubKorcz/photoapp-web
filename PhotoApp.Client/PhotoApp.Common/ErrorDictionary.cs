using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoApp.Common
{
    public static class ErrorDictionary
    {
        private static readonly Dictionary<string, string> _messages = new()
        {
            { "ERR001", "BŁĄD : Niepoprawne dane logowania." },
            { "ERR002", "BŁĄD : Twoje konto jest zablokowane." },
            { "ERR003", "BŁĄD : Sesja wygasła, zaloguj się ponownie." }
        };

        public static string GetMessage(string code) =>
            _messages.TryGetValue(code, out var message) ? message : "Wystąpił nieznany błąd.";
    }
}

