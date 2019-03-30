using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SRMLInstaller
{
    class Patcher : IDisposable
    {
        private AssemblyDefinition curAssembly;
        private MethodDefinition target;
        private MethodReference methodToPatchIn;
        private String filename;
        public Patcher(String filename,MethodReference methodToPatchIn)
        {
            try
            {
                curAssembly = AssemblyDefinition.ReadAssembly(filename);
            }
            catch
            {
                throw new Exception($"Couldn't find {filename}!");
            }

            target = FindTarget();
            this.methodToPatchIn = methodToPatchIn;
            this.filename = filename;
        }

        MethodDefinition FindTarget()
        {
            return curAssembly.MainModule.GetType("GameContext").Methods.First((x) => x.Name == "Awake");
        }

        public bool IsPatched()
        {
            return target.Body.Instructions[0].OpCode == OpCodes.Call && target.Body.Instructions[0].Operand is MethodReference methRef && methRef.FullName==methodToPatchIn.FullName;
        }

        public void Patch()
        {
            target.Body.GetILProcessor().InsertBefore(target.Body.Instructions[0],target.Body.GetILProcessor().Create(OpCodes.Call,curAssembly.MainModule.ImportReference(methodToPatchIn)));
        }

        public void Unpatch()
        {
            target.Body.GetILProcessor().Remove(target.Body.Instructions[0]);
        }

        bool CheckOrDelete(String path)
        {
            if (!File.Exists(path)) return true;
            Console.Write($"Found {Path.GetFileName(path)} in target directory! Delete? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                Console.WriteLine("Deleting and continuing...");
                File.Delete(path);
                return true;
            }
            Console.WriteLine("Aborting...");
            return false;
        }
        public void Save()
        {
            var pathRoot = Path.GetDirectoryName(filename);
            string patchedname = Path.Combine(pathRoot,
                Path.GetFileNameWithoutExtension(filename) + "_patched.dll");
            string oldname = Path.Combine(pathRoot,
                Path.GetFileNameWithoutExtension(filename) + "_old.dll");
            if(!CheckOrDelete(patchedname)||!CheckOrDelete(oldname)) throw new Exception();
            curAssembly.Write(patchedname);
            Dispose();
            File.Move(filename,oldname);
            File.Move(patchedname,filename);
            
        }

        public void Dispose()
        {
            curAssembly?.Dispose();
            methodToPatchIn?.Module?.Assembly?.Dispose();
        }

        public void Uninstall()
        {
            var pathRoot = Path.GetDirectoryName(filename);
            string oldname = Path.Combine(pathRoot,
                Path.GetFileNameWithoutExtension(filename) + "_old.dll");
            string patchedname = Path.Combine(pathRoot,
                Path.GetFileNameWithoutExtension(filename) + "_time_to_delete_it.dll");
            if (!File.Exists(oldname))
            {
                Console.WriteLine($"Couldn't find old {Path.GetFileName(filename)}!");
                Console.WriteLine("Attempting forceful uninstallation...");
                Unpatch();
                if (!CheckOrDelete(patchedname)) throw new Exception();
                curAssembly.Write(patchedname);
                Dispose();
                File.Delete(filename);
                File.Move(patchedname,filename);
            }
            else
            {
                Dispose();
                File.Delete(filename);
                File.Move(oldname,filename);
            }
            Console.WriteLine("Unpatching Successful!");
        }
    }
}
