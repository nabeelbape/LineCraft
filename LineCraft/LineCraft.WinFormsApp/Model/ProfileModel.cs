using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineCraft.WinFormsApp.Model
{
    public class ProfileModel
    {
        public string Name { get; set; }
        public List<FolderModel> Folders { get; set; }
    }

    public class FolderModel
    {
        public string Name { get; set; }
        public List<CommandModel> Commands { get; set; }
    }

    public class CommandModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Expression { get; set; }
        public List<ParameterModel> Parameters { get; set; }
    }

    public class ParameterModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
