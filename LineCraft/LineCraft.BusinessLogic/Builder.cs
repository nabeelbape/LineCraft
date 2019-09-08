using LineCraft.Entities;
using System;
using System.Text;

namespace LineCraft.BusinessLogic
{
    public class Builder
    {
        public string BuildCommand(Command command)
        {
            StringBuilder builder = new StringBuilder(command.Expression);

            foreach (var parameter in command.Parameters)
            {
                builder.Replace("@" + parameter.Name, parameter.Value);
            }

            return builder.ToString();
        }
    }
}
