
namespace Caesura.Modules
{
    using System;
    using System.Threading.Tasks;
    using System.Reflection;
    
    public interface IAssemblyLoader : IDisposable
    {
        event Func<string, Task> OnUnload;
        
        string GetName();
        Assembly? Load(string path);
    }
}
