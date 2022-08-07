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
            bool vortexMode = args.Length > 0 && args[0].StartsWith("-v");
            bool vortexUninstall = vortexMode && args[0] == "-vu";

            try
            {
                string filename = "";
                if (!vortexMode && args.Length == 0)
                    filename = GameFinder.FindGame();
                else
                    filename = (vortexMode ? args[1] : args[0]) + "/SlimeRancher_Data/Managed/Assembly-CSharp.dll";

                string root = Path.GetDirectoryName(filename);
                var srmlPath = Path.Combine(root, SRML);

                Console.WriteLine();

                bool uninstalling = false;
                bool alreadypatched = false;
                bool didSrmlExist = File.Exists(srmlPath);

                try_to_patch:
                if (File.Exists(srmlPath))
                {
                    var patcher = new Patcher(filename, GetOnLoad(srmlPath));
                    if (patcher.IsPatched())
                    {
                        alreadypatched = true;
                        if (!vortexMode)
                        {
                            Console.Write($"Game is already patched! Would you like to uninstall? (selecting n will instead trigger an update) (y/n): ");

                            poll_user:
                            var response = Console.ReadLine();
                            if (response == "yes" || response == "y")
                            {
                                uninstalling = true;
                                patcher.Uninstall();
                            }
                            else if (response == "n" || response == "no")
                            {
                            }
                            else
                            {
                                Console.Write("Please enter a valid option! (y/n): ");
                                goto poll_user;
                            }
                        }

                        if (vortexUninstall)
                        {
                            uninstalling = true;
                            patcher.Uninstall();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Patching...");
                        patcher.Patch();
                        Console.WriteLine("Patching Successful!");
                        patcher.Save();
                    }

                    patcher.Dispose();
                    SendFilesOver(didSrmlExist);
                }
                else
                {
                    SendFilesOver();
                    goto try_to_patch;
                }

                string GetAlternateRoot()
                {
                    string alternateRoot = Path.Combine(Directory.GetParent(root).Parent.FullName, "SRML", "Libs");
                    if (!Directory.Exists(alternateRoot))
                    {
                        Directory.CreateDirectory(alternateRoot);
                    }

                    return alternateRoot;
                }

                void SendFilesOver(bool canLog = true)
                {
                    foreach(var file in Directory.GetFiles(GetAlternateRoot()))
                        File.Delete(file);
                    
                    foreach (var v in Assembly.GetExecutingAssembly().GetManifestResourceNames().Where((x) =>
                        x.Length > embeddedResourceProject.Length &&
                        x.Substring(0, embeddedResourceProject.Length) == embeddedResourceProject))
                    {
                        var file = v.Substring(embeddedResourcePath.Length);
                        var combine = Path.Combine(file.Contains("SRML") ? root:GetAlternateRoot(), file);
                        //var libPath = Path.Combine(libFolder, file);
                        if (File.Exists(combine))
                        {
                            if (canLog)
                                if (!uninstalling) Console.WriteLine($"Found old {file}! Replacing...");
                                else Console.WriteLine($"Deleting {file}...");
                            File.Delete(combine);
                        }
                        if (uninstalling) continue;
                        var str = File.Create(combine);
                        var otherStream = Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream(typeof(Program), v.Substring(embeddedResourceProject.Length));
                        otherStream.CopyTo(str);
                        otherStream.Close();
                        str.Close();
                    }

                    if (!uninstalling)
                    {
                        string umfPath = Path.Combine(Directory.GetParent(root).Parent.FullName, "uModFramework", "Lib", "net462");
                        if (File.Exists(Path.Combine(umfPath, "0Harmony.dll")))
                        {
                            Console.WriteLine("Found existing UMF installation! Patching...");
                            File.Copy(Path.Combine(GetAlternateRoot(), "0Harmony.dll"), Path.Combine(umfPath, "0Harmony.dll"), true);
                        }
                    }
                }

                Console.WriteLine();
                
                string type = alreadypatched ? "Update" : "Installation";
                string ending = alreadypatched? "" : $"(old assembly stored as { Path.GetFileNameWithoutExtension(filename)}_old.dll)";

                if (!uninstalling)
                {
                    Console.WriteLine(
                        $"{type} complete! "+ending);
                    var modpath = Path.Combine(Directory.GetParent(root).Parent.FullName, "SRML", "Mods");
                    if (!Directory.Exists(modpath)) Directory.CreateDirectory(modpath);
                    Console.WriteLine($"Mods can be installed at {modpath}");
                }
                else Console.WriteLine($"Uninstallation complete!");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"Please run {Path.GetFileName(Assembly.GetExecutingAssembly().Location)} as an administrator!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (vortexMode) return;

            Console.Write("Press any key to continue...");
            Console.ReadKey();
            return;
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
