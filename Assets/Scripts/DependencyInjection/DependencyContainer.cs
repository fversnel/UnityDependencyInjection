using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RamjetAnvil.Util;

namespace RamjetAnvil.DependencyInjection {

    public class DependencyContainer {

        private readonly IDictionary<string, Dependency> _depsByString;
        private readonly IDictionary<Type, IList<Dependency>> _depsByType; 

        public DependencyContainer(IDictionary<string, object> dependencies) {
            _depsByString = new Dictionary<string, Dependency>();
            _depsByType = new Dictionary<Type, IList<Dependency>>();

            foreach (var d in dependencies) {
                AddDependency(new Dependency(d.Key, d.Value));
            }
        }

        public IDictionary<string, Dependency> DepsByString {
            get { return _depsByString; }
        }

        public IDictionary<Type, IList<Dependency>> DepsByType {
            get { return _depsByType; }
        }

        public void AddDependency(Dependency dependency) {
            _depsByString.Add(dependency.Name, dependency);

            // Fill the container with each possible type of the 
            // dependency, i.g. the instance type and all of its parents.
            foreach (var type in dependency.Instance.GetType().GetAllTypes())
            {
                IList<Dependency> deps;
                if (!_depsByType.TryGetValue(type, out deps))
                {
                    deps = new List<Dependency>();
                    _depsByType.Add(type, deps);
                }
                deps.Add(dependency);
            }
        }
    }
}
