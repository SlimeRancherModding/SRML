using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data
{
    public class CompoundDataPiece : DataPiece
    {
        public CompoundDataPiece(string key)
        {
            this.typeId = DataType.COMPOUND;
            this.data = new HashSet<DataPiece>();
            this.key = key;
        }

        internal CompoundDataPiece() { }

        public HashSet<DataPiece> DataList
        {
            get { return data as HashSet<DataPiece>; }
        }

        static CompoundDataPiece()
        {
            EnumTranslator.RegisterEnumFixer<CompoundDataPiece>((translator, mode, piece) =>
            {
                foreach(var v in piece.DataList)
                {
                    translator.FixEnumValues(mode, v);
                }
            });
        }

        public override void SetEnumTranslator(EnumTranslator.EnumFixerDelegate del)
        {
            return;
        }

        private Dictionary<string, DataPiece> _cache = new Dictionary<string, DataPiece>();

        DataPiece GetCachedPiece(string key)
        {
            if (!_cache.TryGetValue(key, out var piece)||piece==null)
            {
                var cachedPiece = DataList.FirstOrDefault((x) => key == x.key);
                if (cachedPiece != null) _cache[key] = piece;
                return cachedPiece;
            }
            else
            {
                if (!DataList.Contains(piece))
                {
                    _cache.Remove(key);
                    return null;
                }

                if (piece.key != key)
                {
                    _cache.Remove(key);
                    _cache[key] = piece;
                }
                return piece;
            }
        }

        

        public DataPiece this[string index]
        {
            get { return GetCachedPiece(index); }
        }

        public object GetValue(string key)
        {
            return this[key].GetValue();
        }

        public T GetValue<T>(string key)
        {
            return (T)GetValue(key);
        }

        public DataPiece AddPiece(DataPiece piece)
        {
            DataList.Add(piece);
            return piece;
        }

        public bool HasPiece(string key)
        {
            return this[key] != null;
        }

        public DataPiece GetPiece(string key,Type type)
        {
            if (HasPiece(key)) return this[key];
            return AddPiece(new DataPiece(key, type));
        }

        public DataPiece SetPiece(string key, object val)
        {
            var p = GetPiece(key, val.GetType());
            p.SetValue(val);

            return p;
        }

        public CompoundDataPiece GetCompoundPiece(string key)
        {
            if (this[key] is DataPiece piece)
            {
                if (!(piece is CompoundDataPiece p)) throw new Exception("Piece is not compound data piece");
                return p;
            }

            return AddPiece(new CompoundDataPiece(key)) as CompoundDataPiece;
        }

        public DataPiece GetPiece<T>(string key)
        {
            return GetPiece(key, typeof(T));
        }


        public void SetValue(string key, object value)
        {
            GetPiece(key,value.GetType()).SetValue(value);
        }

        public override String ToString()
        {
            var build = new StringBuilder();

            Stringify(build);

            return build.ToString();

            
        }
        void Stringify(StringBuilder builder, Action<String> adder = null)
        {

            if (adder == null)
            {
                adder = (x) => builder.AppendLine(x);
            }

            adder($"COMPOUND {key} = ");
            Action<String> newadder = (x) => adder("    " + x);
            foreach (var dataPiece in DataList)
            {
                if (dataPiece is CompoundDataPiece compoundDataPiece)
                {
                    compoundDataPiece.Stringify(builder, newadder);
                }
                else
                {
                    newadder(dataPiece.ToString());
                }
            }
        }
    }
}