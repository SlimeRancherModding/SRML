using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML
{
    /// <summary>
    /// Utilities for communication between mods
    /// </summary>
    public static class IntermodCommunication
    {
        public delegate object CallMethodDelegate(params object[] args);

        internal static Dictionary<SRMod, Dictionary<string, IntermodCommunicationInfo>> IMCMethods = new Dictionary<SRMod,Dictionary<string,IntermodCommunicationInfo>>();

        static void RegisterIntermodMethod(SRMod mod, string imcmethodName, IntermodCommunicationInfo info)
        {
            if(!IMCMethods.TryGetValue(mod, out var dict))
            {
                dict = new Dictionary<string, IntermodCommunicationInfo>();
                IMCMethods[mod] = dict;
            }

            dict.Add(imcmethodName, info);
        }

        static bool TryGetIMCInfo(SRMod mod, string imcMethod, out IntermodCommunicationInfo info)
        {
            info = default(IntermodCommunicationInfo);
            if (mod == null) return false;
            if (!IMCMethods.TryGetValue(mod, out var imc)) return false;
            if (!imc.TryGetValue(imcMethod,out info)) return false;
            return true;
        }

        public static String CallingMod { get; private set; }

        static void RegisterIntermodMethod(string imcmethodName,IntermodCommunicationInfo info)
        {
            // TODO: Upgrade to new method
            //RegisterIntermodMethod(SRMod.GetCurrentMod(), imcmethodName, info);
        }

        public static void RegisterIntermodMethod(string imcmethodName, CallMethodDelegate method, Type[] paramtypes, Type returnType)
        {
            RegisterIntermodMethod(imcmethodName, new IntermodCommunicationInfo(method, paramtypes, returnType));
        }

        public static void RegisterIntermodMethod(string imcmethodName, CallMethodDelegate method)
        {
            RegisterIntermodMethod( imcmethodName, new IntermodCommunicationInfo(method, null, null));
        }

        public static void RegisterIntermodMethod<TResult>(string imcmethodName, Func<TResult> method)
        {
            RegisterIntermodMethod( imcmethodName,new IntermodCommunicationInfo((x)=>method(),new Type[0],typeof(TResult)));
        }

        public static void RegisterIntermodMethod<T,TResult>(string imcmethodName, Func<T,TResult> method)
        {
            RegisterIntermodMethod(imcmethodName, new IntermodCommunicationInfo((x) => method((T)x[0]), new Type[] { typeof(T) }, typeof(TResult)));
        }

        public static void RegisterIntermodMethod<T, K, TResult>(string imcmethodName, Func<T, K, TResult> method)
        {
            RegisterIntermodMethod(imcmethodName, new IntermodCommunicationInfo((x) => method((T)x[0],(K)x[1]), new Type[] { typeof(T),typeof(K) }, typeof(TResult)));
        }

        public static void RegisterIntermodMethod<T, K, R, TResult>(string imcmethodName, Func<T, K, R, TResult> method)
        {
            RegisterIntermodMethod(imcmethodName, new IntermodCommunicationInfo((x) => method((T)x[0], (K)x[1],(R)x[2]), new Type[] { typeof(T), typeof(K), typeof(R) }, typeof(TResult)));
        }

        public static void RegisterIntermodMethod<T, K, R, Q, TResult>(string imcmethodName, Func<T, K, R, Q, TResult> method)
        {
            RegisterIntermodMethod(imcmethodName, new IntermodCommunicationInfo((x) => method((T)x[0], (K)x[1], (R)x[2], (Q)x[3]), new Type[] { typeof(T), typeof(K), typeof(R),typeof(Q) }, typeof(TResult)));
        }

        public static void RegisterIntermodMethod<T>(string imcmethodName, Action<T> method)
        {
            RegisterIntermodMethod(imcmethodName, new IntermodCommunicationInfo((x) => { method((T)x[0]); return null; }, new Type[] { typeof(T)},typeof(void)));
        }

        public static void RegisterIntermodMethod<T,K>(string imcmethodName, Action<T,K> method)
        {
            RegisterIntermodMethod(imcmethodName, new IntermodCommunicationInfo((x) => { method((T)x[0],(K)x[1]); return null; }, new Type[] { typeof(T), typeof(K) }, typeof(void)));
        }

        public static void RegisterIntermodMethod<T, K, Q>(string imcmethodName, Action<T, K, Q> method)
        {
            RegisterIntermodMethod(imcmethodName, new IntermodCommunicationInfo((x) => { method((T)x[0], (K)x[1], (Q)x[2]); return null; }, new Type[] { typeof(T), typeof(K), typeof(Q) }, typeof(void)));
        }

        public static void RegisterIntermodMethod<T, K, Q, R>(string imcmethodName, Action<T, K, Q, R> method)
        {
            RegisterIntermodMethod(imcmethodName, new IntermodCommunicationInfo((x) => { method((T)x[0], (K)x[1], (Q)x[2], (R)x[3]); return null; }, new Type[] { typeof(T), typeof(K), typeof(Q),typeof(R) }, typeof(void)));
        }

        public static object CallIMCMethod(string modid, string methodName,params object[] args)
        {
            
            try
            {
                // TODO: Upgrade to new system
                SRMod currentMod = /*SRMod.GetCurrentMod()*/null;
                var mod = SRModLoader.GetMod(modid);
                if (mod == null) throw new Exception($"Non-Existent modid: '{modid}'");
                if (!TryGetIMCInfo(mod, methodName, out var info)) throw new Exception($"Non-Existent IMC method '{methodName}' for mod '{modid}'");
                CallingMod = currentMod?.ModInfo.Id ?? "srml";
                return info.Invoke(args);
            }
            finally
            {   
                CallingMod = null;
            }
        }

        internal struct IntermodCommunicationInfo
        {
            public CallMethodDelegate MethodDelegate;
            public Type[] Types;
            public Type ReturnType;

            public IntermodCommunicationInfo(CallMethodDelegate methodDelegate, Type[] types, Type returnType)
            {
                MethodDelegate = methodDelegate;
                Types = types;
                ReturnType = returnType;
            }

            public override string ToString()
            {
                if (Types == null) return "UNKNOWN CALLING INFORMATION";
                StringBuilder builder = new StringBuilder();
                foreach(var v in Types)
                {
                    builder.Append(v.ToString()+", ");
                }

                builder.Append("returns " + ReturnType.ToString());
                return builder.ToString();
            }

            public object Invoke(params object[] args)
            {
                if (args!=null && args.Length != Types.Length) throw new ArgumentException(ToString());
                return MethodDelegate(args);
            }
        }
    }
}
