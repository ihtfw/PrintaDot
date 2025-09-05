using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Encodings.Web;
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


    public static void ApplyDefaultJsonSerializerOptions(JsonSerializerOptions target)
    {
        target.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        target.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }
}
