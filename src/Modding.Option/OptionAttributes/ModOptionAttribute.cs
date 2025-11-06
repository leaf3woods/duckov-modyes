using System;

namespace Modding.ModOption.OptionAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ModOptionAttribute : Attribute
    {
        public ModOptionAttribute(string modName, string? description = null, string? version = null)
        {
            ModName = modName;
            Description = description;
            Version = version;
        }

        public string? ModName { get; private set; }

        public string? Description { get; private set; }

        public string? Version { get; private set; }
    }
}
