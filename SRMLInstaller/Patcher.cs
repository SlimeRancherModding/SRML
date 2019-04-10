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
            return target.Body.Instructions[0].OpCode == OpCodes.Call && target.Body.Instructions[0].Operand is MethodReference methRef && (methRef.Name=="LoadSRML" || methRef.Name=="PreLoad");
        }

        public MethodDefinition AddLoadMethod()
        {
            if (target.DeclaringType.Methods.FirstOrDefault((x) => x.Name == "LoadSRML") is MethodDefinition d
            ) return d;

            var method = new MethodDefinition("LoadSRML",MethodAttributes.Public|MethodAttributes.Static,target.Module.TypeSystem.Void);
            var proc = method.Body.GetILProcessor();
            var maincall = target.Body.GetILProcessor().Create(OpCodes.Call, curAssembly.MainModule.ImportReference(methodToPatchIn));
            if (!curAssembly.MainModule.TryGetTypeReference("UnityEngine.Debug", out var reference))
                throw new Exception("Couldn't find UnityEngine.Debug");
            var logref = new MethodReference("Log", curAssembly.MainModule.TypeSystem.Void, reference);
            logref.Parameters.Add(new ParameterDefinition(curAssembly.MainModule.TypeSystem.Object));
            var onfailwrite = proc.Create(OpCodes.Call,logref);

            if (!curAssembly.MainModule.TryGetTypeReference("UnityEngine.Application", out var quitreference))
                throw new Exception("Couldn't find UnityEngine.Application");

            var applicationquit = proc.Create(OpCodes.Call,new MethodReference("Quit",curAssembly.MainModule.TypeSystem.Void,quitreference));

            var ret = proc.Create(OpCodes.Ret);
            var leave = proc.Create(OpCodes.Leave, ret);

            var mainret = proc.Create(OpCodes.Ret);

            proc.Append(maincall);
            proc.InsertAfter(maincall, mainret);
            proc.InsertAfter(mainret,onfailwrite);
            proc.InsertAfter(onfailwrite,applicationquit);
            proc.InsertAfter(applicationquit,leave);
            proc.InsertAfter(leave,ret);

            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = method.Body.Instructions.First(),
                TryEnd = onfailwrite,
                HandlerStart = onfailwrite,
                HandlerEnd = ret,
                CatchType = curAssembly.MainModule.ImportReference(typeof(Exception)),
            };
            


            method.Body.ExceptionHandlers.Add(handler);

            target.DeclaringType.Methods.Add(method);

            return method;
        }

        public void Patch()
        {
            var proc = target.Body.GetILProcessor();
            proc.InsertBefore(target.Body.Instructions[0],proc.Create(OpCodes.Call,AddLoadMethod()));

        }

        public void Unpatch()
        {
            target.Body.GetILProcessor().Remove(target.Body.Instructions[0]);
            if (target.DeclaringType.Methods.FirstOrDefault((x) => x.Name == "LoadSRML") is MethodDefinition d)
            {
                target.DeclaringType.Methods.Remove(d);
            }
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
            if(!CheckOrDelete(patchedname)||!CheckOrDelete(oldname)) throw new Exception("Cannot continue installation while this file exists!");
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
