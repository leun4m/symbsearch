using System;
using System.Reflection;
using System.Collections.Generic;
using Gtk;
using SymbSearch;

public partial class MainWindow : Gtk.Window
{
    private void CreateClassTrees()
    {
        CreateTreeColumn(ref nvClassesExcluded, "Class", 0);
        listStoreExcluded = new Gtk.ListStore(typeof(string));
        nvClassesExcluded.Model = listStoreExcluded;

        CreateTreeColumn(ref nvClassesIncluded, "Class", 0);
        listStoreIncluded = new Gtk.ListStore(typeof(string));
        nvClassesIncluded.Model = listStoreIncluded;
    }

    private void UpdateClasses()
    {
        listStoreIncluded.Clear();
        listStoreExcluded.Clear();

        catManager.UpdateCurrentCategory(cbeCategorySelect.ActiveText);
        foreach (string s in unicodeClasses)
        {
            if (catManager.CurrentContainsClass(s))
            {
                listStoreIncluded.AppendValues(s);
            }
            else
            {
                listStoreExcluded.AppendValues(s);
            }
        }
        UpdateClassLabels();
        catManager.SaveCategories();
    }

    private void UpdateClassLabels()
    {
        lClassesExcluded.Text = "Available: " + listStoreExcluded.IterNChildren().ToString();
        lClassesIncluded.Text = "Included: " + listStoreIncluded.IterNChildren().ToString();
    }

    protected void OnBCreateCategoryClicked(object sender, EventArgs e)
    {
        string categoryName = cbeCategorySelect.ActiveText.Trim();
        if (catManager.AddCategory(categoryName))
        {
            catSelectListStore.AppendValues(categoryName);
            categoryListStore.AppendValues(categoryName);
            int idx = catSelectListStore.IterNChildren() - 1;
            cbeCategorySelect.Active = idx;
            UpdateClassLabels();
            catManager.SaveCategories();
        }
    }

    protected void OnBRemoveCategoryClicked(object sender, EventArgs e)
    {
        string categoryName = cbeCategorySelect.ActiveText;
        if (cbeCategorySelect.GetActiveIter(out catSelectIter))
        {
            catSelectListStore.Remove(ref catSelectIter);
            catManager.RemoveCategory(categoryName);
            cbeCategorySelect.Active = 0;
            UpdateClassLabels();
            catManager.SaveCategories();
        }        
    }

    protected void OnCbeCategorySelectChanged(object sender, EventArgs e)
    {
        cbeCategorySelect.GetActiveIter(out catSelectIter);
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
        if (listStoreIncluded.IterIsValid(includedClassIter))
        {
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
        if (listStoreExcluded.IterIsValid(excludedClassIter))
        {
            nvClassesExcluded.Selection.SelectIter(excludedClassIter);
            // update selected class
            selectedExcludedClass = listStoreExcluded.GetValue(excludedClassIter, 0).ToString();
        }
    }

    protected void OnBExcludeSymbolClassClicked(object sender, EventArgs e)
    {
        if (String.IsNullOrWhiteSpace(selectedIncludedClass)
            || !listStoreIncluded.IterIsValid(includedClassIter))
        {
            return;
        }

        if (catManager.RemoveClassFromCurrent(selectedIncludedClass))
        {
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
            || !listStoreExcluded.IterIsValid(excludedClassIter))
        {
            return;
        }

        if (catManager.AddClassToCurrent(selectedExcludedClass))
        {
            // Append selected class from excludedClass to included classes
            listStoreIncluded.AppendValues(selectedExcludedClass);
            // Remove selected class from excludedClass
            listStoreExcluded.Remove(ref excludedClassIter);

            SelectNextClassExcluded();
            UpdateClassLabels();
        }
    }
}