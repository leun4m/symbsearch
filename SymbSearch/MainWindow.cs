using System;
using System.Collections.Generic;
using Gtk;
using SymbSearch;

public partial class MainWindow: Gtk.Window
{
	private Parser parser = new Parser();
	private Gtk.ListStore tvListStore;
	private Gtk.ListStore tvConversionListStore;
	private bool caseSensitive = false;
	private char currentSymbol = 'A';
	private string symbolFont = "Arial 20";
	private SymbolPresentation symbPresentation = new SymbolPresentation();
	private SymbolConversion conversion = new SymbolConversion();
	private TreeIter tvIter;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();
		UpdateFont();
		UpdateCategories();
		CreateTree();
		CreateConversionTree();
		cbCategory.Active = 0;
		cbPresentation.Active = 0;
		cbConversionUnit.Active = 0;
		UpdateResult();
		eSearchBox.GrabFocus();
	}

	private void UpdateCategories()
	{
		List<string> categories = parser.GetCategories();
		CellRendererText cell = new CellRendererText();
		cbCategory.AddAttribute(cell, "text", 0);
		ListStore listStore = new ListStore(typeof(string));
		cbCategory.Model = listStore;
		foreach (string cat in categories) {
			listStore.AppendValues(cat);
		}
	}

	private void CreateTree()
	{
		CreateTreeColumn(ref tvResult, "Name", 0);
		CreateTreeColumn(ref tvResult, "Symbol", 1);
		CreateTreeColumn(ref tvResult, "Category", 2);
		tvListStore = new Gtk.ListStore(typeof(string), typeof(string), typeof(string));
		tvResult.Model = tvListStore;
	}

	private void CreateConversionTree()
	{
		CreateTreeColumn(ref tvConversionResult, "Unit", 0);
		CreateTreeColumn(ref tvConversionResult, "Value", 1);
		tvConversionListStore = new Gtk.ListStore(typeof(string), typeof(string));
		tvConversionResult.Model = tvConversionListStore;
	}

	private void CreateTreeColumn(ref NodeView tree, String name, int index)
	{
		TreeViewColumn col = new Gtk.TreeViewColumn();
		col.Title = name;
		tree.AppendColumn(col);
		CellRendererText colCell = new Gtk.CellRendererText();
		col.PackStart(colCell, true);
		col.AddAttribute(colCell, "text", index);
	}

	private void UpdateResult()
	{
		tvListStore.Clear();
		List<Symb> list = parser.FilterList(eSearchBox.Text.ToString(), cbCategory.ActiveText, caseSensitive);
		foreach (Symb symb in list) {
			tvListStore.AppendValues(symb.name, symb.sign.ToString(), symb.cat);
		}
		lSymbolCounter.Text = "Entries: " + Convert.ToString(list.Count);
		if (tvListStore.GetIterFirst(out tvIter)) {
			tvResult.Selection.SelectIter(tvIter);
			UpdateSymbol();
		}
	}

	private void UpdateSymbol()
	{
		currentSymbol = tvListStore.GetValue(tvIter, 1).ToString()[0];
		symbPresentation.SetSymbol(currentSymbol);
		lSymbol.Text = symbPresentation.GetPresentation();
	}

	private void UpdateFont()
	{
		lFontExample.ModifyFont(Pango.FontDescription.FromString(symbolFont));
		lSymbol.ModifyFont(Pango.FontDescription.FromString(symbolFont));
		lConversionSymbol.ModifyFont(Pango.FontDescription.FromString(symbolFont));
	}

	private void ConvertInput()
	{
		if (String.IsNullOrWhiteSpace(eConversionInput.Text)) {
			return;
		}

		if (cbConversionUnit.ActiveText == "Symbol") {
			conversion.SetSymbol(eConversionInput.Text);
		} else if (cbConversionUnit.ActiveText == "Decimal") {
			conversion.SetDecimal(eConversionInput.Text);
		} else if (cbConversionUnit.ActiveText == "Hexadecimal") {
			conversion.SetHexadecimal(eConversionInput.Text);
		} else if (cbConversionUnit.ActiveText == "HTML Code") {
			conversion.SetHtml(eConversionInput.Text);
		} else {
			conversion.SetSymbol(eConversionInput.Text);
		}  
		lConversionSymbol.Text = conversion.ToPresentation().GetSymbol();
		UpdateConversionTable();
	}

	private void UpdateConversionTable()
	{
		SymbolPresentation pres = conversion.ToPresentation();
		tvConversionListStore.Clear();
		tvConversionListStore.AppendValues("Decimal", pres.GetDecimal());
		tvConversionListStore.AppendValues("Hexadecimal", pres.GetHexadecimal());
		tvConversionListStore.AppendValues("HTML Code", pres.GetHtmlCode());
		tvConversionListStore.AppendValues("Symbol", pres.GetSymbol());
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void OnESearchBoxChanged(object sender, EventArgs e)
	{
		UpdateResult();
	}

	protected void OnCbCategoryChanged(object sender, EventArgs e)
	{
		UpdateResult();
	}

	private void RowToClipboard(RowActivatedArgs args)
	{
		Gtk.Clipboard clipboard = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
		clipboard.Text = symbPresentation.GetPresentation();
	}

	protected void OnTvResultCursorChanged(object sender, EventArgs e)
	{
		TreeSelection selection = (sender as TreeView).Selection;
		TreeModel model;
		if (selection.GetSelected(out model, out tvIter)) {
			UpdateSymbol();
			eConversionInput.Text = currentSymbol.ToString();
			cbConversionUnit.Active = 3;
			ConvertInput();
		}
	}

	protected void OnTvResultRowActivated(object o, RowActivatedArgs args)
	{
		RowToClipboard(args);
	}

	protected void OnCCaseSensitiveToggled(object sender, EventArgs e)
	{
		caseSensitive = cCaseSensitive.Active;
	}

	protected void OnCbPresentationChanged(object sender, EventArgs e)
	{
		if (cbPresentation.ActiveText == "Symbol") {
			symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.Symbol);
		} else if (cbPresentation.ActiveText == "Decimal") {
			symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.Decimal);
		} else if (cbPresentation.ActiveText == "Hexadecimal") {
			symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.Hexadecimal);
		} else if (cbPresentation.ActiveText == "HTML Code") {
			symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.HtmlCode);
		} else {
			symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.Symbol);
		}  
	}

	protected void OnFbSymbolFontFontSet(object sender, EventArgs e)
	{
		symbolFont = fbSymbolFont.FontName;
		UpdateFont();
	}

	protected void OnCbConversionUnitChanged(object sender, EventArgs e)
	{
		ConvertInput();
	}

	protected void OnEConversionInputChanged(object sender, EventArgs e)
	{
		ConvertInput();
	}

	protected void OnESearchBoxKeyPressEvent(object o, KeyPressEventArgs args)
	{
		if (args.Event.Key == Gdk.Key.Down) {
			tvResult.GrabFocus();
			tvListStore.IterNext(ref tvIter);
		}
	}

	protected void OnTvResultKeyPressEvent(object o, KeyPressEventArgs args)
	{
		eSearchBox.GrabFocus();
	}
}
