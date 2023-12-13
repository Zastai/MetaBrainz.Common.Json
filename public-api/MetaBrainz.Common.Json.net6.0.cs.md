# API Reference: MetaBrainz.Common.Json

## Assembly Attributes

```cs
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETCoreApp,Version=v6.0", FrameworkDisplayName = ".NET 6.0")]
```

## Namespace: MetaBrainz.Common.Json

### Type: IJsonBasedObject

```cs
public interface IJsonBasedObject {

  System.Collections.Generic.IReadOnlyDictionary<string, object?>? UnhandledProperties {
    public abstract get;
  }

}
```

### Type: JsonBasedObject

```cs
public abstract class JsonBasedObject : IJsonBasedObject {

  System.Collections.Generic.Dictionary<string, object?>? UnhandledProperties {
    public get;
    public set;
  }

  protected JsonBasedObject();

}
```

### Type: JsonUtils

```cs
public static class JsonUtils {

  public static readonly System.Diagnostics.TraceSource TraceSource;

  bool WriteIndentedByDefault {
    public static get;
    public static set;
  }

  public static System.Text.Json.JsonSerializerOptions CreateReaderOptions();

  public static System.Text.Json.JsonSerializerOptions CreateReaderOptions(System.Collections.Generic.IEnumerable<System.Text.Json.Serialization.JsonConverter> readers);

  public static System.Text.Json.JsonSerializerOptions CreateReaderOptions(System.Collections.Generic.IEnumerable<System.Text.Json.Serialization.JsonConverter> readers, params System.Text.Json.Serialization.JsonConverter[] moreReaders);

  public static System.Text.Json.JsonSerializerOptions CreateReaderOptions(params System.Text.Json.Serialization.JsonConverter[] readers);

  public static System.Text.Json.JsonSerializerOptions CreateWriterOptions();

  public static System.Text.Json.JsonSerializerOptions CreateWriterOptions(System.Collections.Generic.IEnumerable<System.Text.Json.Serialization.JsonConverter> writers);

  public static System.Text.Json.JsonSerializerOptions CreateWriterOptions(System.Collections.Generic.IEnumerable<System.Text.Json.Serialization.JsonConverter> writers, params System.Text.Json.Serialization.JsonConverter[] moreWriters);

  public static System.Text.Json.JsonSerializerOptions CreateWriterOptions(params System.Text.Json.Serialization.JsonConverter[] writers);

  public static T? Deserialize<T>(string json, System.Text.Json.JsonSerializerOptions options);

  public static T? Deserialize<T>(string json);

  public static System.Threading.Tasks.ValueTask<T> DeserializeAsync<T>(System.IO.Stream json, System.Text.Json.JsonSerializerOptions options, System.Threading.CancellationToken cancellationToken = default);

  public static System.Threading.Tasks.ValueTask<T> DeserializeAsync<T>(System.IO.Stream json, System.Threading.CancellationToken cancellationToken = default);

  public static T GetJsonContent<T>(System.Net.Http.HttpResponseMessage response);

  public static T GetJsonContent<T>(System.Net.Http.HttpResponseMessage response, System.Text.Json.JsonSerializerOptions options);

  public static System.Threading.Tasks.Task<T> GetJsonContentAsync<T>(System.Net.Http.HttpResponseMessage response, System.Text.Json.JsonSerializerOptions options, System.Threading.CancellationToken cancellationToken = default);

  public static System.Threading.Tasks.Task<T> GetJsonContentAsync<T>(System.Net.Http.HttpResponseMessage response, System.Threading.CancellationToken cancellationToken = default);

  public static object GetObject(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options);

  public static T GetObject<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options);

  public static T GetObject<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.Serialization.JsonConverter<T> converter, System.Text.Json.JsonSerializerOptions options);

  public static bool? GetOptionalBoolean(this ref System.Text.Json.Utf8JsonReader reader);

  public static byte? GetOptionalByte(this ref System.Text.Json.Utf8JsonReader reader);

  public static System.DateTimeOffset? GetOptionalDateTimeOffset(this ref System.Text.Json.Utf8JsonReader reader);

  public static decimal? GetOptionalDecimal(this ref System.Text.Json.Utf8JsonReader reader);

  public static double? GetOptionalDouble(this ref System.Text.Json.Utf8JsonReader reader);

  public static System.Guid? GetOptionalGuid(this ref System.Text.Json.Utf8JsonReader reader);

  public static short? GetOptionalInt16(this ref System.Text.Json.Utf8JsonReader reader);

  public static int? GetOptionalInt32(this ref System.Text.Json.Utf8JsonReader reader);

  public static long? GetOptionalInt64(this ref System.Text.Json.Utf8JsonReader reader);

  public static object? GetOptionalObject(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options);

  public static T? GetOptionalObject<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options)
    where T : class;

  public static T? GetOptionalObject<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.Serialization.JsonConverter<T> converter, System.Text.Json.JsonSerializerOptions options)
    where T : class;

  public static sbyte? GetOptionalSByte(this ref System.Text.Json.Utf8JsonReader reader);

  public static float? GetOptionalSingle(this ref System.Text.Json.Utf8JsonReader reader);

  public static ushort? GetOptionalUInt16(this ref System.Text.Json.Utf8JsonReader reader);

  public static uint? GetOptionalUInt32(this ref System.Text.Json.Utf8JsonReader reader);

  public static ulong? GetOptionalUInt64(this ref System.Text.Json.Utf8JsonReader reader);

  public static System.Uri? GetOptionalUri(this ref System.Text.Json.Utf8JsonReader reader);

  public static T? GetOptionalValue<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.Serialization.JsonConverter<T> converter, System.Text.Json.JsonSerializerOptions options)
    where T : struct;

  public static string GetPropertyName(this ref System.Text.Json.Utf8JsonReader reader);

  public static string GetRawStringValue(this ref System.Text.Json.Utf8JsonReader reader);

  public static string GetStringValue(this ref System.Text.Json.Utf8JsonReader reader);

  public static System.Uri GetUri(this ref System.Text.Json.Utf8JsonReader reader);

  public static T GetValue<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.Serialization.JsonConverter<T> converter, System.Text.Json.JsonSerializerOptions options)
    where T : struct;

  public static string Prettify(string json);

  public static System.Collections.Generic.IReadOnlyDictionary<string, T>? ReadDictionary<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options);

  public static System.Collections.Generic.IReadOnlyDictionary<string, T>? ReadDictionary<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.Serialization.JsonConverter<T> converter, System.Text.Json.JsonSerializerOptions options);

  public static System.Collections.Generic.IReadOnlyDictionary<string, T>? ReadDictionary<T, TValue>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options)
    where TValue : T;

  public static System.Collections.Generic.IReadOnlyDictionary<string, T>? ReadDictionary<T, TValue>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.Serialization.JsonConverter<TValue> converter, System.Text.Json.JsonSerializerOptions options)
    where TValue : T;

  public static System.Collections.Generic.IReadOnlyList<T>? ReadList<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options);

  public static System.Collections.Generic.IReadOnlyList<T>? ReadList<T>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.Serialization.JsonConverter<T> converter, System.Text.Json.JsonSerializerOptions options);

  public static System.Collections.Generic.IReadOnlyList<T>? ReadList<T, TValue>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options)
    where TValue : T;

  public static System.Collections.Generic.IReadOnlyList<T>? ReadList<T, TValue>(this ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.Serialization.JsonConverter<TValue> converter, System.Text.Json.JsonSerializerOptions options)
    where TValue : T;

  public static bool TryGetUri(this ref System.Text.Json.Utf8JsonReader reader, out System.Uri? value);

  public static void WriteList<T>(this System.Text.Json.Utf8JsonWriter writer, System.Collections.Generic.IEnumerable<T> values, System.Text.Json.JsonSerializerOptions options);

  public static void WriteList<TList, TConverter>(this System.Text.Json.Utf8JsonWriter writer, System.Collections.Generic.IEnumerable<TList> values, System.Text.Json.Serialization.JsonConverter<TConverter> converter, System.Text.Json.JsonSerializerOptions options)
    where TList : TConverter;

  public static System.Threading.Tasks.Task WriteListAsync<T>(this System.Text.Json.Utf8JsonWriter writer, System.Collections.Generic.IAsyncEnumerable<T> values, System.Text.Json.JsonSerializerOptions options);

  public static System.Threading.Tasks.Task WriteListAsync<TList, TConverter>(this System.Text.Json.Utf8JsonWriter writer, System.Collections.Generic.IAsyncEnumerable<TList> values, System.Text.Json.Serialization.JsonConverter<TConverter> converter, System.Text.Json.JsonSerializerOptions options)
    where TList : TConverter;

}
```

