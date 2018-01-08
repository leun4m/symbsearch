using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SymbSearch
{
    public class CategoryContainer
    {
        private Dictionary<string, List<string>> data;
        private List<string> currentCategory = new List<string>();

        public CategoryContainer()
        {
            LoadCategories();    
        }

        public void LoadCategories()
        {
            using (StreamReader reader = new StreamReader(@"categories.json"))
            {
                string json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
                if (data == null) {
                    data = new Dictionary<string, List<string>>();
                }
            }
        }

        public void SaveCategories()
        {
            using (StreamWriter writer = new StreamWriter(@"categories.json"))
            {
                string json = JsonConvert.SerializeObject(data);
                writer.Write(json);
                writer.Flush();
            }
        }

        public bool AddCategory(string name)
        {
            // name is null/whitespace or already included?
            if (String.IsNullOrWhiteSpace(name) || data.ContainsKey(name)) {
                return false;
            } else {
                data.Add(name, new List<string>());
                return data.ContainsKey(name);
            }
        }

        public void RemoveCategory(string name)
        {
            data.Remove(name);
        }

        public void UpdateCurrentCategory(string name)
        {
            if (data.ContainsKey(name)) {
                if (!data.TryGetValue(name, out currentCategory)) {
                    currentCategory = new List<string>();
                }
            }
        }

        public List<string> GetCategoryNames()
        {
            return new List<string>(data.Keys);
        }

        public bool CurrentContainsClass(string name)
        {
            return currentCategory.Contains(name);
        }

        public bool AddClassToCurrent(string name)
        {
            if (!currentCategory.Contains(name)) {
                currentCategory.Add(name);
                return true;   
            }
            return false;
        }

        public bool RemoveClassFromCurrent(string name)
        {
            if (currentCategory.Contains(name)) {
                currentCategory.Remove(name);
                return true;
            }
            return false;
        }
    }
}

