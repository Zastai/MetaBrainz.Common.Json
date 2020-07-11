using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using MetaBrainz.Common.Json.Converters;

namespace MetaBrainz.Common.Json {

  /// <summary>Utility class, providing various methods to ease the use of System.Text.Json.</summary>
  [PublicAPI]
  public static class JsonUtils {

    #region General Utilities

    private static string DecodeUtf8(ReadOnlySpan<byte> bytes) {
#if NETSTANDARD2_1 || NETCOREAPP2_1 || NETCOREAPP3_1
      return Encoding.UTF8.GetString(bytes);
#else
      return Encoding.UTF8.GetString(bytes.ToArray());
#endif
    }

    /// <summary>Deserializes JSON to an object of the specified type, using default options.</summary>
    /// <param name="json">The JSON to deserialize.</param>
    /// <typeparam name="T">The type of object to deserialize.</typeparam>
    /// <returns>A newly deserialized object of type <typeparamref name="T"/>.</returns>
    /// <remarks>The options used match those returned by <see cref="CreateReaderOptions()"/>.</remarks>
    public static T Deserialize<T>(string json)
      => JsonUtils.Deserialize<T>(json, JsonUtils.ReaderOptions);

    /// <summary>Deserializes JSON to an object of the specified type.</summary>
    /// <param name="json">The JSON to deserialize.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <typeparam name="T">The type of object to deserialize.</typeparam>
    /// <returns>A newly deserialized object of type <typeparamref name="T"/>.</returns>
    public static T Deserialize<T>(string json, JsonSerializerOptions options)
      => JsonSerializer.Deserialize<T>(json, options);

    /// <summary>Deserializes JSON to an object of the specified type, using default options.</summary>
    /// <param name="json">The JSON to deserialize.</param>
    /// <typeparam name="T">The type of object to deserialize.</typeparam>
    /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
    /// <returns>A newly deserialized object of type <typeparamref name="T"/>.</returns>
    /// <remarks>The options used match those returned by <see cref="CreateReaderOptions()"/>.</remarks>
    public static ValueTask<T> DeserializeAsync<T>(Stream json, CancellationToken cancellationToken = default)
      => JsonUtils.DeserializeAsync<T>(json, JsonUtils.ReaderOptions, cancellationToken);

    /// <summary>Deserializes JSON to an object of the specified type.</summary>
    /// <param name="json">The JSON to deserialize.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
    /// <typeparam name="T">The type of object to deserialize.</typeparam>
    /// <returns>A newly deserialized object of type <typeparamref name="T"/>.</returns>
    public static ValueTask<T> DeserializeAsync<T>(Stream json, JsonSerializerOptions options, CancellationToken cancellationToken = default)
      => JsonSerializer.DeserializeAsync<T>(json, options, cancellationToken);

    /// <summary>Pretty-prints a JSON string.</summary>
    /// <param name="json">The JSON string to pretty-print.</param>
    /// <returns>
    /// An indented version of <paramref name="json"/>. If anything goes wrong, <paramref name="json"/> is returned unchanged.
    /// </returns>
    public static string Prettify(string json) {
      try {
        return JsonSerializer.Serialize(JsonDocument.Parse(json).RootElement, JsonUtils.PrettifyOptions);
      }
      catch {
        return json;
      }
    }

    #endregion

    #region Options

    private static readonly JsonSerializerOptions PrettifyOptions = new JsonSerializerOptions {
      // @formatter:off
      IgnoreNullValues = false,
      WriteIndented    = true,
      // @formatter:on
    };

    private static readonly JsonSerializerOptions ReaderOptions = JsonUtils.CreateReaderOptions();

    private static readonly JsonSerializerOptions WriterOptions = JsonUtils.CreateWriterOptions();

