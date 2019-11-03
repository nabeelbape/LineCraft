using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LineCraft.WinFormsApp
{
    public partial class NewCommandForm : Form
    {
        private Form parentForm;

        public NewCommandForm()
        {
            InitializeComponent();
        }

        public NewCommandForm(Form parent)
        {
            InitializeComponent();
            parentForm = parent;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            parentForm.Show();
            this.Close();
        }
    }
}
