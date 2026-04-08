using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace TextureReplacerEditor.Miscellaneous;

public class CultureInfoConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var num = float.Parse(value.ToString());
        writer.WriteValue(num.ToString(CultureInfo.InvariantCulture));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    private static readonly List<Type> ConvertableTypes = new()
    {
        typeof(decimal),
        typeof(float),
        typeof(int),
        typeof(uint),
        typeof(nint),
        typeof(nuint),
        typeof(long),
        typeof(ulong),
        typeof(short),
        typeof(ushort)
    };

    public override bool CanRead => false;

    public override bool CanConvert(Type objectType)
    {
        return ConvertableTypes.Any(t => t == objectType);
    }
}