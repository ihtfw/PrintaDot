using PrintaDot.Shared.CommunicationProtocol;
using System.Text;

namespace PrintaDot.Shared.Common;

/// <summary>
/// Handle main stream of communication between browser and native application.
/// </summary>
public static class StreamHandler
{

    /// <summary>
    /// Reads and decodes the message according to the native messaging protocol and
    /// deserializes it into a Message object.
    /// </summary>
    public static Message? Read()
    {
        Stream stdin = Console.OpenStandardInput();

        // Read message length (4 bytes)
        byte[] lengthBytes = new byte[4];
        stdin.Read(lengthBytes, 0, 4);
        int length = BitConverter.ToInt32(lengthBytes, 0);

        // Read message bytes
        byte[] buffer = new byte[length];
        stdin.Read(buffer, 0, length);

        // Convert UTF-8 bytes → string
        string jsonString = Encoding.UTF8.GetString(buffer);

        Log.LogMessage(jsonString, nameof(StreamHandler));

        return jsonString.FromJsonToMessage();
    }

    /// <summary>
    /// Writes message to the standard output stream.
    /// Encodes the message according to the native messaging protocol
    /// </summary>
    public static void Write(Response response)
    {
        Log.LogMessage("Sending..." + response.ToJson());

        byte[] bytes = Encoding.UTF8.GetBytes(response.ToJson()!);
        Stream stdout = Console.OpenStandardOutput();

        stdout.WriteByte((byte)(bytes.Length >> 0 & 0xFF));
        stdout.WriteByte((byte)(bytes.Length >> 8 & 0xFF));
        stdout.WriteByte((byte)(bytes.Length >> 16 & 0xFF));
        stdout.WriteByte((byte)(bytes.Length >> 24 & 0xFF));
        stdout.Write(bytes, 0, bytes.Length);

        stdout.Flush();

        Log.LogMessage("Message has been send");
    }
}
