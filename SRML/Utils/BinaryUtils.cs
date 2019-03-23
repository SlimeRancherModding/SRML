using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
namespace SRML.Utils
{
    public static class BinaryUtils
    {

        public static void SwapPattern(byte[] bytes, byte[] pattern, byte[] replacement)
        {
            if (pattern.Length != replacement.Length) throw new Exception("Mismatched pattern and replacement length");

            for (int i = 0; i < bytes.Length - pattern.Length; i++)
            {
                bool matches = true;
                for (int x = 0; x < pattern.Length; x++)
                {
                    var bigIndex = i + x;
                    matches = bytes[bigIndex] == pattern[x];
                }

                if (!matches) continue;

                for (int x = 0; x < replacement.Length; x++)
                {
                    var bigIndex = i + x;
                    bytes[bigIndex] = replacement[x];
                }
            }

        }

        public static void WriteMeshWithBones(BinaryWriter writer, SkinnedMeshRenderer rend)
        {
            WriteArray(writer, rend.bones, (x, y) => WriteTransform(x, (Transform)y));
            WriteMesh(writer, rend.sharedMesh);
        }

        public static void ReadMeshWithBones(BinaryReader reader, SkinnedMeshRenderer rend)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ReadTransform(reader,rend.bones[i]);
            }
            if (!rend.sharedMesh) rend.sharedMesh = new Mesh();
            ReadMesh(reader, rend.sharedMesh);
        }
        public static void WriteMesh(BinaryWriter writer, Mesh mesh)
        {
            WriteArray(writer,mesh.vertices, (x, y) => WriteVector3(x, (Vector3) y));
            WriteArray(writer,mesh.triangles,(x,y)=>x.Write((int)y));
            WriteArray(writer,mesh.normals,(x,y)=>WriteVector3(x,(Vector3)y));
            WriteArray(writer,mesh.colors, (x, y) => WriteColor(x, (Color)y));
            WriteArray(writer,mesh.uv, (x, y) => WriteVector2(x, (Vector2) y));
            WriteArray(writer,mesh.tangents,(x,y)=>WriteVector4(x,(Vector4)y));
            WriteArray(writer,mesh.bindposes,(x,y)=>WriteMatrix4(x,(Matrix4x4)y));
            WriteArray(writer,mesh.boneWeights,(x,y)=>WriteBoneWeight(x,(BoneWeight)y));

            
        }

        public static void ReadMesh(BinaryReader reader, Mesh mesh)
        {
            mesh.vertices = ReadArray(reader, ReadVector3);
            mesh.triangles = ReadArray(reader, (x) => x.ReadInt32());
            mesh.normals = ReadArray(reader, ReadVector3);
            mesh.colors = ReadArray(reader, ReadColor);
            mesh.uv = ReadArray(reader, ReadVector2);
            mesh.tangents = ReadArray(reader, ReadVector4);
            mesh.bindposes = ReadArray(reader, ReadMatrix4);
            mesh.boneWeights = ReadArray(reader, ReadBoneWeight);
        }

        public static void WriteBoneWeight(BinaryWriter writer, BoneWeight weight)
        {
            writer.Write(weight.boneIndex0);
            writer.Write(weight.boneIndex1);
            writer.Write(weight.boneIndex2);
            writer.Write(weight.boneIndex3);
            writer.Write(weight.weight0);
            writer.Write(weight.weight1);
            writer.Write(weight.weight2);
            writer.Write(weight.weight3);
        }

        public static BoneWeight ReadBoneWeight(BinaryReader reader)
        {
            var weight = new BoneWeight();

            weight.boneIndex0 = reader.ReadInt32();
            weight.boneIndex1 = reader.ReadInt32();
            weight.boneIndex2 = reader.ReadInt32();
            weight.boneIndex3 = reader.ReadInt32();

            weight.weight0 = reader.ReadSingle();
            weight.weight1 = reader.ReadSingle();
            weight.weight2 = reader.ReadSingle();
            weight.weight3 = reader.ReadSingle();

            return weight;
        }

        public static void WriteColor(BinaryWriter writer, Color color)
        {
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }

        public static Color ReadColor(BinaryReader reader)
        {
            return new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
        public static void WriteTransform(BinaryWriter writer,Transform transform)
        {
            WriteVector3(writer, transform.localScale);
            WriteVector3(writer, transform.position);
            WriteQuaternion(writer, transform.rotation);
        }

        public static void ReadTransform(BinaryReader reader,Transform trans)
        {
            trans.localScale = ReadVector3(reader);
            trans.SetPositionAndRotation(ReadVector3(reader),ReadQuaternion(reader));
        }

        public static void WriteQuaternion(BinaryWriter writer, Quaternion quat)
        {
            writer.Write(quat.x);
            writer.Write(quat.y);
            writer.Write(quat.z);
            writer.Write(quat.w);
        }

        public static Quaternion ReadQuaternion(BinaryReader reader)
        {
            return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static void WriteVector3(BinaryWriter writer, Vector3 vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }
        public static Vector3 ReadVector3(BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());
        }

        public static void WriteVector2(BinaryWriter writer, Vector2 vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
        }

        public static Vector2 ReadVector2(BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public static void WriteVector4(BinaryWriter writer, Vector4 vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
            writer.Write(vec.w);
        }

        public static Vector4 ReadVector4(BinaryReader reader)
        {
            return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),reader.ReadSingle());
        }

        public static void WriteMatrix4(BinaryWriter writer, Matrix4x4 matrix)
        {
            for (int i = 0; i < 4; i++)
            {
                WriteVector4(writer, matrix.GetColumn(i));
            }
            
        }

        public static Matrix4x4 ReadMatrix4(BinaryReader reader)
        {
            return new Matrix4x4(ReadVector4(reader), ReadVector4(reader), ReadVector4(reader), ReadVector4(reader));
        }

        public static void WriteArray(BinaryWriter writer, Array array, Action<BinaryWriter,object> writeAction)
        {
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                writeAction(writer, array.GetValue(i));
            }
        }

        public static T[] ReadArray<T>(BinaryReader reader, Func<BinaryReader, T> readAction)
        {
            int length = reader.ReadInt32();
            T[] output = new T[length];
            for (int i = 0; i < length; i++)
            {
                
                output[i] = readAction(reader);
            }

            return output;
        }
    }
}
