using System;

namespace SymbSearch
{
	public class SymbolPresentation
	{
		public enum SymbRep {
			Symbol,
			Decimal,
			Hexadecimal,
			HtmlCode
		};

		private SymbRep representation = SymbRep.Symbol;
		private char symbol = 'A';

		public SymbolPresentation()
		{
		}

		public SymbolPresentation(char symb)
		{
			symbol = symb;
		}

		public void SetSymbol(char symb)
		{
			symbol = symb;
		}

		public void SetRepresentation(SymbRep rep)
		{
			representation = rep;
		}

		public string GetPresentation()
		{
			switch (representation) {
			case SymbRep.Symbol:
				return GetSymbol();
			case SymbRep.Decimal:
				return GetDecimal();
			case SymbRep.Hexadecimal:
				return GetHexadecimal();
			case SymbRep.HtmlCode:
				return GetHtmlCode();
			default:
				return GetSymbol();
			}
		}

		public string GetSymbol()
		{
			return symbol.ToString();
		}

		public string GetDecimal()
		{
			return String.Format("{0:D4}", (int)symbol);
		}

		public string GetHexadecimal()
		{
			return "0x" + String.Format("{0:X4}", (int)symbol);
		}

		public string GetHtmlCode()
		{
			return "&#x" + String.Format("{0:X4}", (int)symbol) + ";";
		}
	}
}

