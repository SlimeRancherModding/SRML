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
                string filename = "";
                if (args.Length == 0)
                {
                    filename = GameFinder.FindGame();
                }
                else
                {
                    filename  = args[0];
                }
                
                string root = Path.GetDirectoryName(filename);
                string currentAssembly = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); //TODO: make it so it copies dependencies too
                var libFolder = Path.Combine(currentAssembly, "Libs");
                var currentSRML = Path.Combine(libFolder,SRML);
                if(!File.Exists(currentSRML)) throw new Exception($"Couldn't find {SRML}!");
                foreach (var v in Directory.GetFiles(libFolder))
                {
                    var file = Path.GetFileName(v);
                    var combine = Path.Combine(root, file);
                    var libPath = Path.Combine(libFolder, file);
                    if (File.Exists(combine))
                    {
                        Console.WriteLine($"Found old {file}! Replacing...");
                        File.Delete(combine);
                    }

                    File.Copy(libPath, combine);
                }
                var srmlPath = Path.Combine(root, SRML);

                var patcher = new Patcher(filename,GetOnLoad(srmlPath));

                if (patcher.IsPatched())
                {
                    throw new Exception($"Game is already patched!");
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
            Console.Write("Press any key to continue...");
            Console.ReadKey();
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
