using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json
{
    public static class JsonSerializerExtensions
    {
        extension(JsonSerializer)
        {

            /// <summary>
            /// Reads the <see cref="HttpContent"/> representing a single JSON value into a <typeparamref name="TValue"/>.
            /// The Stream will be read to completion.
            /// </summary>
            /// 
            /// <typeparam name="TValue">
            /// The type to deserialize the JSON value into.
            /// </typeparam>
            /// 
            /// <returns>
            /// A <typeparamref name="TValue"/> representation of the JSON value.
            /// </returns>
            /// 
            /// <param name="content">
            /// <see cref="HttpContent"/> containing JSON data to parse.
            /// </param>
            /// 
            /// <param name="options">
            /// Options to control the behavior during reading.
            /// </param>
            /// 
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="content"/> is <see langword="null"/>.
            /// </exception>
            /// 
            /// <exception cref="JsonException">
            /// The JSON is invalid,
            /// <typeparamref name="TValue"/> is not compatible with the JSON,
            /// or when there is remaining data in the Stream.
            /// </exception>
            /// 
            /// <exception cref="NotSupportedException">
            /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
            /// for <typeparamref name="TValue"/> or its serializable members.
            /// </exception>
            public static TValue? Deserialize<TValue>(HttpContent content, JsonSerializerOptions? options = default)
            {
                // Read the HttpContent as a stream synchronously
                Stream stream = content.ReadAsStream();
                // Deserialize the stream to an instance of T
                TValue? value = JsonSerializer.Deserialize<TValue>(stream, options);
                // Return the instance
                return value;
            }

            public static TValue? DeserializeOrDefault<TValue>(HttpContent content, JsonSerializerOptions? options = default, TValue? defaultValue = default)
            {
                try
                {
                    return JsonSerializer.Deserialize<TValue>(content, options);
                }
                catch
                {
                    return defaultValue;
                }
            }

            /// <summary>
            /// Reads the <see cref="HttpContent"/> representing a single JSON value into a <typeparamref name="TValue"/>.
            /// The Stream will be read to completion.
            /// </summary>
            /// 
            /// <typeparam name="TValue">
            /// The type to deserialize the JSON value into.
            /// </typeparam>
            /// 
            /// <returns>
            /// A <typeparamref name="TValue"/> representation of the JSON value.
            /// </returns>
            /// 
            /// <param name="content">
            /// <see cref="HttpContent"/> containing JSON data to parse.
            /// </param>
            /// 
            /// <param name="options">
            /// Options to control the behavior during reading.
            /// </param>
            /// 
            /// <param name="cancellationToken">
            /// The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the read operation.
            /// </param>
            /// 
            /// <exception cref="System.ArgumentNullException">
            /// <paramref name="content"/> is <see langword="null"/>.
            /// </exception>
            /// 
            /// <exception cref="JsonException">
            /// The JSON is invalid,
            /// <typeparamref name="TValue"/> is not compatible with the JSON,
            /// or when there is remaining data in the Stream.
            /// </exception>
            /// 
            /// <exception cref="NotSupportedException">
            /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
            /// for <typeparamref name="TValue"/> or its serializable members.
            /// </exception>
            public static async ValueTask<TValue?> DeserializeAsync<TValue>(HttpContent content, JsonSerializerOptions? options = default, CancellationToken cancellationToken = default)
            {
                // Read the HttpContent as a stream asynchronously
                Stream stream = await content.ReadAsStreamAsync(cancellationToken);
                // Deserialize the stream to an instance of T
                TValue? value = await JsonSerializer.DeserializeAsync<TValue>(stream, options, cancellationToken);
                // Return the instance
                return value;
            }
        }
    }
}
