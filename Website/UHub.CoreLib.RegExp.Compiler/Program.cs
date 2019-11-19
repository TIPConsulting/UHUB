using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UHub.CoreLib.RegExp.Attributes;

namespace UHub.CoreLib.RegExp.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            List<RegexCompilationInfo> regexes = new List<RegexCompilationInfo>();


            Assembly.GetAssembly(typeof(RgxPtrns)).GetTypes().ToList()
                .ForEach(rgxType =>
                {
                    Console.WriteLine(rgxType);

                    rgxType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField).ToList()
                    .ForEach(rgxStr =>
                    {
                        if (rgxStr.MemberType != MemberTypes.Field)
                        {
                            return;
                        }

                        var parentType = rgxStr.DeclaringType;


                        var assemName = parentType.Assembly.FullName.Split(',')[0];
                        var compiledNamespace = assemName + ".Compiler.Output." + parentType.Name;

                        Console.WriteLine(compiledNamespace + "." + rgxStr.Name);
                        var rgxVal = rgxStr.GetValue(null) as string;


                        var flagAttr = rgxStr.GetCustomAttribute<RegexOptionFlagAttribute>();
                        int flagVal = 0;
                        if (flagAttr != null)
                        {
                            flagVal = flagAttr.Value;
                        }
                        RegexOptions rgxOpts = (RegexOptions)flagVal;



                        RegexCompilationInfo SentencePattern =
                                 new RegexCompilationInfo(rgxVal,
                                                          rgxOpts,
                                                          rgxStr.Name,
                                                          compiledNamespace,
                                                          true);

                        regexes.Add(SentencePattern);

                    });

                });




            var assemblyName = "UHub.CoreLib.RegExp.Compiler.Output";
            AssemblyName compiledAssemName = new AssemblyName($"{assemblyName}, Version=1.0.0.1000, Culture=neutral, PublicKeyToken=null");
            Regex.CompileToAssembly(regexes.ToArray(), compiledAssemName);



            //Place in {solution}\_ExternalDLLs
            var src = $"{assemblyName}.dll";
            var dest = $@"..\..\..\_ExternalDLLs\{assemblyName}.dll";
            if (File.Exists(dest))
            {
                File.Delete(dest);
            }
            File.Move(src, dest);


            var autoGenBat = @"..\..\..\UHub.CoreLib.RegExp.Compiled\T4AutoGen.bat";

            var proc = new Process();
            proc.StartInfo.FileName = autoGenBat;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.LoadUserProfile = true;
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
            proc.WaitForExit();


        }
    }
}
