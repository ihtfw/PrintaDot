using PrintaDot.Shared.CommunicationProtocol;
using PrintaDot.Shared.CommunicationProtocol.V1;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace PrintaDot.Shared.Common;

public static class PrintaDotJsonSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new();

    static PrintaDotJsonSerializer()
    {
        ApplyDefaultJsonSerializerOptions(DefaultOptions);
    }

    public static T FromJson<T>(this string self, bool safe = false)
    {
        return (T)self.FromJson(typeof(T), safe);
    }

    public static object FromJson(this string self, Type type, bool safe = false)
    {
        return Deserialize(self, type, safe);
    }

    public static string ToJson(this object self)
    {
        return self == null ? null : Serialize(self);
    }

    public static object Deserialize(string value, Type type, bool safe = false)
    {
        if (value == null)
            return null;

        if (safe)
        {
            try
            {
                return JsonSerializer.Deserialize(value, type, DefaultOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        return JsonSerializer.Deserialize(value, type, DefaultOptions);
    }

    public static TBase DeserializeAbstract<TBase>(string value, JsonConverter converter) where TBase : class
    {
        var options = new JsonSerializerOptions();
        ApplyDefaultJsonSerializerOptions(options);
        options.Converters.Add(converter);

        return JsonSerializer.Deserialize<TBase>(value, options);
    }

    public static string Serialize(object value)
    {
        return JsonSerializer.Serialize(value, DefaultOptions);
    }

    public static string SerializeAbstract<TBase>(TBase value, JsonConverter converter) where TBase : class
    {
        var options = new JsonSerializerOptions();
        ApplyDefaultJsonSerializerOptions(options);
        options.Converters.Add(converter);

        return JsonSerializer.Serialize(value, typeof(TBase), options);
    }

    public static Message? FromJsonToMessage(this string self)
    {
        try
        {
            using var document = JsonDocument.Parse(self);
            var root = document.RootElement;

            if (!root.TryGetProperty("type", out var typeProperty) ||
                !root.TryGetProperty("version", out var versionProperty))
            {
                return null;
            }

            var type = typeProperty.GetString();
            var version = versionProperty.GetInt32();
            
            if (string.IsNullOrEmpty(type))
                return null;

            return DeserializeWithFallback(self, type, version);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static Message? DeserializeWithFallback(string json, string messageType, int requestedVersion)
    {
        Log.LogMessage($"{requestedVersion} {messageType}");
        if (!MessageTypes.SupportedMessageVersions.TryGetValue(messageType, out var supportedVersions) ||
            supportedVersions.Count == 0)
        {
            return null;
        }
        Log.LogMessage("type recived");
        var sortedVersions = supportedVersions.OrderByDescending(v => v).ToList();

        foreach (var targetVersion in sortedVersions)
        {
            if (targetVersion > requestedVersion)
                continue;

            try
            {
                var message = DeserializeToVersion(json, messageType, targetVersion);
                if (message != null)
                {
                    message.Version = targetVersion;
                    return message;
                }
            }
            catch (Exception ex)
            {
                Log.LogMessage($"{ex.Message}");

                continue;
            }
        }

        return null;
    }

    private static Message? DeserializeToVersion(string json, string messageType, int targetVersion)
    {
        return messageType switch
        {
            MessageTypes.PrintRequestMessageType => targetVersion switch
            {
                0 => json.FromJson<Message>(),
                1 => json.FromJson<PrintRequestMessageV1>(),
                _ => null
            },
            MessageTypes.GetPrintStatusRequestMessageType => targetVersion switch
            {
                1 => json.FromJson<GetPrintStatusRequestMessageV1>(),
                _ => null
            },
            MessageTypes.GetPrintStatusResponseMessageType => targetVersion switch
            {
                1 => json.FromJson<GetPrintStatusResponseMessageV1>(),
                _ => null
            },
            MessageTypes.ProfileType => targetVersion switch
            {
                1 => json.FromJson<ProfileMessageV1>(),
                _ => null
            },
            MessageTypes.ProfilesType => targetVersion switch
            {
                1 => json.FromJson<ProfilesMessageV1>(),
                _ => null
            },
            _ => ExceptionMessageV1.Create("Exception during deserialization: Provided messsage type doesnt exist"),
        };
    }

    public static void ApplyDefaultJsonSerializerOptions(JsonSerializerOptions target)
    {
        target.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        target.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        target.UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip;
        target.Converters.Add(new JsonStringEnumConverter());
    }
}
