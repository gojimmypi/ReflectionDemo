using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;

namespace BlackBoxDemo
{
    // See also https://msdn.microsoft.com/en-us/library/system.reflection.memberinfo(v=vs.110).aspx
    //          https://www.tutorialspoint.com/csharp/csharp_reflection.htm
    //

    public class BlackBox
    {
        private readonly string MainExecutableName = System.Reflection.Assembly.GetEntryAssembly().Location; // or  AppContext.BaseDirectory + AppDomain.CurrentDomain.FriendlyName; 
        private readonly string thisCodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

        // from https://social.msdn.microsoft.com/Forums/vstudio/en-US/02962055-3e07-4d93-83ea-6973154d9724/getting-stack-local-variables?forum=netfxbcl
        /// <summary>
        /// If the method_name is given, show the details of the method designated
        /// If the method_name is String.Empty, show the details of the whole call stack.
        /// The details include the following:
        /// - method name
        /// - name and type of the method paramters
        /// - index and type of local variables
        /// </summary>
        /// <param name="method_name"></param>
        public static void GetCallStackDetails(String method_name)
        {
            StackTrace st = new StackTrace();
            StackFrame[] sfs = st.GetFrames();
            foreach (StackFrame sf in sfs)
            {
                MethodInfo method = sf.GetMethod() as MethodInfo;
                MethodBody method_body = null;
                if (method != null)
                {
                    method_body = method.GetMethodBody();

                    if (method_name == String.Empty || method.Name.Equals(method_name))
                    {
                        Console.WriteLine(new String('-', 60));
                        Console.WriteLine("Call stack for: " + method.Name);
                        Console.WriteLine(" Found {0} local variable(s)", method_body.LocalVariables.Count);
                        Console.WriteLine(" Paramters:");
                        ParameterInfo[] pis = method.GetParameters();
                        foreach (ParameterInfo pi in pis)
                        {
                            Console.WriteLine(" Name:{0} Type:{1}", pi.Name, pi.ParameterType.ToString());
                        }
                        Console.WriteLine(" Local Variables:");
                        foreach (LocalVariableInfo lvi in method_body.LocalVariables)
                        {
                            Console.WriteLine(" Index:{0} Type:{1}", lvi.LocalIndex, lvi.LocalType.ToString());
                        }
                    }
                }
            }
            Console.WriteLine(new String('-', 60));
        }

        public static void GetLocalVariables(Assembly asm, MethodInfo m)
        {
            // Assembly ass = Assembly.GetExecutingAssembly();
            ISymbolReader symreader = SymUtil.GetSymbolReaderForFile(asm.Location, null);

            // MethodInfo m = ass.GetType("PdbTest.TestClass").GetMethod("GetStringRepresentation");
            ISymbolMethod met = symreader.GetMethod(new SymbolToken(m.MetadataToken));

            int count = met.SequencePointCount;

            ISymbolDocument[] docs = new ISymbolDocument[count];
            int[] offsets = new int[count];
            int[] lines = new int[count];
            int[] columns = new int[count];
            int[] endlines = new int[count];
            int[] endcolumns = new int[count];

            met.GetSequencePoints(offsets, docs, lines, columns, endlines, endcolumns);

            Console.WriteLine("The source code for method: " + m.Name);
            Console.WriteLine(new String('*', 60));
            // although there's an array of docs, they seem to all have the same value (?) so we'll only use the first one
            StreamReader reader = new StreamReader(docs[0].URL);
            string  linesOfCode = reader.ReadToEnd();
            Console.WriteLine(linesOfCode);
            reader.Close();

            Console.WriteLine(new String('*', 60));
            Console.WriteLine();
            linesOfCode = null;
        }

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
                    
            // see https://stackoverflow.com/questions/18362368/loading-dlls-at-runtime-in-c-sharp
            var DLL = Assembly.LoadFile(MainExecutableName);

            foreach (System.Reflection.TypeInfo assemblyItem in assemblyType)
            {
                Console.WriteLine("Found Assembly Item: " + assemblyItem.FullName);
                var theType = DLL.GetType(assemblyItem.FullName);
                var c = Activator.CreateInstance(theType);

                // invoke all the main application methods
                foreach (System.Reflection.MethodInfo mi in assemblyItem.DeclaredMethods)
                {
                    {
                        Console.WriteLine(new String('-', 60));
                        Console.WriteLine("Looking at method: " + mi.Name);
                        GetCallStackDetails(mi.Name);
                        GetLocalVariables(DLL, mi);
                        LocalVariableNameReader lv = new LocalVariableNameReader(mi);

                        Console.WriteLine();
                        if (mi.Name != "Main") // we'd likely run into a recursion problem by re-invoking Main()
                        {
                            // see https://stackoverflow.com/questions/2202381/reflection-how-to-invoke-method-with-parameters
                            Console.WriteLine("Invoking: " + mi.Name);
                            mi.Invoke(c, new object[] { });
                        }
                    }
                }

                // show all the main application fields
                foreach (System.Reflection.FieldInfo fi in assemblyItem.DeclaredFields)
                {
                    // See https://stackoverflow.com/questions/43251571/c-sharp-methodinfo-invoke
                    Console.WriteLine("Found " + fi.Attributes + " variable: " + fi.Name + " = " + fi.GetValue(c));
                }
            }
        }
    }

}
