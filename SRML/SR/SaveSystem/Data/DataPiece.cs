using SRML.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data
{
    public class CompoundDataPiece : DataPiece
    {
        public CompoundDataPiece() { }

        public List<DataPiece> dataList
        {
            get { return data as List<DataPiece>; }
        }

        public DataPiece this[string index]
        {
            get { return dataList.First((x) => x.key == index); }
        }
    }

    public class DataPiece
    {
        public DataType typeId;
        public string key;
        public object data;

        public static Dictionary<DataType, SerializerPair> serializerPairs = new Dictionary<DataType, SerializerPair>()
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
            {DataType.COMPOUND,new SerializerPair<List<DataPiece>>((x, y) =>
            {
                x.Write(y.Count);
                foreach (var v in y)
                {
                    Serialize(x, v);
                }
            },
            (x) =>
            {
                var newList = new List<DataPiece>();
                int count = x.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    newList.Add(Deserialize(x));
                }

                return newList;

            }
            ) }
        };

        public static Dictionary<Type, DataType> typeToDataType = new Dictionary<Type, DataType>();

        static DataPiece()
        {
            foreach (var v in serializerPairs)
            {
                typeToDataType.Add(v.Value.GetSerializedType(), v.Key);
            }
        }

        public static DataPiece GetNewDataPiece(DataType forType)
        {
            if (forType == DataType.COMPOUND) return new CompoundDataPiece();
            return new DataPiece();

        }

        public static DataPiece Deserialize(BinaryReader reader)
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

        public static void Serialize(BinaryWriter writer, DataPiece piece)
        {

            if (!serializerPairs.TryGetValue(piece.typeId, out var serializer))
                throw new Exception($"Unrecognized Data Type {piece.typeId}");
            writer.Write((int)piece.typeId);
            writer.Write(piece.key);
            serializer.Serialize(writer, piece.data);
        }

        public void Serialize(BinaryWriter writer)
        {
            Serialize(writer, this);
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
        COMPOUND
    }
}
