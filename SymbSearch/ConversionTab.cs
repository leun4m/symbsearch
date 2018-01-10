using System;
using System.Reflection;
using System.Collections.Generic;
using Gtk;
using SymbSearch;

public partial class MainWindow : Gtk.Window
{
    private void ConvertInput()
    {
        if (String.IsNullOrWhiteSpace(eConversionInput.Text))
        {
            return;
        }

        if (cbConversionUnit.ActiveText == "Symbol")
        {
            conversion.SetSymbol(eConversionInput.Text);
        }
        else if (cbConversionUnit.ActiveText == "Decimal")
        {
            conversion.SetDecimal(eConversionInput.Text);
        }
        else if (cbConversionUnit.ActiveText == "Hexadecimal")
        {
            conversion.SetHexadecimal(eConversionInput.Text);
        }
        else if (cbConversionUnit.ActiveText == "HTML Code")
        {
            conversion.SetHtml(eConversionInput.Text);
        }
        else
        {
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

    protected void OnCbConversionUnitChanged(object sender, EventArgs e)
    {
        ConvertInput();
    }

    protected void OnEConversionInputChanged(object sender, EventArgs e)
    {
        ConvertInput();
    }
}