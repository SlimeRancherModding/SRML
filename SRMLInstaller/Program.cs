using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace SRMLInstaller
{
    class Program
    {
        public const String SRML = "SRML.dll";
        static void Main(string[] args)
        {
            try
            {
                string filename = args[0];
                string root = Directory.GetParent(filename).ToString();
                string currentAssembly = Directory.GetParent(Assembly.GetEntryAssembly().Location).ToString();
                var currentSRML = Path.Combine(currentAssembly,SRML);
                if(!File.Exists(currentSRML)) throw new Exception($"Couldn't find {SRML}!");
                var combine = Path.Combine(root, SRML);
                if (File.Exists(combine))
                {
                    Console.WriteLine($"Found old {SRML}! Replacing...");
                    File.Delete(combine);
                }
                File.Copy(currentSRML,combine);
                
                var patcher = new Patcher(filename,GetOnLoad(combine));

                if (patcher.IsPatched())
                {
                    throw new Exception($"{filename} is already patched!");
                }

                Console.WriteLine("Patching...");
                patcher.Patch();
                Console.WriteLine("Patching Successful!");
                patcher.Save();
                Console.WriteLine($"Installation complete! (old assembly stored as {Path.GetFileNameWithoutExtension(filename)}_old.dll)");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        static MethodReference GetOnLoad(string path)
        {
            AssemblyDefinition def = null;
            try
            {
                def = AssemblyDefinition.ReadAssembly(path);
            }
            catch
            {
                throw new Exception($"Couldn't find {SRML}!");
            }

            return def.MainModule.GetType("SRML.Main").Methods.First((x) => x.Name == "PreLoad");

        }
    }
}