    /// <summary>Creates JSON serializer options for reading (deserialization).</summary>
    /// <returns>JSON serializer options for reading (deserialization).</returns>
    public static JsonSerializerOptions CreateReaderOptions() => new JsonSerializerOptions {
      // @formatter:off
      AllowTrailingCommas         = false,
      IgnoreNullValues            = false,
      PropertyNameCaseInsensitive = false,
      // @formatter:on
    };

    /// <summary>Creates JSON serializer options for reading (deserialization).</summary>
    /// <param name="readers">JSON converters to register in the options.</param>
    /// <returns>JSON serializer options for reading (deserialization).</returns>
    public static JsonSerializerOptions CreateReaderOptions(IEnumerable<JsonConverter> readers) {
      var options = JsonUtils.CreateReaderOptions();
      foreach (var reader in readers)
        options.Converters.Add(reader);
      return options;
    }

    /// <summary>Creates JSON serializer options for reading (deserialization).</summary>
    /// <param name="readers">JSON converters to register in the options.</param>
    /// <returns>JSON serializer options for reading (deserialization).</returns>
    public static JsonSerializerOptions CreateReaderOptions(params JsonConverter[] readers)
      => JsonUtils.CreateReaderOptions((IEnumerable<JsonConverter>) readers);

    /// <summary>Creates JSON serializer options for reading (deserialization).</summary>
    /// <param name="readers">JSON converters to register in the options.</param>
    /// <param name="moreReaders">More JSON converters to register in the options.</param>
    /// <returns>JSON serializer options for reading (deserialization).</returns>
    public static JsonSerializerOptions CreateReaderOptions(IEnumerable<JsonConverter> readers, params JsonConverter[] moreReaders)
      => JsonUtils.CreateReaderOptions(readers.Concat(moreReaders));

    /// <summary>Creates JSON serializer options for writing (serialization).</summary>
    /// <returns>JSON serializer options for writing (serialization).</returns>
    public static JsonSerializerOptions CreateWriterOptions() => new JsonSerializerOptions {
      // @formatter:off
      IgnoreNullValues         = false,
      IgnoreReadOnlyProperties = false,
      // @formatter:on
#if DEBUG
      WriteIndented = true,
#else
      WriteIndented = false,
#endif
    };

    /// <summary>Creates JSON serializer options for writing (serialization).</summary>
    /// <param name="writers">JSON converters to register in the options.</param>
    /// <returns>JSON serializer options for writing (serialization).</returns>
    public static JsonSerializerOptions CreateWriterOptions(IEnumerable<JsonConverter> writers) {
      var options = JsonUtils.CreateWriterOptions();
      foreach (var reader in writers)
        options.Converters.Add(reader);
      return options;
    }

    /// <summary>Creates JSON serializer options for writing (serialization).</summary>
    /// <param name="writers">JSON converters to register in the options.</param>
    /// <returns>JSON serializer options for writing (serialization).</returns>
    public static JsonSerializerOptions CreateWriterOptions(params JsonConverter[] writers)
      => JsonUtils.CreateWriterOptions((IEnumerable<JsonConverter>) writers);

    /// <summary>Creates JSON serializer options for writing (serialization).</summary>
    /// <param name="writers">JSON converters to register in the options.</param>
    /// <param name="moreWriters">More JSON converters to register in the options.</param>
    /// <returns>JSON serializer options for writing (serialization).</returns>
    public static JsonSerializerOptions CreateWriterOptions(IEnumerable<JsonConverter> writers, params JsonConverter[] moreWriters)
      => JsonUtils.CreateWriterOptions(writers.Concat(moreWriters));

    #endregion

    #region Read Operations

    /// <summary>Reads and converts JSON to an object of type <typeparamref name="T"/>.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>The object of type <typeparamref name="T"/> that was read.</returns>
    /// <typeparam name="T">The type to read.</typeparam>
    public static T GetObject<T>(this ref Utf8JsonReader reader, JsonConverter<T> converter, JsonSerializerOptions options)
      => converter.Read(ref reader, typeof(T), options);

