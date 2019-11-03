using LineCraft.BusinessLogic;
using LineCraft.Entities;
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
    public partial class Main : Form
    {
        private readonly Builder builder;
        private readonly DataRepository dataRepository;
        private Command currentCommand;
        private SettingsModel settings;

        public Main()
        {
            InitializeComponent();
            builder = new Builder();
            dataRepository = new DataRepository();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Hide();

            settings = dataRepository.GetSettings();
            var folders = GetFolders(settings.Profile);
            RenderTree(folders);
        }

        private void Parameter_TextChanged(object sender, EventArgs e)
        {
            UpdateCommandText((TextBox)sender);
        }

        private void BtnNewCommand_Click(object sender, EventArgs e)
        {
            this.Hide();
            var newCommandForm = new NewCommandForm(this);
            newCommandForm.Show();
        }

        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 0 || e.Node.Level == 1)
                return;

            string selectedFolderName = e.Node.Parent.Text;
            string selectedCommandName = e.Node.Text;

            var folder = GetFolders(settings.Profile).First(f => f.Name == selectedFolderName);
            currentCommand = folder.Commands.First(c => c.Name == selectedCommandName);
            RenderCommand(currentCommand);
            splitContainer1.Panel2.Show();
        }

        private List<Folder> GetFolders(ProfileModel profile)
        {
            var folders = new List<Folder>();

            foreach (var profileFolder in profile.Folders)
            {
                var folder = new Folder();
                folder.Name = profileFolder.Name;
                folder.Commands = new List<Command>();

                foreach (var profileCommand in profileFolder.Commands)
                {
                    var command = new Command();
                    command.Name = profileCommand.Name;
                    command.Description = profileCommand.Description;
                    command.Expression = profileCommand.Expression;
                    command.Parameters = new List<Parameter>();

                    foreach (var profileParameter in profileCommand.Parameters)
                    {
                        var parameter = new Parameter();
                        parameter.Name = profileParameter.Name;
                        parameter.DisplayName = profileParameter.DisplayName;

                        command.Parameters.Add(parameter);
                    }

                    folder.Commands.Add(command);
                }

                folders.Add(folder);
            }

            return folders;
        }

        private void RenderTree(List<Folder> folders)
        {
            TreeNode rootNode = new TreeNode("Home");
            rootNode.Expand();

            foreach (var folder in folders)
            {
                TreeNode folderNode = new TreeNode(folder.Name);
                folderNode.Expand();

                foreach (var command in folder.Commands)
                {
                    var commandNode = new TreeNode(command.Name);
                    folderNode.Nodes.Add(commandNode);
                }
                rootNode.Nodes.Add(folderNode);
            }

            treeView1.Nodes.Add(rootNode);
        }

        private void RenderCommand(Command command)
        {
            lblCommandName.Text = command.Name;
            lblCommandDescription.Text = command.Description;
            RenderCommandParameters(command.Parameters);
        }

        private void RenderCommandParameters(List<Parameter> parameters)
        {
            parametersPanel.Controls.Clear();

            foreach (var parameter in parameters)
            {
                var panel = new Panel();
                panel.Size = new Size(350, 40);
                
                var label = new Label();
                label.Text = parameter.DisplayName;
                label.Location = new Point(10, 12);

                var textbox = new TextBox();
                textbox.Name = "txt" + parameter.Name;
                textbox.Location = new Point(120, 9);
                textbox.Size = new Size(200, 40);
                textbox.TextChanged += Parameter_TextChanged;

                panel.Controls.Add(label);
                panel.Controls.Add(textbox);

                //panel.BackColor = Color.Red;

                parametersPanel.Controls.Add(panel);
                UpdateCommandText(textbox);
            }
        }

        private void UpdateCommandText(TextBox textBox)
        {
            string parameterName = textBox.Name.Replace("txt", "");

            foreach (var parameter in currentCommand.Parameters)
            {
                if (parameter.Name == parameterName)
                {
                    parameter.Value = textBox.Text;
                    break;
                }
            }
            txtCommand.Text = builder.BuildCommand(currentCommand);
        }

        private void BtnCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtCommand.Text);
        }
    }
}
