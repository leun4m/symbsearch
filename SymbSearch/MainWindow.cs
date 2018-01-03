using System;
using System.Collections.Generic;
using Gtk;
using SymbSearch;

public partial class MainWindow: Gtk.Window
{
	private Parser parser = new Parser();
	private Gtk.ListStore tvListStore;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();
		ChangeLabelSize();
		UpdateCategories();
		CreateTree();
		cbCategory.Active = 0;
		UpdateResult();
	}

	private void UpdateCategories()
	{
		List<string> categories = parser.GetCategories();
		CellRendererText cell = new CellRendererText();
		//cbCategory.PackStart(cell, true);
		cbCategory.AddAttribute(cell, "text", 0);
		ListStore listStore = new ListStore(typeof(string));
		cbCategory.Model = listStore;
		foreach (string cat in categories) {
			listStore.AppendValues(cat);
		}
	}

	private void CreateTree()
	{
		TreeViewColumn colName = new Gtk.TreeViewColumn();
		CreateTreeColumn("Name", 0);
		CreateTreeColumn("Sign", 1);
		CreateTreeColumn("Category", 2);
		tvListStore = new Gtk.ListStore(typeof(string), typeof(string), typeof(string));
		tvResult.Model = tvListStore;
	}

	private void CreateTreeColumn(String name, int index)
	{
		TreeViewColumn col = new Gtk.TreeViewColumn();
		col.Title = name;
		tvResult.AppendColumn(col);
		CellRendererText colCell = new Gtk.CellRendererText();
		col.PackStart(colCell, true);
		col.AddAttribute(colCell, "text", index);
	}

	private void UpdateResult()
	{
		tvListStore.Clear();
		//eSearchBox.Text 
		List<Symb> list = parser.FilterList(eSearchBox.Text.ToString(), cbCategory.ActiveText);
		for (int i = 0; i < list.Count; i++) {
			tvListStore.AppendValues(list[i].name, list[i].sign.ToString(), list[i].cat);
		}
	}

	private void ChangeLabelSize()
	{
		lSymbol.ModifyFont(Pango.FontDescription.FromString("Arial 20"));
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
		clipboard.Text = GetSelectedSign(args).ToString();
	}

	private object GetSelectedSign(RowActivatedArgs args)
	{
		TreeIter iter;
		tvListStore.GetIter(out iter, args.Path);
		return tvListStore.GetValue(iter, 1);
	}

	protected void OnTvResultCursorChanged(object sender, EventArgs e)
	{
		TreeSelection selection = (sender as TreeView).Selection;
		TreeModel model;
		TreeIter iter;
		if (selection.GetSelected(out model, out iter)) {
			lSymbol.Text = model.GetValue(iter, 1).ToString();
		}
	}

	protected void OnTvResultRowActivated(object o, RowActivatedArgs args)
	{
		RowToClipboard(args);
	}
}
