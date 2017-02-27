using System;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SymbSearch
{
    public partial class FormMain : Form
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int MOD_ALT = 0x0001;
        const int MOD_CONTROL = 0x0002;
        const int MOD_SHIFT = 0x0004;
        const int WM_HOTKEY = 0x0312;

        private List<Symb> symbols = new List<Symb>();

        public FormMain()
        {
            InitializeComponent();
            symbols = MakeList();
            ShowList(symbols);
            ShowCats(GetCats());
        }
        /// <summary>
        /// Filters symbols by category and search
        /// </summary>
        private void FilterList()
        {
            List<Symb> newList = new List<Symb>();
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i].name.ToLower().Contains(searchbox.Text.ToLower()))
                {
                    if (cbCategory.SelectedItem.ToString() == "all" || cbCategory.SelectedItem.ToString() == symbols[i].cat)
                    {
                        newList.Add(symbols[i]);
                    }
                }
            }
            DeleteList();
            ShowList(newList);
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
            }
            return list;
        }
        /// <summary>
        /// Adds the given list to listBox
        /// </summary>
        /// <param name="list"></param>
        private void ShowList(List<Symb> list)
        {
            foreach (Symb temp in list)
            {
                listBox.Items.Add(temp.sign + "\t" + temp.name);
            }
        }
        /// <summary>
        /// Clears the listBox
        /// </summary>
        private void DeleteList()
        {
            listBox.Items.Clear();
        }
        #endregion
        #region Cats of Symb
        /// <summary>
        /// Returns the categories of symbols
        /// </summary>
        /// <returns></returns>
        private List<string> GetCats()
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
        /// Adds the given categories to list
        /// </summary>
        private void ShowCats(List<string> cats)
        {
            cbCategory.Items.Clear();
            foreach (string temp in cats)
            {
                cbCategory.Items.Add(temp);
            }
            cbCategory.Text = "all";
        }
        /// <summary>
        /// Removes doubles of the given list
        /// </summary>
        private List<string> RemoveDoubles(List<string> l)
        {
            return l.Distinct().ToList();
        }
        #endregion
        #region Show and Hide
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && (int)m.WParam == 1)
            {
                if (WindowState == FormWindowState.Normal)
                    MiniForm();
                else if (WindowState == FormWindowState.Minimized)
                    ShowForm();
            }
            base.WndProc(ref m);
        }
        private void MiniForm()
        {
            WindowState = FormWindowState.Minimized;
        }
        private void ShowForm()
        {
            WindowState = FormWindowState.Normal;
            searchbox.Select();
        }
        #endregion
        #region Keys and Actions
        private void KeyControl(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    NextElement(listBox);
                    break;
                case Keys.Up:
                    PrevElement(listBox);
                    break;
                case Keys.PageDown:
                    NextElement(cbCategory);
                    break;
                case Keys.PageUp:
                    PrevElement(cbCategory);
                    break;
                case Keys.Enter:
                    ChooseSymb();
                    break;
            }
        }
        private static void NextElement(ListBox a)
        {
            if (a.SelectedIndex < a.Items.Count - 1)
                a.SelectedIndex = a.SelectedIndex + 1;
            else
                a.SelectedIndex = 0;
        }
        private static void NextElement(ComboBox a)
        {
            if (a.SelectedIndex < a.Items.Count - 1)
                a.SelectedIndex = a.SelectedIndex + 1;
            else
                a.SelectedIndex = 0;
        }
        private static void PrevElement(ListBox a)
        {
            if (a.SelectedIndex > 0)
                a.SelectedIndex = a.SelectedIndex - 1;
            else
                a.SelectedIndex = a.Items.Count - 1;
        }
        private static void PrevElement(ComboBox a)
        {
            if (a.SelectedIndex > 0)
                a.SelectedIndex = a.SelectedIndex - 1;
            else
                a.SelectedIndex = a.Items.Count - 1;
        }
        private void ChooseSymb()
        {
            if (listBox.SelectedItem != null)
            {
                Clipboard.SetText(listBox.SelectedItem.ToString().Substring(0, 1));
            }
            MiniForm();
        }
        #endregion 
        #region Events on Form
        private void btnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblSign.Text = listBox.SelectedItem.ToString().Substring(0, 1);
        }
        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterList();
        }
        private void searchbox_TextChanged(object sender, EventArgs e)
        {
            FilterList();
        }
        private void Form_Load(object sender, EventArgs e)
        {
            RegisterHotKey(this.Handle, 1, MOD_CONTROL + MOD_ALT, (int)Keys.W);
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 1);
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            KeyControl(e);
        }

        private void searchbox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
            KeyControl(e);
        }
        #endregion
    }
}
