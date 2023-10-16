using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRML.Console
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConsoleAppearance : Attribute
    {
        internal Colors consoleCol = Colors.green;
        internal string name;

        public ConsoleAppearance(Colors nameCol)
        {
            consoleCol = nameCol;
        }

        public ConsoleAppearance(string name)
        {
            this.name = name;
        }

        public ConsoleAppearance(Colors nameCol, string name)
        {
            consoleCol = nameCol;
            this.name = name;
        }
    }
}
