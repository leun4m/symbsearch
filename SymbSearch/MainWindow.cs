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
	private ListStore categoryListStore;
	private ListStore listStoreExcluded;
	private ListStore listStoreIncluded;
	private TreeIter excludedClassIter;
	private TreeIter includedClassIter;
	private string selectedExcludedClass;
	private string selectedIncludedClass;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
        Build();
        this.Title = "SymbSearch " 
            + Assembly.GetExecutingAssembly().GetName().Version.Major
            + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor;
        CreateClasses();
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

	private void CreateClasses()
	{
        unicodeClasses = parser.GetCategories();
		CellRendererText cell = new CellRendererText();
		cbCategory.AddAttribute(cell, "text", 0);
        foreach (string s in catManager.GetCategoryNames()) {
            cbCategory.AppendText(s);
		}
	}

	private void CreateCategories()
	{
		CellRendererText cell = new CellRendererText();
		cbeCategorySelect.AddAttribute(cell, "text", 0);
		categoryListStore = new ListStore(typeof(string));
		cbeCategorySelect.Model = categoryListStore;
        foreach (string s in catManager.GetCategoryNames()) {
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

	private void CreateClassTrees()
	{
		CreateTreeColumn(ref nvClassesExcluded, "Class", 0);
		listStoreExcluded = new Gtk.ListStore(typeof(string));
		nvClassesExcluded.Model = listStoreExcluded;

		CreateTreeColumn(ref nvClassesIncluded, "Class", 0);
		listStoreIncluded = new Gtk.ListStore(typeof(string));
		nvClassesIncluded.Model = listStoreIncluded;
	}

    private void UpdateCategory()
    {
        catManager.UpdateCurrentCategory(cbCategory.ActiveText);
    }

    private void UpdateClasses()
	{
		listStoreIncluded.Clear();
		listStoreExcluded.Clear();

        catManager.UpdateCurrentCategory(cbeCategorySelect.ActiveText);
		foreach (string s in unicodeClasses) {
            if (catManager.CurrentContainsClass(s)) {
				listStoreIncluded.AppendValues(s);
			} else {
				listStoreExcluded.AppendValues(s);
			}
		}
        UpdateClassLabels();
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

	private void UpdateClassLabels()
	{
		lClassesExcluded.Text = "Available: " + listStoreExcluded.IterNChildren().ToString();
		lClassesIncluded.Text = "Included: " + listStoreIncluded.IterNChildren().ToString();
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

	private void RowToClipboard()
	{
		Gtk.Clipboard clipboard = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
		clipboard.Text = symbPresentation.GetPresentation();
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

        UpdateSymbol();
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

	protected void OnBCreateCategoryClicked(object sender, EventArgs e)
	{
		string categoryName = cbeCategorySelect.ActiveText.Trim();
        if (catManager.AddCategory(categoryName)) {
            categoryListStore.AppendValues(categoryName);
            //cbeCategorySelect.AppendText(categoryName);
            int idx = categoryListStore.IterNChildren();
            cbeCategorySelect.Active = idx;
            UpdateClassLabels();
		}
	}

	protected void OnBRemoveCategoryClicked(object sender, EventArgs e)
	{
		string categoryName = cbeCategorySelect.ActiveText;
		int idx = cbeCategorySelect.Active;
		cbeCategorySelect.RemoveText(idx);
        catManager.RemoveCategory(categoryName);
		cbeCategorySelect.Active = 0;
        UpdateClassLabels();
	}

	protected void OnCbeCategorySelectChanged(object sender, EventArgs e)
	{
		UpdateClasses();
	}

	protected void OnNvClassesExcludedCursorChanged(object sender, EventArgs e)
	{
		TreeSelection selection = (sender as TreeView).Selection;
        if (selection.GetSelected(out TreeModel model, out excludedClassIter))
        {
            selectedExcludedClass = listStoreExcluded.GetValue(excludedClassIter, 0).ToString();
        }
    }

	protected void OnNvClassesIncludedCursorChanged(object sender, EventArgs e)
	{
		TreeSelection selection = (sender as TreeView).Selection;
        if (selection.GetSelected(out TreeModel model, out includedClassIter))
        {
            selectedIncludedClass = listStoreIncluded.GetValue(includedClassIter, 0).ToString();
        }
    }

	private void SelectNextClassIncluded()
	{
		/* The idea was to select the item after the removed one
			   however this is no working, the iterator gets invalid after remove */
		/*			
		listStoreIncluded.IterNext(ref includedClassIter);*/
		nvClassesIncluded.GrabFocus();
		listStoreIncluded.GetIterFirst(out includedClassIter);
		if (listStoreIncluded.IterIsValid(includedClassIter)) {
			nvClassesIncluded.Selection.SelectIter(includedClassIter);
			// update selected class
			selectedIncludedClass = listStoreIncluded.GetValue(includedClassIter, 0).ToString();
		}
	}

	private void SelectNextClassExcluded()
	{
		/* The idea was to select the item after the removed one
			   however this is no working, the iterator gets invalid after remove */

		/*listStoreExcluded.IterNext(ref excludedClassIter);*/
		nvClassesExcluded.GrabFocus();
		listStoreExcluded.GetIterFirst(out excludedClassIter);
		if (listStoreExcluded.IterIsValid(excludedClassIter)) {
			nvClassesExcluded.Selection.SelectIter(excludedClassIter);
			// update selected class
			selectedExcludedClass = listStoreExcluded.GetValue(excludedClassIter, 0).ToString();
		}
	}

	protected void OnBExcludeSymbolClassClicked(object sender, EventArgs e)
	{
		if (String.IsNullOrWhiteSpace(selectedIncludedClass)
			|| !listStoreIncluded.IterIsValid(includedClassIter)) {
			return;
		}

        if (catManager.RemoveClassFromCurrent(selectedIncludedClass)) {
			// Append selected class from includedClass to excluded classes
			listStoreExcluded.AppendValues(selectedIncludedClass);
            // Remove selected class from includedClass
			listStoreIncluded.Remove(ref includedClassIter);

			SelectNextClassIncluded();
			UpdateClassLabels();
		}
	}

	protected void OnBIncludeSymbolClassClicked(object sender, EventArgs e)
	{
		if (String.IsNullOrWhiteSpace(selectedExcludedClass) 
			|| !listStoreExcluded.IterIsValid(excludedClassIter)) {
			return;
		}

        if (catManager.AddClassToCurrent(selectedExcludedClass)) {
			// Append selected class from excludedClass to included classes
			listStoreIncluded.AppendValues(selectedExcludedClass);
			// Remove selected class from excludedClass
			listStoreExcluded.Remove(ref excludedClassIter);

			SelectNextClassExcluded();
			UpdateClassLabels();
		}
	}
}