## Namespace: MetaBrainz.Common.Json.Converters

### Type: AnyObjectReader

```cs
public sealed class AnyObjectReader : JsonReader<object> {

  public static readonly AnyObjectReader Instance;

  public AnyObjectReader();

  public override object Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options);

}
```

### Type: JsonConverter\<T, TReader, TWriter>

```cs
public class JsonConverter<T, TReader, TWriter> : System.Text.Json.Serialization.JsonConverter<T>
  where TReader : JsonReader<T>, new()
  where TWriter : JsonWriter<T>, new() {

  public JsonConverter();

  public JsonConverter(TReader reader);

  public JsonConverter(TReader reader, TWriter writer);

  public JsonConverter(TWriter writer);

  public override T? Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options);

  public override void Write(System.Text.Json.Utf8JsonWriter writer, T value, System.Text.Json.JsonSerializerOptions options);

}
```

### Type: JsonReader\<T>

```cs
public abstract class JsonReader<T> : System.Text.Json.Serialization.JsonConverter<T> {

  protected JsonReader();

  public sealed override void Write(System.Text.Json.Utf8JsonWriter writer, T value, System.Text.Json.JsonSerializerOptions options);

}
```

### Type: JsonWriter\<T>

```cs
public abstract class JsonWriter<T> : System.Text.Json.Serialization.JsonConverter<T> {

  protected JsonWriter();

  public sealed override T Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options);

}
```

### Type: ObjectReader\<T>

```cs
public abstract class ObjectReader<T> : JsonReader<T> {

  protected ObjectReader();

  public sealed override T Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options);

  protected abstract T ReadObjectContents(ref System.Text.Json.Utf8JsonReader reader, System.Text.Json.JsonSerializerOptions options);

}
```

### Type: ObjectWriter\<T>

```cs
public abstract class ObjectWriter<T> : JsonWriter<T> {

  protected ObjectWriter();

  public sealed override void Write(System.Text.Json.Utf8JsonWriter writer, T value, System.Text.Json.JsonSerializerOptions options);

  protected abstract void WriteObjectContents(System.Text.Json.Utf8JsonWriter writer, T value, System.Text.Json.JsonSerializerOptions options);

}
```
