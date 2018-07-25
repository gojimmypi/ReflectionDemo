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
    class StackPeeker
    {
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
    }
}
