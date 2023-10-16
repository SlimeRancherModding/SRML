using SRML.SR.SaveSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRML.SR.SaveSystem
{
    public abstract class ModdedGameData : IdHandler, ExtendedData.Participant
    {
        internal static readonly Dictionary<string, ModdedGameData> allData = new Dictionary<string, ModdedGameData>();
        internal static readonly Dictionary<string, Dictionary<Type, int>> amountInSave = new Dictionary<string, Dictionary<Type, int>>();

        public static Dictionary<string, T> All<T>() where T : ModdedGameData => 
            allData.Where(x => x.Value.GetType() == typeof(T)).ToDictionary(x => x.Key, y => (T)y.Value);

        internal static void IncrementInSave<T>(string id)
        {
            Type t = typeof(T);

            if (!amountInSave.ContainsKey(id))
                amountInSave.Add(id, new Dictionary<Type, int>());
            if (!amountInSave[id].ContainsKey(t))
                amountInSave[id].Add(t, 0);

            amountInSave[id][t]++;
        }

        internal static int GetInSave<T>(string id) => amountInSave.TryGetValue(id, out var dict) ? 
            (dict.TryGetValue(typeof(T), out int v) ? v : 0) : 0;

        public abstract void Init();
        public abstract void ReadData(CompoundDataPiece piece);
        public abstract void WriteData(CompoundDataPiece piece);
    }
}
