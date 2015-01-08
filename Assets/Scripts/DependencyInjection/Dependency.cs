using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RamjetAnvil.DependencyInjection
{
    public struct Dependency
    {
        private readonly string _name;
        private readonly object _instance;

        public Dependency(string name, object instance)
        {
            _name = name;
            _instance = instance;
        }

        public string Name
        {
            get { return _name; }
        }

        public object Instance
        {
            get { return _instance; }
        }
    }
}
