using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;


namespace BlackBox
{
    public class AppConfigKeyViewer
    {
        public static void Show()
        {
            Console.WriteLine("Access main application AppSettings[]:");
            foreach (string key in ConfigurationManager.AppSettings)
            {
                Console.WriteLine(" key: " + key + "; value = " + ConfigurationManager.AppSettings[key]);
            }
            Console.WriteLine();
        }
        public AppConfigKeyViewer()
        {
        }
    }
}

