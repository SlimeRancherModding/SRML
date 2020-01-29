using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    public static class DictionaryUtils
    {
        public static Dictionary<V, K> Invert<K, V>(this Dictionary<K, V> dict)
        {
            var newDict = new Dictionary<V, K>();
            foreach (var item in dict) newDict[item.Value] = item.Key;
            return newDict;
        }

        public static K GetKeyOfValue<K, V>(this Dictionary<K, V> dict, V val) => dict.First(x => x.Value.Equals(val)).Key;
    }

