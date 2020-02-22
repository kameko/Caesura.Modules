
namespace Caesura.Modules.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    
    public class MockAssemblyLoader : IAssemblyLoader
    {
        public event Func<string, Task> OnUnload;
        
        private string Name { get; set; }
        
        public MockAssemblyLoader(string name) : this(name, true)
        {
            
        }
        
        public MockAssemblyLoader(string name, bool isCollectable)
        {
            Name     = name;
            OnUnload = delegate { return Task.CompletedTask; };
        }
        
        public string GetName()
        {
            return Name;
        }
        
        public Assembly? Load(string path)
        {
            return Assembly.GetExecutingAssembly();
        }
        
        public void Dispose()
        {
            Task.Run(() => OnUnload.Invoke(Name));
        }
    }
}
