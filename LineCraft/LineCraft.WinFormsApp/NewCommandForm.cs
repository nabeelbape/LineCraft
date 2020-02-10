using LineCraft.WinFormsApp.Model;
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
        private string folderName;
        private string commandName;

        private bool editMode = false;
        private bool isCancelClicked = false;

        private readonly DataRepository dataRepository;
        private SettingsModel settings;

        public NewCommandForm()
        {
            InitializeComponent();
        }

        public NewCommandForm(Form parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm;
            dataRepository = new DataRepository();
        }

        public NewCommandForm(Form parentForm, string folderName, string commandName)
        {
            InitializeComponent();
            this.parentForm = parentForm;
            this.folderName = folderName;
            this.commandName = commandName;
            editMode = true;
            dataRepository = new DataRepository();
        }

        private void NewCommandForm_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            settings = dataRepository.GetSettings();
            if (editMode)
            {
                Text = "Edit Command";
                var folder = settings.Profile.Folders.First(f => f.Name == folderName);
                var command = folder.Commands.First(c => c.Name == commandName);
                PrefillForm(command); 
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            isCancelClicked = true;
            Close();
            parentForm.Show();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show(txtExpression.Text);
        }

        private void NewCommandForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!isCancelClicked)
                parentForm.Close();
        }

        void PrefillForm(CommandModel commandModel)
        {
            txtCommandName.Text = commandModel.Name;
            txtExpression.Text = commandModel.Expression;
        }
    }
}
