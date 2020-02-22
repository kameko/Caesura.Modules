
namespace Caesura.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    
    public class ModuleContainer : IDisposable
    {
        public string Name => Loader?.GetName() ?? string.Empty;
        public string TempPath { get; set; }
        public string OriginPath { get; set; }
        
        protected Func<string, IAssemblyLoader> LoaderFactory { get; set; }
        protected IAssemblyLoader? Loader { get; set; }
        protected Assembly? ModuleAssembly { get; set; }
        
        public ModuleContainer()
        {
            LoaderFactory = name => new AssemblyLoaderProvider(name);
            TempPath      = string.Empty;
            OriginPath    = string.Empty;
        }
        
        public ModuleContainer(Func<string, IAssemblyLoader> loader_factory)
        {
            LoaderFactory = loader_factory;
            TempPath      = string.Empty;
            OriginPath    = string.Empty;
        }
        
        public void Load(string origin_path, string temp_path)
        {
            Loader       ??= LoaderFactory.Invoke(Name);
            OriginPath     = origin_path;
            // TODO: copy the entire folder at origin_path to temp_path
            ModuleAssembly = Loader.Load(temp_path);
        }
        
        public IEnumerable<Type> GetSelectTypes<T>()
        {
            if (ModuleAssembly is null)
            {
                throw new InvalidOperationException("Module not loaded yet");
            }
            
            return ModuleAssembly.GetExportedTypes().Where(x => x.IsSubclassOf(typeof(T)));
        }
        
        public Type GetSelectType<T>()
        {
            var types = GetSelectTypes<T>();
            
            if (types.Count() > 1)
            {
                throw new InvalidOperationException(
                    $"More than one type inheriting or implementing \"{typeof(T).GetType().Name}\" found in assembly"
                );
            }
            else if (types.Count() == 0)
            {
                throw new InvalidOperationException(
                    $"No types inheriting or implementing \"{typeof(T).GetType().Name}\" found in assembly"
                );
            }
            
            return types.First();
        }
        
        public void Dispose()
        {
            Loader?.Dispose();
        }
    }
}
