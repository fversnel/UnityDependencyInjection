using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RamjetAnvil.DependencyInjection {

    [AttributeUsage(AttributeTargets.Property)]
    public class DependencyInfo : Attribute {
        private readonly string _name;

        public DependencyInfo() {
            _name = null;
        }

        public DependencyInfo(string name) {
            _name = name;
        }

        public string Name {
            get { return _name; }
        }
    }
}
