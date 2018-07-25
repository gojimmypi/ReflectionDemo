using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;

namespace BlackBox
{
    public class LocalVariableNameReader
    {
        // from https://stackoverflow.com/questions/6166767/retrieve-local-variable-name-from-pdb-files
        //   "in order for this to work you have to have the pdb of the assembly available. 
        //   Also please make sure you add a refference to ISymWrapper and your project is targeting .Net 4.0 framework, 
        //   not .Net 4.0 Client."
        //
        // see also https://archive.codeplex.com/?p=ccimetadata

        Dictionary<int, string> _names = new Dictionary<int, string>();

        public string this[int index]
        {
            get
            {
                if (!_names.ContainsKey(index)) return null;
                return _names[index];
            }
        }

        public LocalVariableNameReader(MethodInfo m)
        {
            try
            {
                ISymbolReader symReader = SymUtil.GetSymbolReaderForFile(m.DeclaringType.Assembly.Location, null);
                ISymbolMethod met = symReader.GetMethod(new SymbolToken(m.MetadataToken));
                VisitLocals(met.RootScope);
            }
            catch
            {
                Console.WriteLine(" ERROR: Failed LocalVariableNameReader() - perhaps this app needs to be compiled in x86?");
                return;
            }
        }

        void VisitLocals(ISymbolScope iSymbolScope)
        {
            foreach (var s in iSymbolScope.GetLocals())
            {
                Console.WriteLine("  Found Local Variable: " + s.Name);
                _names[s.AddressField1] = s.Name;
            }
            foreach (var c in iSymbolScope.GetChildren()) VisitLocals(c);
        }
    }
}
