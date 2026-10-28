using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatShared.Protocol
{
    public static class MessageSerializer
    {
        private static readonly DataContractJsonSerializer serializer =
           new DataContractJsonSerializer(typeof(ChatMessage));
        public static string Serialize(ChatMessage msg)
        {
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, msg);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        public static ChatMessage Deserialize(string json)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (ChatMessage)serializer.ReadObject(ms);
            }
        }
        public static async Task SendMessageAsync(NetworkStream stream, ChatMessage msg, CancellationToken ct = default)
        {
            string json = Serialize(msg);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
            }
            await stream.WriteAsync(lengthBytes, 0, 4, ct);
            await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length, ct);
            await stream.FlushAsync(ct);
        }
        public static async Task<ChatMessage> ReceiveMessageAsync(NetworkStream stream, CancellationToken ct = default)
        {
            byte[] lengthBytes = new byte[4];
            int bytesRead = 0;
            while (bytesRead < 4)
            {
                int n = await stream.ReadAsync(lengthBytes, bytesRead, 4 - bytesRead, ct);
                if (n == 0) return null;
                bytesRead += n;
            }
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);
            int length = BitConverter.ToInt32(lengthBytes, 0);
            if (length <= 0 || length > 10 * 1024 * 1024)
                throw new InvalidDataException($"Kích thước gói tin không hợp lệ hoặc vượt ngưỡng cho phép (10MB): {length}");
            byte[] jsonBytes = new byte[length];
            bytesRead = 0;
            while (bytesRead < length)
            {
                int n = await stream.ReadAsync(jsonBytes, bytesRead, length - bytesRead, ct);
                if (n == 0) return null;
                bytesRead += n;
            }
            string json = Encoding.UTF8.GetString(jsonBytes);
            return Deserialize(json);
        }
    }
}