    /// <summary>Reads and converts JSON to an appropriate object.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>The object that was read (using <see cref="AnyObjectReader"/>.</returns>
    public static object GetObject(this ref Utf8JsonReader reader, JsonSerializerOptions options)
      => AnyObjectReader.Instance.Read(ref reader, typeof(object), options);

    /// <summary>Reads and converts JSON to an object of type <typeparamref name="T"/>.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>The object of type <typeparamref name="T"/> that was read.</returns>
    /// <typeparam name="T">The type to read.</typeparam>
    public static T GetObject<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
      => JsonSerializer.Deserialize<T>(ref reader, options);

    /// <summary>Reads and converts JSON to a boolean value, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The boolean value that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public static bool? GetOptionalBoolean(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (bool?) null : reader.GetBoolean();

    /// <summary>Reads and converts JSON to an 8-bit unsigned integer, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The 8-bit unsigned integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public static byte? GetOptionalByte(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (byte?) null : reader.GetByte();

    /// <summary>Reads and converts JSON to a <see cref="DateTimeOffset"/>, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The <see cref="DateTimeOffset"/> that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public static DateTimeOffset? GetOptionalDateTimeOffset(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (DateTimeOffset?) null : reader.GetDateTimeOffset();

    /// <summary>Reads and converts JSON to a decimal value, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The decimal value that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public static decimal? GetOptionalDecimal(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (decimal?) null : reader.GetDecimal();

    /// <summary>Reads and converts JSON to a double-precision floating-point value, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>
    /// The double-precision floating-point value that was read, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    public static double? GetOptionalDouble(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (double?) null : reader.GetDouble();

    /// <summary>Reads and converts JSON to a <see cref="Guid">UUID</see>, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The <see cref="Guid">UUID</see> that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public static Guid? GetOptionalGuid(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (Guid?) null : reader.GetGuid();

    /// <summary>Reads and converts JSON to a 16-bit signed integer, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The 16-bit signed integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public static short? GetOptionalInt16(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (short?) null : reader.GetInt16();

    /// <summary>Reads and converts JSON to a 32-bit signed integer, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The 32-bit signed integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public static int? GetOptionalInt32(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (int?) null : reader.GetInt32();

    /// <summary>Reads and converts JSON to a 64-bit signed integer, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The 64-bit signed integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public static long? GetOptionalInt64(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (long?) null : reader.GetInt64();

    /// <summary>Reads and converts JSON to an object of type <typeparamref name="T"/>, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>
    /// The object of type <typeparamref name="T"/> that was read, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    /// <typeparam name="T">The reference type to read.</typeparam>
    public static T? GetOptionalObject<T>(this ref Utf8JsonReader reader, JsonConverter<T> converter, JsonSerializerOptions options)
      where T : class
      => (reader.TokenType == JsonTokenType.Null) ? null : reader.GetObject(converter, options);

    /// <summary>Reads and converts JSON to an appropriate object, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// The object that was read (using <see cref="AnyObjectReader"/>, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    public static object? GetOptionalObject(this ref Utf8JsonReader reader, JsonSerializerOptions options)
      => (reader.TokenType == JsonTokenType.Null) ? null : reader.GetObject(options);

    /// <summary>Reads and converts JSON to an object of type <typeparamref name="T"/>, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// The object of type <typeparamref name="T"/> that was read, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    /// <typeparam name="T">The reference type to read.</typeparam>
    public static T? GetOptionalObject<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
      where T : class
      => (reader.TokenType == JsonTokenType.Null) ? null : reader.GetObject<T>(options);

