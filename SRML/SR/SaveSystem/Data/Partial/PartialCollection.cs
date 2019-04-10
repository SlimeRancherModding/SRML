using SRML.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRML.SR.SaveSystem.Data.Partial
{
    internal class PartialCollection<T> : PartialData<ICollection<T>>, IListProvider
    {
        List<T> hoistedValues = new List<T>();
        private Predicate<T> hoistCondition;
        private SerializerPair<T> serializer;

        public PartialCollection(Predicate<T> hoistCondition, SerializerPair<T> serializer)
        {
            this.hoistCondition = hoistCondition;
            this.serializer = serializer;
        }

        public IList InternalList => hoistedValues;

        public override void Pull(ICollection<T> data)
        {
            hoistedValues.Clear();

            foreach (var v in data)
            {
                if(hoistCondition(v)) hoistedValues.Add(v);
            }

            foreach (var v in hoistedValues)
            {
                data.Remove(v);
            }
        }

        public override void Push(ICollection<T> data)
        {
            foreach(var v in hoistedValues) data.Add(v);
        }

        public override void Read(BinaryReader reader)
        {
            BinaryUtils.ReadList(reader,hoistedValues,(x)=>(T)serializer.Deserialize(x));
        }

        public override void Write(BinaryWriter writer)
        {
            BinaryUtils.WriteList(writer,hoistedValues,(x,y)=>serializer.Serialize(x,y));
        }

    }
}
