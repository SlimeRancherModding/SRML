using SRML.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data
{
    public class DataPiece
    {
        public DataType typeId { get; protected set; }
        public string key { get; protected set; }
        internal object data { get; set; }

        internal static readonly Dictionary<DataType, SerializerPair> serializerPairs = new Dictionary<DataType, SerializerPair>()
        {

            {DataType.INT32,new SerializerPair<int>((x,y)=>x.Write(y),(x)=>x.ReadInt32()) },
            {DataType.INT64,new SerializerPair<long>((x,y)=>x.Write(y),(x)=>x.ReadInt64()) },
            {DataType.BYTE,new SerializerPair<byte>((x,y)=>x.Write(y),(x)=>x.ReadByte()) },
            {DataType.UINT32,new SerializerPair<uint>((x,y)=>x.Write(y),(x)=>x.ReadUInt32()) },
            {DataType.UINT64,new SerializerPair<ulong>((x,y)=>x.Write(y),(x)=>x.ReadUInt64()) },
            {DataType.FLOAT,new SerializerPair<float>((x,y)=>x.Write(y),(x)=>x.ReadSingle()) },
            {DataType.DOUBLE,new SerializerPair<double>((x,y)=>x.Write(y),(x)=>x.ReadDouble()) },
            {DataType.VECTOR2,new SerializerPair<Vector2>(BinaryUtils.WriteVector2,BinaryUtils.ReadVector2) },
            {DataType.VECTOR3,new SerializerPair<Vector3>(BinaryUtils.WriteVector3,BinaryUtils.ReadVector3) },
            {DataType.VECTOR4,new SerializerPair<Vector4>(BinaryUtils.WriteVector4,BinaryUtils.ReadVector4) },
            {DataType.QUATERNION,new SerializerPair<Quaternion>(BinaryUtils.WriteQuaternion,BinaryUtils.ReadQuaternion) },
            {DataType.MATRIX4,new SerializerPair<Matrix4x4>(BinaryUtils.WriteMatrix4,BinaryUtils.ReadMatrix4) },
            {DataType.STRING,new SerializerPair<string>((x,y)=>x.Write(y),(x)=>x.ReadString()) },
            {DataType.COMPOUND,new SerializerPair<HashSet<DataPiece>>((x, y) =>
            {
                x.Write(y.Count);
                foreach (var v in y)
                {
                    Serialize(x, v);
                }
            },
            (x) =>
            {
                var newList = new HashSet<DataPiece>();
                int count = x.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    newList.Add(Deserialize(x));
                }

                return newList;

            }
            ) },
            {DataType.ARRAY, new SerializerPair<Array>((x, y) =>
            {
                var arrayType = y.GetType().GetElementType();
                DataType typeId;
                try
                {
                    typeId=GetTypeID(arrayType);
                }
                catch{
                    throw new Exception("Array holds type that is invalid!");
                }

                if(!serializerPairs.TryGetValue(typeId,out var serializer))throw new Exception($"Unrecognized Data Type {typeId}");

                x.Write((int)typeId);
                x.Write(y.Length);
                if(typeId!=DataType.ENUM)
                {
                    foreach (var v in y)
                    {
                        serializer.Serialize(x, v);
                    }
                }
                else
                {
                    var intSerializer = serializerPairs[DataType.INT32];
                    x.Write(arrayType.AssemblyQualifiedName);
                    for(int i = 0; i < y.Length;i++)
                    {
                            intSerializer.Serialize(x,(int)y.GetValue(i));
                    }
                    
                }
            },
            (x) =>
            {
                var typeId = (DataType)x.ReadInt32();
                if(typeId!=DataType.ENUM){
                var arrayType = typeToDataType.FirstOrDefault((g) => g.Value == typeId).Key;
                if (arrayType == null) throw new Exception($"Unrecognized Data Type {typeId}");
                if(!serializerPairs.TryGetValue(typeId,out var serializer)) throw new Exception($"Unrecognized Data Type {typeId}");
                int count = x.ReadInt32();
                var array = Array.CreateInstance(arrayType, count);

                for (int i = 0; i < count; i++)
                {
                    array.SetValue(serializer.Deserialize(x), i);
                }

                return array;
                }
                else
                {
                    int count = x.ReadInt32();
                    var str = x.ReadString();
                    Type type = Type.GetType(str);
                    var array = Array.CreateInstance(type,count);
                    var intSerializer = serializerPairs[DataType.INT32];
                    for(int i = 0; i < count; i++)
                    {
                        array.SetValue(Enum.ToObject(type,(int)intSerializer.Deserialize(x)),i);
                    }
                    return array;
                }
            }) },
            {DataType.BOOLEAN,new SerializerPair<bool>((x,y)=>x.Write(y),(x)=>x.ReadBoolean()) },
            {DataType.COLOR, new SerializerPair<Color>((x, y) => {
                x.Write(y.r);
                x.Write(y.g);
                x.Write(y.b);
                x.Write(y.a);
            },
            (x) => { return new Color(x.ReadSingle(), x.ReadSingle(), x.ReadSingle(), x.ReadSingle()); }) },
            {DataType.ENUM, new SerializerPair<object>((x,y)=>{
            x.Write(y.GetType().AssemblyQualifiedName);x.Write((int)y); },(x)=>Enum.ToObject(Type.GetType(x.ReadString()),x.ReadInt32()))}
        };

        internal static readonly Dictionary<Type, DataType> typeToDataType = new Dictionary<Type, DataType>();

        static DataPiece()
        {
            foreach (var v in serializerPairs)
            {
                typeToDataType.Add(v.Value.GetSerializedType(), v.Key);
            }
            EnumTranslator.RegisterEnumFixer<DataPiece>((translator, mode, piece) =>
            {

                if (piece.dataFixer != null) piece.dataFixer.Invoke(translator, mode, piece.data); else if (piece.typeId == DataType.ENUM) piece.data = translator.TranslateEnum(piece.data.GetType(), mode, piece.data); else translator.FixEnumValues(mode, piece.data);
                
            });
        }

        internal static DataPiece GetNewDataPiece(DataType forType)
        {
            if (forType == DataType.COMPOUND) return new CompoundDataPiece();
            return new DataPiece();

        }

        public override string ToString()
        {
            return $"{typeId} {key} = {data.ToString()};";
        }

        internal static DataPiece Deserialize(BinaryReader reader)
        {
            var type = (DataType)reader.ReadInt32();
            if (!serializerPairs.TryGetValue(type, out var serializer))
                throw new Exception($"Unrecognized Data Type {type}");
            var piece = GetNewDataPiece(type);
            piece.typeId = type;
            piece.key = reader.ReadString();
            piece.data = serializer.Deserialize(reader);
            return piece;
        }


        internal static void Serialize(BinaryWriter writer, DataPiece piece)
        {

            if (!serializerPairs.TryGetValue(piece.typeId, out var serializer))
                throw new Exception($"Unrecognized Data Type {piece.typeId}");
            writer.Write((int)piece.typeId);
            writer.Write(piece.key);
            serializer.Serialize(writer, piece.data);
        }

        internal void Serialize(BinaryWriter writer)
        {
            Serialize(writer, this);
        }

        internal DataPiece() { }

        public DataPiece(string key, object value) : this(key, value.GetType())
        {
            this.data = value;
        }

        public object GetValue()
        {
            return data;

        }

        public T GetValue<T>()
        {
            return (T) data;

        }

        public void SetValue(object b)
        {
            if (typeId != GetTypeID(b))
                throw new Exception($"Tried to set data piece to a value invalid for its type ID");
            this.data = b;
        }

        public void SetValue<T>(T b)
        {
            if (typeId != GetTypeID(typeof(T)))
                throw new Exception($"Tried to set data piece to a value invalid for its type ID");
            this.data = b;
        }


        public DataPiece(string key, Type valueType)
        {
            this.key = key;
            this.typeId = GetTypeID(valueType);
        }

        public static DataType GetTypeID(Type type)
        {
            return type.IsArray ? DataType.ARRAY : type.IsEnum ? DataType.ENUM : (typeToDataType.TryGetValue(type,out var dataType) ? dataType : DataType.NULL);
        }

        public static DataType GetTypeID(object val)
        {
            return GetTypeID(val.GetType());
        }

        public Type GetDataType()
        {
            return typeId == DataType.ENUM?data?.GetType():typeToDataType.FirstOrDefault((g) => g.Value == typeId).Key;
        }

        public override bool Equals(object obj)
        {
            var piece = obj as DataPiece;
            return piece != null &&
                   key == piece.key&&typeId==piece.typeId&&data==piece.data;
        }

        public override int GetHashCode()
        {
            return 249886028 + EqualityComparer<string>.Default.GetHashCode(key);
        }


        EnumTranslator.EnumFixerDelegate dataFixer;

        public virtual void SetEnumTranslator(EnumTranslator.EnumFixerDelegate del)
        {
            dataFixer = del;   
        }
        public static DataPiece ObjectToPiece(object obj)
        {
            List<object> serializedObjects = new List<object>();
           
            return ObjectToPieceInternal(obj);
            DataPiece ObjectToPieceInternal(object b)
            {
                
                
                if (DataPiece.GetTypeID(b.GetType()) != DataType.NULL) return new DataPiece("root", b);
                
                var comp = new CompoundDataPiece("root");
                if (serializedObjects.Any(x=>object.ReferenceEquals(b,x))) return comp;
                
                comp.SetValue("__type", b.GetType().AssemblyQualifiedName);

                void ProcessCollection(ICollection collection, string name)
                {

                    if (collection == null) return;
                    if (collection is IDictionary dict)
                    {
                        var dictComp = new CompoundDataPiece(name);
                        dictComp.SetValue("__type", dict.GetType().AssemblyQualifiedName);
                        
                        var valueComp = dictComp.GetCompoundPiece("values");
                        var keyComp = dictComp.GetCompoundPiece("keys");
                        var keyEnumerator = dict.Keys.GetEnumerator();
                        var valueEnumerator = dict.Values.GetEnumerator();
                        keyEnumerator.MoveNext();
                        valueEnumerator.MoveNext();
                        for (int i = 0; i < dict.Count; i++)
                        {
                            var valuePiece = ObjectToPieceInternal(valueEnumerator.Current);
                            valuePiece.key = i.ToString();
                            valueComp.AddPiece(valuePiece);

                            var keyPiece = ObjectToPieceInternal(keyEnumerator.Current);
                            keyPiece.key = i.ToString();
                            keyComp.AddPiece(keyPiece);

                            valueEnumerator.MoveNext();
                            keyEnumerator.MoveNext();
                        }
                        comp.AddPiece(dictComp);
                    }
                    else
                    {
                        var arrayComp = new CompoundDataPiece(name);
                        arrayComp.SetValue("__type", collection.GetType().AssemblyQualifiedName);

                        int counter = 0;
                        foreach (var v in collection)
                        {
                            var piece = ObjectToPieceInternal(v);
                            piece.key = counter.ToString();
                            arrayComp.AddPiece(piece);
                            counter++;
                        }
                        comp.AddPiece(arrayComp);
                    }
                }
                if (b is ICollection colle)
                {
                    ProcessCollection(colle, "root");
                    return comp.GetPiece("root", null);
                }
                else
                foreach (var field in b.GetType().GetFields())
                {
                    if (DataPiece.typeToDataType.ContainsKey(field.FieldType))
                    {
                        comp.SetValue(field.Name, field.GetValue(b));
                    }
                    else if (typeof(ICollection).IsAssignableFrom((field.FieldType)))
                    {
                        ProcessCollection(field.GetValue(b) as ICollection, field.Name);
                    }
                    else
                    {
                        serializedObjects.Add(b);
                        if (field.GetValue(b) == null) continue;
                        var piece = ObjectToPieceInternal(field.GetValue(b));
                        piece.key = field.Name;
                        comp.AddPiece(piece);
                    }

                }
                return comp;
            }

        }

        public static object PieceToObject(DataPiece piece)
        {
            if (DataPiece.GetTypeID(piece.GetDataType()) != DataType.COMPOUND) return piece.data;
            var comp = piece as CompoundDataPiece;
            
            
            foreach (var v in comp.DataList)
            {
                if (v.key == "__type") continue;    
            }
            throw new NotImplementedException();
        }

  
    }

    

    public enum DataType
    {
        NULL,
        INT32,
        INT64,
        BYTE,
        UINT32,
        UINT64,
        FLOAT,
        DOUBLE,
        VECTOR2,
        VECTOR3,
        VECTOR4,
        MATRIX4,
        QUATERNION,
        STRING,
        COMPOUND,
        ARRAY,
        BOOLEAN,
        COLOR,
        ENUM
    }
}
