using System;

namespace FreeModBot.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string command)
        {
            Command = command;
        }

        public string Command { get; }
        public bool AdminOnly { get; set; }
    }
}
