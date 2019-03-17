using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SRMLInstaller
{
    class Patcher
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
            var pathRoot = Directory.GetParent(filename).ToString();
            string patchedname = Path.Combine(pathRoot,
                Path.GetFileNameWithoutExtension(filename) + "_patched.dll");
            string oldname = Path.Combine(pathRoot,
                Path.GetFileNameWithoutExtension(filename) + "_old.dll");
            if(!CheckOrDelete(patchedname)||!CheckOrDelete(oldname)) throw new Exception();
            curAssembly.Write(patchedname);
            curAssembly.Dispose();
            File.Move(filename,oldname);
            File.Move(patchedname,filename);
            
        }
    }
}
