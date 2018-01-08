using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SymbSearch
{
    public class JsonParser
    {
        public class Entry
        {
            public string s; // hexadecimal value (unicode symbol)
            public string n; // name of symbol
            public string c; // category name
        }

        public class DataStruct
        {
            public List<Entry> symbols;
        }

        private DataStruct data;
        private HashSet<string> categories = new HashSet<string>();

        public JsonParser()
        {
            LoadSymbols();
        }

        public void LoadSymbols()
        {
            using (StreamReader reader = new StreamReader(@"symbols.json"))
            {
                string json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<DataStruct>(json);
            }

            data.symbols.RemoveAll(x => String.IsNullOrWhiteSpace(x.c));
            data.symbols.RemoveAll(x => String.IsNullOrWhiteSpace(x.n));

            //categories.Add("all");
            foreach (Entry entry in data.symbols)
            {
                categories.Add(entry.c);
            }
        }

        public HashSet<string> GetCategories()
        {
            return categories;
        }

        public Symb EntryToSymb(Entry entry) {
            char s = (char)Convert.ToInt16(entry.s, 16);
            return new Symb(entry.n, s, entry.c);
        }

        public List<Symb> FilterList(String filtertext, String categoryName, bool caseSensitive)
        {
            List<Symb> newList = new List<Symb>();
            if (!caseSensitive)
            {
                filtertext = filtertext.ToLower();
            }
            string[] substrings = filtertext.Split(null); // split whitespace

            foreach (Entry entry in data.symbols)
            {
                if (categoryName != "all" && categoryName != entry.c)
                {
                    continue;
                }

                string name = entry.n;
                if (!caseSensitive)
                {
                    name = name.ToLower();
                }

                bool valid = true;
                foreach (string str in substrings)
                {
                    if (!name.Contains(str))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    newList.Add(EntryToSymb(entry));
                }
            }
            return newList;
        }
    }
}
