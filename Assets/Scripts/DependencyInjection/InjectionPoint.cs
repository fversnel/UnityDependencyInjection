using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RamjetAnvil.DependencyInjection {

    public struct InjectionPoint
    {
        private readonly DependencyInfo _info;
        private readonly MethodInfo _injector;

        public InjectionPoint(DependencyInfo info, MethodInfo injector)
        {
            _info = info;
            _injector = injector;
        }

        public DependencyInfo Info
        {
            get { return _info; }
        }

        public MethodInfo Injector
        {
            get { return _injector; }
        }
    }
}
