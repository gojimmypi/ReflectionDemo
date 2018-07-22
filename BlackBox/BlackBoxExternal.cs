using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;
 

namespace BlackBoxDemo
{
    public class BlackBox
    {
        private readonly string MainExecutableName = System.Reflection.Assembly.GetEntryAssembly().Location; // or  AppContext.BaseDirectory + AppDomain.CurrentDomain.FriendlyName; 
        private readonly string thisCodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

        //private void PeekAtVariables()
        //{
        //    foreach (System.Reflection.FieldInfo item in ((System.Reflection.TypeInfo)typeof(Program)).DeclaredFields) {
        //        Console.WriteLine("Found: " + item.Name + " = " + item.ToString());
        //    }
        //}

        public BlackBox() // instantiation does all the interesting stuff
        {
            Console.WriteLine("Inspecting: " + MainExecutableName);
            Console.WriteLine("  from ");
            Console.WriteLine(thisCodeBase);
            Console.WriteLine();

            Console.WriteLine("Access main application AppSettings[]:");
            Console.WriteLine(ConfigurationManager.AppSettings["mySecretString"]);
            Console.WriteLine();

            IEnumerable<System.Reflection.TypeInfo> assemblyType = Assembly.LoadFile(MainExecutableName).DefinedTypes;

            var DLL = Assembly.LoadFile(MainExecutableName);

            foreach (System.Reflection.TypeInfo assemblyItem in assemblyType)
            {
                Console.WriteLine("Found Assembly Item: " + assemblyItem.FullName);
                //var thisType = DLL.GetType(assemblyItem.UnderlyingSystemType());
                if (assemblyItem.FullName != "myProgram.BlackBox")
                {
                    var theType = DLL.GetType(assemblyItem.FullName);
                    var c = Activator.CreateInstance(theType);

                    // invoke all the main application methods
                    foreach (System.Reflection.MethodInfo mi in assemblyItem.DeclaredMethods)
                    {
                        {
                            if (mi.Name != "Main") // we'd likely run into a recursion problem by re-invoking Main()
                            {
                                mi.Invoke(c, new object[] { });
                            }
                        }
                    }

                    // show all the main application fields
                    foreach(System.Reflection.FieldInfo fi in assemblyItem.DeclaredFields)
                    {
                        Console.WriteLine("Found " + fi.Attributes + " variable: " + fi.Name + " = " + fi.GetValue(c));
                    }
                }
            }
        }
    }

}
