using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mauren.Net.Test
{
    /// <summary>
    /// Defines <see cref="HttpMessageHandler"/> method signatures of protected members.
    /// </summary>
    public interface IHttpMessageHandler : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);

        void Dispose(Boolean disposing = false);
    }
}
