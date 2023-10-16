using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SRML.Utils;

namespace SRML.SR.SaveSystem.Data.Partial
{
    internal class PartialList<T> : PartialData<IList<T>>, IDictionaryProvider
    {
        private Predicate<T> hoistCondition;
        private Func<T> emptyValueCreator;
        private SerializerPair<T> serializer;

        Dictionary<int,T> hoistedValues = new Dictionary<int, T>();

        public PartialList(Predicate<T> hoistCondition, SerializerPair<T> serializer, Func<T> emptyValueCreator=null)
        {
            this.hoistCondition = hoistCondition;
            if (emptyValueCreator == null) emptyValueCreator = () => default(T);
            this.emptyValueCreator = emptyValueCreator;
            this.serializer = serializer;
        }

        public IDictionary InternalDictionary => hoistedValues;

        public override void Pull(IList<T> data)
        {
            hoistedValues.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                if (hoistCondition(data[i]))
                {
                    hoistedValues.Add(i,data[i]);
                    data[i] = emptyValueCreator();
                }
            }
        }

        public override void Push(IList<T> data)
        {
            foreach (var pair in hoistedValues)
            {
                data[pair.Key] = pair.Value;
            }
        }

        public override void Read(BinaryReader reader)
        {
            BinaryUtils.ReadDictionary(reader,hoistedValues,(x)=>x.ReadInt32(),(x)=> (T)serializer.Deserialize(x));
        }

        public override void Write(BinaryWriter writer)
        {
            BinaryUtils.WriteDictionary(writer,hoistedValues,(x,y)=>x.Write(y),(x,y)=>serializer.Serialize(x,y));
        }
    }
}
