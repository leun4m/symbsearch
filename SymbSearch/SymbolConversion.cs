using System;

namespace SymbSearch
{
	public class SymbolConversion
	{
		private char symbol = 'A';

		public SymbolConversion()
		{
			
		}

		public void SetSymbol(string str)
		{
			if (str.Length > 0) {
				symbol = str[0];
			}
		}


		public void SetDecimal(string str)
		{
			try {
				int val = Int32.Parse(str);
				symbol = (char)val;
			} catch {
				// conversion failed ...
			}
		}

		public void SetHexadecimal(string str)
		{
			try {
				int val = Convert.ToInt32(str, 16);
				symbol = (char)val;
			} catch {
				// conversion failed ...
			}
		}

		public void SetHtml(string str)
		{
			if (str.StartsWith("&#x")) {
				str = str.Substring(3);
				str = TrimHtmlEnd(str);
				SetHexadecimal(str);
			} else if (str.StartsWith("&#")) {
				str = str.Substring(2);
				str = TrimHtmlEnd(str);
				SetDecimal(str);
			} else if (str.StartsWith("&")) {
				str = str.Substring(1);
				str = TrimHtmlEnd(str);
				SetDecimal(str);
			} else {
				str = TrimHtmlEnd(str);
				SetDecimal(str);
			} 
		}

		private string TrimHtmlEnd(string str)
		{
			int end = str.IndexOf(";");
			if (end > 0) {
				str = str.Substring(0, end);
			}
			return str;
		}

		public SymbolPresentation ToPresentation()
		{
			return new SymbolPresentation(symbol);
		}
	}
}