    /// <summary>Reads and converts JSON to an 8-bit signed integer, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The 8-bit signed integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    [CLSCompliant(false)]
    public static sbyte? GetOptionalSByte(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (sbyte?) null : reader.GetSByte();

    /// <summary>Reads and converts JSON to a single-precision floating-point value, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>
    /// The single-precision floating-point value that was read, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    public static float? GetOptionalSingle(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (float?) null : reader.GetSingle();

    /// <summary>Reads and converts JSON to a 16-bit unsigned integer, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The 16-bit unsigned integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    [CLSCompliant(false)]
    public static ushort? GetOptionalUInt16(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (ushort?) null : reader.GetUInt16();

    /// <summary>Reads and converts JSON to a 32-bit unsigned integer, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The 32-bit unsigned integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    [CLSCompliant(false)]
    public static uint? GetOptionalUInt32(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (uint?) null : reader.GetUInt32();

    /// <summary>Reads and converts JSON to a 64-bit unsigned integer, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The 64-bit unsigned integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    [CLSCompliant(false)]
    public static ulong? GetOptionalUInt64(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? (ulong?) null : reader.GetUInt64();

    /// <summary>
    /// Takes the next JSON token value from the specified reader and parses it as an absolute <see cref="Uri">URI</see>.
    /// </summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The URI, if the entire UTF-8 encoded token value was successfully parsed.</returns>
    /// <exception cref="JsonException">
    /// When the current JSON token value is not a string, or could not be parsed as an absolute <see cref="Uri">URI</see>.
    /// </exception>
    public static Uri? GetOptionalUri(this ref Utf8JsonReader reader)
      => (reader.TokenType == JsonTokenType.Null) ? null : reader.GetUri();

    /// <summary>Reads and converts JSON to a value of type <typeparamref name="T"/>, allowing null.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>
    /// The (nullable) value of type <typeparamref name="T"/> that was read, or <see langword="null"/> if a JSON null value was
    /// found.
    /// </returns>
    /// <typeparam name="T">The value type to read.</typeparam>
    public static T? GetOptionalValue<T>(this ref Utf8JsonReader reader, JsonConverter<T> converter, JsonSerializerOptions options)
      where T : struct
      => (reader.TokenType == JsonTokenType.Null) ? (T?) null : converter.Read(ref reader, typeof(T), options);

    /// <summary>Decodes the current raw JSON value as a string.</summary>
    /// <param name="reader">The UTF-8 JSON reader to get the raw value from.</param>
    /// <returns>The raw value as a string.</returns>
    public static string GetRawStringValue(this ref Utf8JsonReader reader) {
      var value = "";
      if (reader.HasValueSequence) {
        foreach (var memory in reader.ValueSequence)
          value += JsonUtils.DecodeUtf8(memory.Span);
      }
      else
        value = JsonUtils.DecodeUtf8(reader.ValueSpan);
      return value;
    }

    /// <summary>
    /// Takes the next JSON token value from the specified reader and parses it as an absolute <see cref="Uri">URI</see>.
    /// </summary>
    /// <param name="reader">The reader to use.</param>
    /// <returns>The URI, if the entire UTF-8 encoded token value was successfully parsed.</returns>
    /// <exception cref="JsonException">
    /// When the current JSON token value is not a string, or could not be parsed as an absolute <see cref="Uri">URI</see>.
    /// </exception>
    public static Uri GetUri(this ref Utf8JsonReader reader) {
      if (reader.TryGetUri(out var uri))
        return uri ?? throw new JsonException("Expected a URI but received null.");
      throw new JsonException($"Expected a URI but received a JSON token of type '{reader.TokenType}' ({reader.GetRawStringValue()}).");
    }

    /// <summary>Reads and converts JSON to a value of type <typeparamref name="T"/>.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>The value of type <typeparamref name="T"/> that was read.</returns>
    /// <typeparam name="T">The value type to read.</typeparam>
    public static T GetValue<T>(this ref Utf8JsonReader reader, JsonConverter<T> converter, JsonSerializerOptions options)
      where T : struct
      => converter.Read(ref reader, typeof(T), options);

    /// <summary>Reads and converts JSON to a (read-only) dictionary.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) dictionary of containing the key/value pairs read, or <see langword="null"/> if the value was specified as
    /// <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the dictionary.</typeparam>
    public static IReadOnlyDictionary<string, T>? ReadDictionary<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
      => reader.ReadDictionary<T, T>(options);

    /// <summary>Reads and converts JSON to a (read-only) dictionary.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) dictionary of containing the key/value pairs read, or <see langword="null"/> if the value was specified as
    /// <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the dictionary.</typeparam>
    /// <typeparam name="TValue">The type to use when deserializing the dictionary values.</typeparam>
    public static IReadOnlyDictionary<string, T>? ReadDictionary<T, TValue>(this ref Utf8JsonReader reader, JsonSerializerOptions options) where TValue : T {
      if (reader.TokenType == JsonTokenType.Null)
        return null;
      if (reader.TokenType != JsonTokenType.StartObject)
        throw new JsonException("Expected start of dictionary not found.");
      reader.Read();
      var elements = new Dictionary<string, T>();
      while (reader.TokenType != JsonTokenType.EndObject) {
        if (reader.TokenType != JsonTokenType.PropertyName)
          throw new JsonException("Expected key value not found.");
        var key = reader.GetString();
        reader.Read();
        elements.Add(key, JsonSerializer.Deserialize<TValue>(ref reader, options));
        reader.Read();
      }
      return elements;
    }

    /// <summary>Reads and converts JSON to a (read-only) dictionary.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) dictionary of containing the key/value pairs read, or <see langword="null"/> if the value was specified as
    /// <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the dictionary.</typeparam>
    public static IReadOnlyDictionary<string, T>? ReadDictionary<T>(this ref Utf8JsonReader reader, JsonConverter<T> converter, JsonSerializerOptions options)
      => reader.ReadDictionary<T, T>(converter, options);

    /// <summary>Reads and converts JSON to a (read-only) dictionary.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) dictionary of containing the key/value pairs read, or <see langword="null"/> if the value was specified as
    /// <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the dictionary.</typeparam>
    /// <typeparam name="TValue">The type to use when deserializing the dictionary values.</typeparam>
    public static IReadOnlyDictionary<string, T>? ReadDictionary<T, TValue>(this ref Utf8JsonReader reader, JsonConverter<TValue> converter, JsonSerializerOptions options) where TValue : T {
      if (reader.TokenType == JsonTokenType.Null)
        return null;
      if (reader.TokenType != JsonTokenType.StartObject)
        throw new JsonException("Expected start of dictionary not found.");
      reader.Read();
      var elements = new Dictionary<string, T>();
      while (reader.TokenType != JsonTokenType.EndObject) {
        if (reader.TokenType != JsonTokenType.PropertyName)
          throw new JsonException("Expected key value not found.");
        var key = reader.GetString();
        reader.Read();
        elements.Add(key, converter.Read(ref reader, typeof(TValue), options));
        reader.Read();
      }
      return elements;
    }

    /// <summary>Reads and converts JSON to a (read-only) list.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list containing the values read, or <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public static IReadOnlyList<T>? ReadList<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
      => reader.ReadList<T, T>(options);

    /// <summary>Reads and converts JSON to a (read-only) list.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list containing the values read, or <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the list.</typeparam>
    /// <typeparam name="TValue">The type to use when deserializing the list values.</typeparam>
    public static IReadOnlyList<T>? ReadList<T, TValue>(this ref Utf8JsonReader reader, JsonSerializerOptions options) where TValue : T {
      if (reader.TokenType == JsonTokenType.Null)
        return null;
      if (reader.TokenType != JsonTokenType.StartArray)
        throw new JsonException("Expected start of list not found.");
      reader.Read();
      // Shortcut for empty list
      if (reader.TokenType == JsonTokenType.EndArray)
        return Array.Empty<T>();
      var elements = new List<T>();
      while (reader.TokenType != JsonTokenType.EndArray) {
        elements.Add(JsonSerializer.Deserialize<TValue>(ref reader, options));
        reader.Read();
      }
      return elements;
    }

    /// <summary>Reads and converts JSON to a (read-only) list.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list containing the values read, or <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the list.</typeparam>
    public static IReadOnlyList<T>? ReadList<T>(this ref Utf8JsonReader reader, JsonConverter<T> converter, JsonSerializerOptions options)
      => reader.ReadList<T, T>(converter, options);

    /// <summary>Reads and converts JSON to a (read-only) list.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list containing the elements read, or <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the list.</typeparam>
    /// <typeparam name="TValue">The type to use when deserializing the list values.</typeparam>
    public static IReadOnlyList<T>? ReadList<T, TValue>(this ref Utf8JsonReader reader, JsonConverter<TValue> converter, JsonSerializerOptions options) where TValue : T {
      if (reader.TokenType == JsonTokenType.Null)
        return null;
      if (reader.TokenType != JsonTokenType.StartArray)
        throw new JsonException("Expected start of list not found.");
      reader.Read();
      // Shortcut for empty list
      if (reader.TokenType == JsonTokenType.EndArray)
        return Array.Empty<T>();
      var elements = new List<T>();
      while (reader.TokenType != JsonTokenType.EndArray) {
        elements.Add(converter.Read(ref reader, typeof(TValue), options));
        reader.Read();
      }
      return elements;
    }

    /// <summary>
    /// Tries to parse the given reader's current JSON token value as an absolute <see cref="Uri">URI</see> and returns a value that
    /// indicates whether the operation succeeded.
    /// </summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="value">When this method returns, contains the parsed value.</param>
    /// <returns>
    /// <see langword="true"/> if the entire UTF-8 encoded token value can be successfully parsed as an absolute
    /// <see cref="Uri">URI</see>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryGetUri(this ref Utf8JsonReader reader, out Uri? value)
      => Uri.TryCreate(reader.GetString(), UriKind.Absolute, out value);

    #endregion

    #region Write Operations

    /// <summary>Writes a list of values of type <typeparamref name="T"/> as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="values">The values to write.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public static void WriteList<T>(this Utf8JsonWriter writer, IEnumerable<T> values, JsonSerializerOptions options) {
      writer.WriteStartArray();
      foreach (var value in values)
        JsonSerializer.Serialize(writer, value, options);
      writer.WriteEndArray();
    }

    /// <summary>Writes a list of values of type <typeparamref name="TList"/> as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="values">The values to write.</param>
    /// <param name="converter">The specific converter to use for serialization.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TConverter">The specific type used by the converter.</typeparam>
    public static void WriteList<TList,TConverter>(this Utf8JsonWriter writer, IEnumerable<TList> values, JsonConverter<TConverter> converter, JsonSerializerOptions options) where TList : TConverter {
      writer.WriteStartArray();
      foreach (var value in values)
        converter.Write(writer, value, options);
      writer.WriteEndArray();
    }

    /// <summary>Writes a list of values of type <typeparamref name="T"/> as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="values">The values to write.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <returns>A task that performs the writes.</returns>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public static async Task WriteListAsync<T>(this Utf8JsonWriter writer, IAsyncEnumerable<T> values, JsonSerializerOptions options) {
      writer.WriteStartArray();
      await foreach (var value in values)
        JsonSerializer.Serialize(writer, value, options);
      writer.WriteEndArray();
    }

    /// <summary>Writes a list of values of type <typeparamref name="TList"/> as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="values">The values to write.</param>
    /// <param name="converter">The specific converter to use for serialization.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <returns>A task that performs the writes.</returns>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TConverter">The specific type used by the converter.</typeparam>
    public static async Task WriteListAsync<TList, TConverter>(this Utf8JsonWriter writer, IAsyncEnumerable<TList> values, JsonConverter<TConverter> converter, JsonSerializerOptions options) where TList : TConverter {
      writer.WriteStartArray();
      await foreach (var value in values)
        converter.Write(writer, value, options);
      writer.WriteEndArray();
    }

    #endregion

  }

}
