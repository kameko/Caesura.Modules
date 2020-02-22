
namespace Caesura.Modules
{
    using System;
    using System.Threading.Tasks;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Loader;
    
    public class AssemblyLoaderProvider : AssemblyLoadContext, IAssemblyLoader
    {
        public event Func<string, Task> OnUnload;
        
        private AssemblyDependencyResolver? _resolver;
        
        public AssemblyLoaderProvider(string name) : this(name, true)
        {
            OnUnload = delegate { return Task.CompletedTask; };
            Unloading += _ => Task.Run(() => OnUnload.Invoke(GetName()));
        }
        
        public AssemblyLoaderProvider(string name, bool isCollectible) : base(name, isCollectible)
        {
            OnUnload = delegate { return Task.CompletedTask; };
            Unloading += _ => Task.Run(() => OnUnload.Invoke(GetName()));
        }
        
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (_resolver is null)
            {
                return null;
            }
            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            if (_resolver is null)
            {
                return IntPtr.Zero;
            }
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (!string.IsNullOrEmpty(libraryPath))
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }
            return IntPtr.Zero;
        }
        
        public string GetName()
        {
            return Name ?? string.Empty;
        }
        
        public Assembly? Load(string path)
        {
            _resolver = new AssemblyDependencyResolver(path);
            var asmname = new AssemblyName(Path.GetFileNameWithoutExtension(path));
            return this.Load(asmname);
        }
        
        public void Dispose()
        {
            if (IsCollectible)
            {
                this.Unload();
            }
        }
    }
}
