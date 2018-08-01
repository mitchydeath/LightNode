using System; 
using System.Threading.Tasks;
using LightNodeForDotNetCore.Interface;

namespace LightNode 
{ 
    public interface _IToriaezu : IClientApi
    {
        Task<System.String> EchoAsync(System.String x, IProgress<float> reportProgress = null);
    }

    public interface _IMyClass : IClientApi
    {
        Task<System.String> HogeAsync(System.String i, IProgress<float> reportProgress = null);
        Task<System.Int32[]> ArraySendTestGetAsync(System.Int32[] xs, IProgress<float> reportProgress = null);
        Task<System.Int32[]> ArraySendTestPostAsync(System.Int32[] xs, IProgress<float> reportProgress = null);
    }


}