using PrintaDot.CommunicationProtocol;
using System.Text;

namespace PrintaDot.Common;

public static class StreamHandler
{

    /// <summary>
    /// Reads and decodes the message according to the native messaging protocol and
    /// deserializes it into a Message object.
    /// </summary>
    public static Message Read()
    {
        Stream stdin = Console.OpenStandardInput();

        byte[] lengthBytes = new byte[4];
        stdin.Read(lengthBytes, 0, 4);

        char[] buffer = new char[BitConverter.ToInt32(lengthBytes, 0)];

        using (StreamReader reader = new StreamReader(stdin))
            if (reader.Peek() >= 0)
            {
                reader.Read(buffer, 0, buffer.Length);
            }

        var jsonString = new string(buffer);

        return jsonString.FromJson<Message>();
    }

    /// <summary>
    /// Writes message to the standard output stream.
    /// Encodes the message according to the native messaging protocol
    /// </summary>
    public static void Write(Message message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message.ToJson());
        Stream stdout = Console.OpenStandardOutput();

        stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
        stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
        stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
        stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
        stdout.Write(bytes, 0, bytes.Length);

        stdout.Flush();
    }
}
