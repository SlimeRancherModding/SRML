using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRML.SR.SaveSystem.Data.Partial
{

    internal class CompoundPartialData<T> : PartialData<T>
    {
        private List<PartialData> partialDatas = new List<PartialData>();

        public CompoundPartialData(IEnumerable<PartialData> partialData)
        {
            partialDatas.AddRange(partialData);
            partialDatas.Sort(new PartialDataComparer());
        }
        public override void Pull(T data)
        {
            partialDatas.ForEach((x)=>x.Pull(data));
        }

        public override void Push(T data)
        {
            partialDatas.ForEach((x) => x.Push(data));
        }

        public override void Read(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                partialDatas[i].Read(reader);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(partialDatas.Count);
            partialDatas.ForEach((x)=>x.Write(writer));
        }

    }

    internal class PartialDataComparer : IComparer<PartialData>
    {
        public int Compare(PartialData x, PartialData y)
        {
            if (x.DataType == y.DataType) return 0;
            if (x.DataType.IsAssignableFrom(y.DataType)) return -1;
            return 1;
        }
    }

    internal abstract class VersionedPartialData<T> : PartialData<T>, IVersionedSerializable
    {
        public abstract int LatestVersion { get; }

        public int Version { get; protected set; }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(LatestVersion);
            WriteData(writer);
        }

        public override void Read(BinaryReader reader)
        {
            Version = reader.ReadInt32();
            ReadData(reader);
        }

        public abstract void WriteData(BinaryWriter writer);
        public abstract void ReadData(BinaryReader reader);
    }

    internal abstract class PartialData<T> : PartialData
    {
        public abstract void Pull(T data);
        public abstract void Push(T data);

        public override Type DataType => typeof(T);

        public override void Pull(object data)
        {
            Pull((T)data);
        }

        public override void Push(object data)
        {
            Push((T)data);
        }

        public abstract override void Write(BinaryWriter writer);
        public abstract override void Read(BinaryReader reader);
    }
    internal abstract partial class PartialData : ISerializable
    {
        public abstract void Pull(object data);
        public abstract void Push(object data);
        public abstract Type DataType { get; }
        public abstract void Write(BinaryWriter writer);
        public abstract void Read(BinaryReader reader);
    }

    internal abstract partial class PartialData
    {
        public delegate PartialData PartialDataCreatorDelegate();

        public delegate PartialData<T> PartialDataCreatorGenericDelegate<T>();
        static Dictionary<Type,PartialDataCreatorDelegate> partialDataCreators = new Dictionary<Type, PartialDataCreatorDelegate>();



        public static void RegisterPartialData(Type type, PartialDataCreatorDelegate creator)
        {
            partialDataCreators[type] = creator;
        }

        public static void RegisterPartialData<T>(PartialDataCreatorGenericDelegate<T> creator)
        {
            RegisterPartialData(typeof(T),()=>(PartialData)creator());
        }

        public static void RegisterPartialData<T>(Type type)
        {
            RegisterPartialData(typeof(T),()=>(PartialData)Activator.CreateInstance(type));
        }

        public static void RegisterPartialData<K,T>() where T : PartialData<K>
        {
            RegisterPartialData<K>(typeof(T));
        }

        public static bool HasPartialData(Type type,bool allowInherited = false)
        {
            return partialDataCreators.ContainsKey(type)||(allowInherited&&partialDataCreators.Any((x)=>x.Key.IsAssignableFrom(type)));
        }

        static PartialDataCreatorDelegate GetPartialDataCreator(Type type,bool allowInherited = false)
        {
            return HasPartialData(type,allowInherited) ? partialDataCreators.First((x)=>type == x.Key || x.Key.IsAssignableFrom(type)).Value : null;
        }

        public static PartialData GetPartialData(Type type,bool allowInherited = false)
        {
            return GetPartialDataCreator(type,allowInherited)?.Invoke();
        }

        public static bool TryGetPartialData(Type type,out PartialData data, bool allowInherited = false)
        {
            data = GetPartialData(type,allowInherited);
            return data != null;
        }

        public static PartialData GetPartialData<T>(bool allowInherited = false)
        {
            return GetPartialData(typeof(T),allowInherited);
        }

        public static bool TryGetPartialData<T>(out PartialData data,bool allowInherited=false)
        {
            return TryGetPartialData(typeof(T), out data,allowInherited);
        }

        public static CompoundPartialData<T> CreateCompoundPartialData<T>()
        {
            return new CompoundPartialData<T>(partialDataCreators.Where((x)=>x.Key.IsAssignableFrom(typeof(T))).Select((x)=>(PartialData)x.Value()));
        }

        static PartialData()
        {
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, IDictionaryProvider v) =>
                {
                    translator.FixEnumValues(mode,v.InternalDictionary);
                });
            EnumTranslator.RegisterEnumFixer(
                (EnumTranslator translator, EnumTranslator.TranslationMode mode, IListProvider v) =>
                {
                    translator.FixEnumValues(mode,v.InternalList);
                });
        }
    }

}
