using LineCraft.BusinessLogic;
using LineCraft.Entities;
using LineCraft.WinFormsApp.Model;
using LineCraft.WinFormsApp.UserControls;
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

        private ContextMenu rootContextMenu;
        private ContextMenu folderContextMenu;
        private ContextMenu commandContextMenu;

        public Main()
        {
            InitializeComponent();
            builder = new Builder();
            dataRepository = new DataRepository();
        }

        #region Events
        private void Main_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            splitContainer1.Panel2.Hide();

            settings = dataRepository.GetSettings();
            var folders = GetFolders(settings.Profile);
            RenderTree(folders);
            InitializeTreeContextMenus();
        }

        private void Parameter_TextChanged(object sender, EventArgs e)
        {
            UpdateCommandText((TextBox)sender);
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;

            if (e.Button != MouseButtons.Right)
                return;

            switch (e.Node.Level)
            {
                case 0:
                    rootContextMenu.Show(treeView1, e.Location);
                    break;

                case 1:
                    folderContextMenu.Show(treeView1, e.Location);
                    break;

                case 2:
                    commandContextMenu.Show(treeView1, e.Location);
                    break;
            }
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

        private void BtnCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtCommand.Text);
        }

        private void RootContextMenuItem_OnClick(object sender, EventArgs e)
        {
            string newFolderName = "";
            if(CustomDialogs.ShowInputDialog("New Folder", "New folder name:", ref newFolderName) == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(newFolderName))
                {
                    var existingFolder = settings.Profile.Folders.FirstOrDefault(f => f.Name == newFolderName);
                    if(existingFolder != null)
                    {
                        MessageBox.Show("A folder with the same name already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var folder = new FolderModel();
                        folder.Name = newFolderName;
                        folder.Commands = new List<CommandModel>();

                        settings.Profile.Folders.Add(folder);
                        dataRepository.SaveProfile(settings.Profile);
                        RenderTree(GetFolders(settings.Profile));
                    }
                }
            }
        }

        private void FolderContextMenuItem_OnClick(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Text == "New Command")
            {
                Form newCommandForm = new NewCommandForm(this);
                newCommandForm.Show();
                Hide();
            }
            else if (menuItem.Text == "Delete Folder")
            {
                var confirmResult = MessageBox.Show("Are you sure? Deleting a folder will also delete all its commands.", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    var folder = settings.Profile.Folders.First(f => f.Name == treeView1.SelectedNode.Text);
                    settings.Profile.Folders.Remove(folder);
                    dataRepository.SaveProfile(settings.Profile);
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
            }
        }

        private void CommandContextMenuItem_OnClick(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Text == "Edit Command")
            {
                Form newCommandForm = new NewCommandForm(this, treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text);
                Hide();
                newCommandForm.Show();
            }
            else if (menuItem.Text == "Delete Command")
            {
                var confirmResult = MessageBox.Show("Are you sure? You want to delete this command?.", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    var folder = settings.Profile.Folders.First(f => f.Name == treeView1.SelectedNode.Parent.Text);
                    var command = folder.Commands.First(c => c.Name == treeView1.SelectedNode.Text);
                    folder.Commands.Remove(command);
                    dataRepository.SaveProfile(settings.Profile);
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
            }
        }

        #endregion

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

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(rootNode);
        }

        private void InitializeTreeContextMenus()
        {
            rootContextMenu = new ContextMenu();
            rootContextMenu.MenuItems.Add(new MenuItem("New Folder", RootContextMenuItem_OnClick));

            folderContextMenu = new ContextMenu();
            folderContextMenu.MenuItems.Add(new MenuItem("New Command", FolderContextMenuItem_OnClick));
            folderContextMenu.MenuItems.Add(new MenuItem("Delete Folder", FolderContextMenuItem_OnClick));

            commandContextMenu = new ContextMenu();
            commandContextMenu.MenuItems.Add(new MenuItem("Edit Command", CommandContextMenuItem_OnClick));
            commandContextMenu.MenuItems.Add(new MenuItem("Delete Command", CommandContextMenuItem_OnClick));
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
    }
}
