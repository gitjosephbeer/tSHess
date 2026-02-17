using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tSHess.Font
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ChessFontResourceNameAttribute : Attribute
    {
        // Fields
        private string name;

        // Methods
        public ChessFontResourceNameAttribute()
        {
        }

        public ChessFontResourceNameAttribute(string name)
        {
            this.name = name;
        }

        // Properties
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
    }
}
