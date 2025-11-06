using System;

namespace Modding.ModOption.OptionAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class BaseOptionAttribute : Attribute
    {
        public BaseOptionAttribute(string keyname, string? description = null)
        {
            KeyName = keyname;
            Description = description;
        }

        public string? KeyName { get; private set; }
        public string? Description { get; private set; }
    }
}
