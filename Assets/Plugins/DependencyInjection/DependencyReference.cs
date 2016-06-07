namespace RamjetAnvil.DependencyInjection
{
    public struct DependencyReference {
        private readonly string _name;
        private readonly object _instance;

        public DependencyReference(string name, object instance)
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
