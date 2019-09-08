using LineCraft.BusinessLogic;
using LineCraft.Entities;
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
        private Command currentCommand;
        private readonly Builder builder;

        public Main()
        {
            InitializeComponent();
            builder = new Builder();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            var folders = GetFolders();
            currentCommand = folders[0].Commands[0];
            lblCommandName.Text = folders[0].Commands[0].Name;
            RenderCommandParameters(folders[0].Commands[0].Parameters);
            RenderCommandParameters(folders[0].Commands[0].Parameters);
            RenderCommandParameters(folders[0].Commands[0].Parameters);
            RenderCommandParameters(folders[0].Commands[0].Parameters);
            RenderCommandParameters(folders[0].Commands[0].Parameters);
            RenderCommandParameters(folders[0].Commands[0].Parameters);
            //flowLayoutPanel1.BackColor = Color.Green;
        }

        private List<Folder> GetFolders()
        {
            var folders = new List<Folder>();
            var folder = new Folder();
            folder.Name = "Default";
            folder.Commands = new List<Command>();

            var command = new Command();
            command.Name = "Command1";
            command.Expression = "SC create \"@param1\" \"@param2\"";
            command.Parameters = new List<Parameter>();

            var parameter1 = new Parameter();
            parameter1.Name = "param1";
            parameter1.DisplayName = "Service Name";

            var parameter2 = new Parameter();
            parameter2.Name = "param2";
            parameter2.DisplayName = "Bin location";

            command.Parameters.Add(parameter1);
            command.Parameters.Add(parameter2);
            folder.Commands.Add(command);

            folders.Add(folder);

            return folders;
        }

        private void RenderCommandParameters(List<Parameter> parameters)
        {
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

                flowLayoutPanel1.Controls.Add(panel);
                UpdateCommandText(textbox);
            }
        }

        private void Parameter_TextChanged(object sender, EventArgs e)
        {
            UpdateCommandText((TextBox)sender);
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
