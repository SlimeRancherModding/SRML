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
        public const String embeddedResourcePath = "SRMLInstaller.Libs.";
        public const String embeddedResourceProject = "SRMLInstaller.";
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
                    filename = args[0];
                }

                string root = Path.GetDirectoryName(filename);

                Console.WriteLine();

                foreach (var v in Assembly.GetExecutingAssembly().GetManifestResourceNames().Where((x) =>
                    x.Length > embeddedResourceProject.Length &&
                    x.Substring(0, embeddedResourceProject.Length) == embeddedResourceProject))
                {
                    var file = v.Substring(embeddedResourcePath.Length);
                    var combine = Path.Combine(root, file);
                    //var libPath = Path.Combine(libFolder, file);
                    if (File.Exists(combine))
                    {
                        Console.WriteLine($"Found old {file}! Replacing...");
                        File.Delete(combine);
                    }

                    var str = File.Create(combine);
                    var otherStream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(typeof(Program), v.Substring(embeddedResourceProject.Length));
                    otherStream.CopyTo(str);
                    otherStream.Close();
                    str.Close();
                }

                Console.WriteLine();

                var srmlPath = Path.Combine(root, SRML);

                var patcher = new Patcher(filename, GetOnLoad(srmlPath));

                if (patcher.IsPatched())
                {

                    Console.WriteLine($"Game is already patched! Update complete!");
                    goto onsuccess;
                }

                Console.WriteLine("Patching...");
                patcher.Patch();
                Console.WriteLine("Patching Successful!");
                patcher.Save();
                Console.WriteLine();
                Console.WriteLine(
                    $"Installation complete! (old assembly stored as {Path.GetFileNameWithoutExtension(filename)}_old.dll)");


                onsuccess:
                var modpath = Path.Combine(Directory.GetParent(root).Parent.FullName, "SRML", "Mods");
                if (!Directory.Exists(modpath)) Directory.CreateDirectory(modpath);
                Console.WriteLine($"Mods can be installed at {modpath}");

            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"Please run {Path.GetFileName(Assembly.GetExecutingAssembly().Location)} as an administrator!");
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
