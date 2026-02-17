using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tSHess.Font
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ChessFontMappingsAttribute : Attribute
    {
        // Fields
        private ChessFontSymbol m_symbol;
        private char m_mappingChar;

        // Methods
        public ChessFontMappingsAttribute()
        {
        }

        public ChessFontMappingsAttribute(ChessFontSymbol symbol, char mappingChar)
        {
            this.m_symbol = symbol;
            this.m_mappingChar = mappingChar;
        }

        // Properties
        public ChessFontSymbol Symbol
        {
            get
            {
                return this.m_symbol;
            }
        }

        public char MappingCharacter
        {
            get
            {
                return this.m_mappingChar;
            }
        }
    }
}
