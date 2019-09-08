using System.Collections.Generic;

namespace LineCraft.Entities
{
    public class Command
    {
        public string Name { get; set; }
        public string Expression { get; set; }
        public List<Parameter> Parameters { get; set; }
    }
}
