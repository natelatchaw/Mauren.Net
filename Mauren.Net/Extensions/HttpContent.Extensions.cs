using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpContentExtensions
    {
        /// <summary>
        /// A wrapper extension method for <see cref="HttpContent.ReadAsStringAsync(System.Threading.CancellationToken)"/>
        /// that handles the case where <paramref name="content"/> is null.
        /// </summary>
        /// 
        /// <param name="content">
        /// The <see cref="HttpContent"/> to read as a <see cref="String"/>.
        /// </param>
        /// 
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static async Task<String?> ReadAsStringOrDefaultAsync(this HttpContent? content, String? defaultValue = default) => (content is not null) switch
        {
            true => await content.ReadAsStringAsync(),
            false => defaultValue,
        };

        /// <summary>
        /// A wrapper extension method for <see cref="HttpContent.ReadAsStringAsync(System.Threading.CancellationToken)"/>
        /// that handles the case where <paramref name="content"/> is null.
        /// </summary>
        /// 
        /// <param name="content">
        /// The <see cref="HttpContent"/> to read as a <see cref="String"/>.
        /// </param>
        /// 
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static async Task<String?> ReadAsStringOrDefaultAsync(this HttpContent? content, CancellationToken cancellationToken, String? defaultValue = default) => (content is not null) switch
        {
            true => await content.ReadAsStringAsync(cancellationToken),
            false => defaultValue,
        };
    }
}
