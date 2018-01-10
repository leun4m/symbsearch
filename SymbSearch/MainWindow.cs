using System;
using System.Reflection;
using System.Collections.Generic;
using Gtk;
using SymbSearch;

public partial class MainWindow: Gtk.Window
{
    private JsonParser parser = new JsonParser();
    private CategoryManager catManager = new CategoryManager();
	private HashSet<string> unicodeClasses;
	private Gtk.ListStore tvListStore;
	private Gtk.ListStore tvConversionListStore;
	private bool caseSensitive = false;
	private char currentSymbol = 'A';
	private string symbolFont = "Arial 20";
	private SymbolPresentation symbPresentation = new SymbolPresentation();
	private SymbolConversion conversion = new SymbolConversion();
	private TreeIter tvIter;
	private ListStore catSelectListStore;
    private ListStore categoryListStore;
	private ListStore listStoreExcluded;
	private ListStore listStoreIncluded;
	private TreeIter excludedClassIter;
	private TreeIter includedClassIter;
    private TreeIter catSelectIter;
	private string selectedExcludedClass;
	private string selectedIncludedClass;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
        Build();
        this.Title = "SymbSearch " 
            + Assembly.GetExecutingAssembly().GetName().Version.Major
            + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor;
        CreateCategories();
		CreateTree();
		CreateConversionTree();
		CreateClassTrees();
		cbCategory.Active = 0;
        cbPresentation.Active = 0;
        cbConversionUnit.Active = 0;
        UpdateCategory();
        UpdateClasses();
        UpdateResult();
        UpdateFont();
        eSearchBox.GrabFocus();
	}

	private void CreateCategories()
	{
        unicodeClasses = parser.GetCategories();
		CellRendererText cell = new CellRendererText();
		cbeCategorySelect.AddAttribute(cell, "text", 0);
        catSelectListStore = new ListStore(typeof(string));
        cbeCategorySelect.Model = catSelectListStore;

        cell = new CellRendererText();
        cbCategory.AddAttribute(cell, "text", 0);
        categoryListStore = new ListStore(typeof(string));
        cbCategory.Model = categoryListStore;
		
        foreach (string s in catManager.GetCategoryNames()) {
            catSelectListStore.AppendValues(s);
            categoryListStore.AppendValues(s);
        }
	}

	private void CreateTree()
	{
		CreateTreeColumn(ref tvResult, "Name", 0);
		CreateTreeColumn(ref tvResult, "Symbol", 1);
		CreateTreeColumn(ref tvResult, "Class", 2);
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

    private void UpdateCategory()
    {
        catManager.UpdateCurrentCategory(cbCategory.ActiveText);
    }

    private void UpdateCategoryList()
    {
        categoryListStore = new ListStore(typeof(string));
        cbCategory.Model = categoryListStore;
        foreach (string s in catManager.GetCategoryNames())
        {
            categoryListStore.AppendValues(s);
        }
        cbCategory.Active = 0;
    }

	private void UpdateResult()
	{
		tvListStore.Clear();
        List<Symb> list = parser.FilterList(eSearchBox.Text, catManager, caseSensitive);
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

    private void UpdatePresentation()
    {
        switch (cbPresentation.ActiveText)
        {
            case "Symbol":
                symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.Symbol);
                break;
            case "Decimal":
                symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.Decimal);
                break;
            case "Hexadecimal":
                symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.Hexadecimal);
                break;
            case "HTML Code":
                symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.HtmlCode);
                break;
            default:
                symbPresentation.SetRepresentation(SymbolPresentation.SymbRep.Symbol);
                break;
        }
    }

    private void RowToClipboard()
    {
        Gtk.Clipboard clipboard = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
        clipboard.Text = symbPresentation.GetPresentation();
    }

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
        catManager.SaveCategories();
		Application.Quit();
		a.RetVal = true;
	}

	protected void OnESearchBoxChanged(object sender, EventArgs e)
	{
		UpdateResult();
	}

	protected void OnCbCategoryChanged(object sender, EventArgs e)
	{
        UpdateCategory();
		UpdateResult();
	}

	protected void OnTvResultCursorChanged(object sender, EventArgs e)
	{
		TreeSelection selection = (sender as TreeView).Selection;
        if (selection.GetSelected(out TreeModel model, out tvIter))
        {
            UpdateSymbol();
            eConversionInput.Text = currentSymbol.ToString();
            cbConversionUnit.Active = 3;
            ConvertInput();
        }
    }

	protected void OnTvResultRowActivated(object o, RowActivatedArgs args)
	{
		RowToClipboard();
	}

	protected void OnCCaseSensitiveToggled(object sender, EventArgs e)
	{
		caseSensitive = cCaseSensitive.Active;
	}

	protected void OnCbPresentationChanged(object sender, EventArgs e)
	{
        UpdatePresentation();
        UpdateSymbol();
	}

    protected void OnFbSymbolFontFontSet(object sender, EventArgs e)
    {
        symbolFont = fbSymbolFont.FontName;
        UpdateFont();
        UpdateSymbol();
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

    protected void OnNbMainSwitchPage(object o, SwitchPageArgs args)
    {
        UpdateCategoryList();
        UpdateResult();
    }
}
