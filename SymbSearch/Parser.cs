using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SymbSearch
{
	public class Parser
	{
		/*[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);*/

		const int MOD_ALT = 0x0001;
		const int MOD_CONTROL = 0x0002;
		const int MOD_SHIFT = 0x0004;
		const int WM_HOTKEY = 0x0312;

		private List<Symb> symbols = new List<Symb>();
		private short databaseVersion;

		public Parser()
		{
			symbols = MakeList();
		}

		/// <summary>
		/// Filters symbols by category and search
		/// </summary>
		public List<Symb> FilterList(String filtertext, String category)
		{
			List<Symb> newList = new List<Symb>();

			for (int i = 0; i < symbols.Count; i++)
			{
				if (symbols[i].name.ToLower().Contains(filtertext.ToLower()))
				{
					if (category == "all" || category == symbols[i].cat)
					{
						newList.Add(symbols[i]);
					}
				}
			}
			return newList;
		}
		#region List of Symb
		/// <summary>
		/// Reads and parses symbols.xml
		/// </summary>
		/// <returns>Returns list of Symb by symbols.xml</returns>
		private List<Symb> MakeList()
		{
			XmlReader reader = XmlReader.Create(@"symbols.xml");
			List<Symb> list = new List<Symb>();
			Symb symb = null;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == "symb")
				{
					symb = new Symb();
					list.Add(symb);
					if (reader.HasAttributes)
					{
						while (reader.MoveToNextAttribute())
						{
							switch (reader.Name)
							{
							case "name":
								symb.name = reader.Value;
								break;
							case "sign":
								symb.sign = reader.Value.ToCharArray()[0];
								break;
							case "cat":
								symb.cat = reader.Value;
								break;
							}
						}
					}
				}
				else if (reader.Name == "symbols" && reader.HasAttributes)
				{
					reader.MoveToNextAttribute();
					if (reader.Name == "version")
					{
						databaseVersion = Convert.ToInt16(reader.Value);
					}
				}
			}
			return list;
		}
		#endregion
		#region Cats of Symb
		/// <summary>
		/// Returns the categories of symbols
		/// </summary>
		/// <returns></returns>
		public List<string> GetCategories()
		{
			List<string> cats = new List<string>();
			cats.Add("all");
			foreach (Symb temp in symbols)
			{
				cats.Add(temp.cat);
			}
			return RemoveDoubles(cats);
		}
		/// <summary>
		/// Removes doubles of the given list
		/// </summary>
		private List<string> RemoveDoubles(List<string> l)
		{
			return l.Distinct().ToList();
		}
		#endregion

	}
}
