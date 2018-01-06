using System;
using System.Collections.Generic;

namespace SymbSearch
{
	public class Category
	{
		private HashSet<string> classes = new HashSet<string>();
		private string categoryName;

		public Category(string name)
		{
			categoryName = name;
		}

		public void AddClass(string name)
		{
			classes.Add(name);
		}

		public void RemoveClass(string name)
		{
			classes.Remove(name);
		}

		public bool Contains(string name)
		{
			return classes.Contains(name);
		}
	}
}

