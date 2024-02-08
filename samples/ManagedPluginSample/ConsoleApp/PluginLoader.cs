using System;
using System.Linq; 
using System.Reflection;

namespace ManagedPluginSample
{
    public class PluginLoader
    {
        Assembly _pluginAssembly;

        public PluginLoader(string pluginPath)
        {
            _pluginAssembly = Assembly.LoadFrom(pluginPath);
        }

        public Type[] GetTypes()
        {
            return _pluginAssembly.GetTypes();
        }

        public dynamic CreateInstance(string typeFullName)
        {
            var type = _pluginAssembly.GetTypes().FirstOrDefault(t => t.FullName == typeFullName);
            if (type == null)
            {
                throw new Exception($"Type {typeFullName} not found in {_pluginAssembly.FullName}.");
            }
            return Activator.CreateInstance(type);
        }
    }
}
