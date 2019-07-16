using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using SRML.Utils;

namespace SRML.SR.SaveSystem
{
    internal class StringRegistry : Dictionary<string, Tuple<SRMod,string>>
    {
        public string Prefix { get; }

        public delegate bool ExternalIDCheckPredicate(string candidate);

        static readonly Random random = new Random();

        ExternalIDCheckPredicate IDChecker;

        internal void SetIDChecker(ExternalIDCheckPredicate pred)
        {
            IDChecker = pred;
        }
        public StringRegistry(string prefix)
        {
            Prefix = prefix;
        }

        public StringRegistry(string prefix, ExternalIDCheckPredicate pred) : this(prefix)
        {
            this.IDChecker = pred;
        }

        public IEnumerable<KeyValuePair<string, string>> NamesToAliases => this.Select(x=>new KeyValuePair<string,string>(x.Key,x.Value.Item2)).Where(x=>x.Value!=null).AsEnumerable();

        public bool Contains(string id) => ContainsKey(id) || (IDChecker?.Invoke(id) ??  false);

        public bool ConformsToPrefix(string candidate) => candidate.StartsWith(Prefix);

        public bool HasAlias(string id) => TryGetValue(id, out var tuple)&&tuple.Item2!=null;
        

        public string GetAlias(string id)
        {
            TryGetValue(id, out var alternate);
            return alternate?.Item2;
        }

        public string FromAliasAndMod(string id, SRMod mod) => FromAliasAndMod(new Tuple<SRMod, string>(mod, id));
        public string FromAliasAndMod(Tuple<SRMod,string> identifier) => this.FirstOrDefault(x => x.Value == identifier).Key;
        

        public bool RegisterID(string id, SRMod mod, string alias)
        {
            if (!ConformsToPrefix(id)) return false;
            if (Contains(id)) return false;
            Add(id, new Tuple<SRMod,string>(mod,alias));
            return true;
        }

        public string GenerateNewID()
        {
            string candidate = "";
            do
            {
                candidate = Prefix;
                for (int i = 0; i < 10; i++) candidate += random.Next(0, 10).ToString();
            } while (Contains(candidate));
            return candidate;
        }
    }
}
