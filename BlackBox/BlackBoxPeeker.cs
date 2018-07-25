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
    // See also https://msdn.microsoft.com/en-us/library/system.reflection.memberinfo(v=vs.110).aspx
    //          https://www.tutorialspoint.com/csharp/csharp_reflection.htm
    //

    public class BlackBoxPeeker
    {
        private readonly string MainExecutableName = System.Reflection.Assembly.GetEntryAssembly().Location; // or  AppContext.BaseDirectory + AppDomain.CurrentDomain.FriendlyName; 
        private readonly string thisCodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

        public static void GetLocalVariables(Assembly asm, MethodInfo m)
        {
            // Assembly asm = Assembly.GetExecutingAssembly();
            ISymbolReader symreader = SymUtil.GetSymbolReaderForFile(asm.Location, null);

            if (symreader == null)
            {
                Console.WriteLine(" ERROR: no symreader was created. Aborting GetLocalVariables...");
                return;
            }
            ISymbolMethod symMethod = symreader.GetMethod(new SymbolToken(m.MetadataToken));

            int sequencePointCount = symMethod.SequencePointCount;

            ISymbolDocument[] docs = new ISymbolDocument[sequencePointCount];
            int[] offsets = new int[sequencePointCount];
            int[] lines = new int[sequencePointCount];
            int[] columns = new int[sequencePointCount];
            int[] endlines = new int[sequencePointCount];
            int[] endcolumns = new int[sequencePointCount];

            symMethod.GetSequencePoints(offsets, docs, lines, columns, endlines, endcolumns);

            Console.WriteLine();
            Console.WriteLine("The source code for method: " + m.Name + "; found in " + docs[0].URL);
            Console.WriteLine(new String('*', 60));

            // although there's an array of docs, they seem to all have the same value (?) so we'll only use the first one
            StreamReader reader = new StreamReader(docs[0].URL); // URL is typically a fully path-qualified filename
            string[]  linesOfCode = reader.ReadToEnd().Split('\r');

            string PrintableLineNumber = "0000";
            Console.WriteLine(linesOfCode[lines[0]-2].Replace('\n', ' ')); // the preceding line (assumes declaration is only one line long, and found on immediately precediting line!


            // foreach (int LineNumber in lines) // print the source code (comments omitted)
            for (int LineNumber = lines[0]; LineNumber < lines[sequencePointCount - 1] + 1; LineNumber++) // print the source code (including comments)
            {
                PrintableLineNumber = new String(' ', 4 - LineNumber.ToString().Length) // padding
                                      + LineNumber.ToString(); 
                Console.WriteLine(PrintableLineNumber + ": " + linesOfCode[LineNumber-1].Replace('\n',' '));
            }
            // Console.WriteLine(linesOfCode[lines[sequencePointCount -1] + 1].Replace('\n', ' ')); // the trailing line
            //Console.WriteLine(linesOfCode);
            reader.Close();

            Console.WriteLine(new String('*', 60));
            Console.WriteLine();
            linesOfCode = null;
        }

        public BlackBoxPeeker() // instantiation does all the interesting stuff
        {
            Console.WriteLine("Inspecting: " + MainExecutableName);
            Console.WriteLine("  from ");
            Console.WriteLine(thisCodeBase);
            Console.WriteLine();

            AppConfigKeyViewer.Show();

            IEnumerable<System.Reflection.TypeInfo> assemblyType = Assembly.LoadFile(MainExecutableName).DefinedTypes;
                    
            // see https://stackoverflow.com/questions/18362368/loading-dlls-at-runtime-in-c-sharp
            var DLL = Assembly.LoadFile(MainExecutableName);
            Console.WriteLine("Successfully loaded " + DLL.EntryPoint.DeclaringType.FullName + "; Image Runtime = " + DLL.ImageRuntimeVersion);
            Console.WriteLine(" Entry Point: " + DLL.EntryPoint);


            foreach (System.Reflection.TypeInfo assemblyItem in assemblyType)
            {
                Console.WriteLine("  Found Assembly Item: " + assemblyItem.FullName);
                var theType = DLL.GetType(assemblyItem.FullName);
                var c = Activator.CreateInstance(theType);

                // invoke all the main application methods
                foreach (System.Reflection.MethodInfo mi in assemblyItem.DeclaredMethods)
                {
                    {
                        Console.WriteLine(new String('-', 60));
                        Console.WriteLine("   Looking at method: " + mi.Name);
                        StackPeeker.GetCallStackDetails(mi.Name);
                        GetLocalVariables(DLL, mi);
                        LocalVariableNameReader lv = new LocalVariableNameReader(mi);

                        Console.WriteLine();
                        if (mi.Name != "Main") // we'd likely run into a recursion problem by re-invoking Main()
                        {
                            // see https://stackoverflow.com/questions/2202381/reflection-how-to-invoke-method-with-parameters
                            Console.WriteLine("  Invoking: " + mi.Name);
                            Console.WriteLine(new String('=', 120));
                            mi.Invoke(c, new object[] { });
                            Console.WriteLine(new String('=', 120));
                            Console.WriteLine();
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
