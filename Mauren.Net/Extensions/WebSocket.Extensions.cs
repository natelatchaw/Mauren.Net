using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
    public static class WebSocketExtensions
    {
        public static async Task ReceiveAsync(this WebSocket socket, Stream stream, Int32 size, CancellationToken cancellationToken)
        {
            // Allocate the buffer
            Byte[] buffer = new Byte[size];

            while (cancellationToken.IsCancellationRequested is false)
            {
                // Receive the data
                WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<Byte>(buffer), cancellationToken);

                // If a Close frame was received
                if (socket.State is WebSocketState.CloseReceived && result.MessageType is WebSocketMessageType.Close)
                {
                    // Retrieve the close status, or Empty if not provided
                    WebSocketCloseStatus status = result.CloseStatus ?? WebSocketCloseStatus.Empty;
                    // Throw a CloseException
                    throw new WebSocketCloseException(status, result.CloseStatusDescription);
                }

                // Determine if this iteration is the last iteration
                Boolean endOfMessage = result.EndOfMessage;

                // Create a memory region over the received data
                Memory<Byte> memory = buffer.AsMemory(0, result.Count);
                // Write the memory region to the stream
                await stream.WriteAsync(memory, cancellationToken);

                // If this iteration was the end of the message, break
                if (endOfMessage) break;
            }
        }

        public static async Task SendAsync(this WebSocket socket, Stream stream, Int32 size, CancellationToken cancellationToken)
        {
            // Allocate the buffer
            Byte[] buffer = new Byte[size];

            while (cancellationToken.IsCancellationRequested is false)
            {
                // Read stream data into buffer and store number of read bytes
                Int32 bytes = await stream.ReadAsync(buffer, cancellationToken);
                // Create data segment from buffer range
                ArraySegment<Byte> data = new(buffer, 0, bytes);

                // Determine if this iteration is the last iteration
                Boolean endOfMessage = bytes < buffer.Length || stream.Position == stream.Length;

                if (socket.State is WebSocketState.CloseReceived)
                {
                    // Throw a CloseException
                    throw new WebSocketCloseException(WebSocketCloseStatus.NormalClosure, "Acknowledging Close frame");
                }

                // Send the data
                await socket.SendAsync(data, WebSocketMessageType.Text, endOfMessage, cancellationToken);

                // If this iteration was the end of the message, break
                if (endOfMessage) break;
            }
        }
    }
}

namespace System.Net.WebSockets
{
    public class WebSocketCloseException : Exception
    {
        public WebSocketCloseStatus Status { get; set; }
        public String? Description => this.Message;
        public WebSocketCloseException(WebSocketCloseStatus status, String? description) : base(description)
        {
            Status = status;
        }
    }
}
