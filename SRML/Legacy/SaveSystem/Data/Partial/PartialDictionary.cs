using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SRML.Utils;
using UnityEngine;

namespace SRML.SR.SaveSystem.Data.Partial
{
    internal class PartialDictionary<K,V> : PartialData<IDictionary<K,V>>, IDictionaryProvider
    {
        Predicate<KeyValuePair<K, V>> hoistPredicate;
        public readonly Dictionary<K, V> hoistedValues = new Dictionary<K, V>();

        private Func<KeyValuePair<K,V>, KeyValuePair<K, V>?> hoistFiller;

        private SerializerPair<K> keySerializer;
        private SerializerPair<V> valueSerializer;

  

        public PartialDictionary(Predicate<KeyValuePair<K,V>> hoistPredicate,SerializerPair<K> keySerializer,SerializerPair<V> valueSerializer,Func<KeyValuePair<K,V>,KeyValuePair<K,V>?> filler=null)
        {
            this.hoistPredicate = hoistPredicate;
            this.keySerializer = keySerializer;
            this.valueSerializer = valueSerializer;
            if (filler == null) filler = (x) => null;
            this.hoistFiller = filler;
        }

        public IDictionary InternalDictionary => hoistedValues;

        public override void Pull(IDictionary<K, V> data)
        {
            hoistedValues.Clear();
            foreach (var pair in data)
            {
                if (hoistPredicate(pair))
                {
                    hoistedValues.Add(pair.Key,pair.Value);
                }
            }

            foreach (var pair in hoistedValues)
            {
                data.Remove(pair.Key);
                var newkey = hoistFiller(pair);
                if(newkey.HasValue) data.Add(newkey.Value.Key,newkey.Value.Value);
            }
        }

        public override void Push(IDictionary<K, V> data)
        {

            foreach(var v in hoistedValues) data[v.Key]=v.Value;
        }

        public override void Read(BinaryReader reader)
        {
            BinaryUtils.ReadDictionary(reader,hoistedValues,keySerializer.DeserializeGeneric,valueSerializer.DeserializeGeneric);
        }

        public override void Write(BinaryWriter writer)
        {
            BinaryUtils.WriteDictionary(writer,hoistedValues,keySerializer.SerializeGeneric,valueSerializer.SerializeGeneric);
        }
    }
}
