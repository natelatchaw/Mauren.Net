using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mauren.Net
{
    internal interface IApiService 
    {
        /// <summary>
        /// Sends an <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <param name="requestMessage">
        /// The <see cref="HttpRequestMessage"/> to send.
        /// </param>
        /// 
        /// <param name="token">
        /// A <see cref="CancellationToken"/> for propogating notifications
        /// to cancel the transaction.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="Task"/> indicating completion,
        /// containing an <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken token = default);
    }
}
