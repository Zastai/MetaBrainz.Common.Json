using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json {

  /// <summary>Utility class, providing various methods to ease the use of System.Text.Json.</summary>
  [PublicAPI]
  public static class JsonUtils {

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

    private static string DecodeUtf8(ReadOnlySpan<byte> bytes) {
#if NETSTD_GE_2_1 || NETCORE_GE_2_1
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
    public static T Deserialize<T>(string json) {
      return JsonSerializer.Deserialize<T>(json, JsonUtils.ReaderOptions);
    }

    /// <summary>Deserializes JSON to an object of the specified type, using default options.</summary>
    /// <param name="json">The JSON to deserialize.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <typeparam name="T">The type of object to deserialize.</typeparam>
    /// <returns>A newly deserialized object of type <typeparamref name="T"/>.</returns>
    public static T Deserialize<T>(string json, JsonSerializerOptions options) {
      return JsonSerializer.Deserialize<T>(json, options);
    }

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

    /// <summary>Pretty-prints a JSON string.</summary>
    /// <param name="json">The JSON string to pretty-print.</param>
    /// <returns>
    /// An indented version of <paramref name="json"/>. If anything goes wrong, <paramref name="json"/> is returned unchanged.
    /// </returns>
    public static string Prettify(string json) {
      try {
        return JsonSerializer.Serialize(JsonDocument.Parse(json).RootElement, JsonUtils.WriterOptions);
      }
      catch {
        return json;
      }
    }

    /// <summary>Reads and converts JSON to a (read-only) list of <typeparamref name="T"/>.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list of <typeparamref name="T"/> containing the elements read, or <see langword="null"/> if the value was
    /// specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public static IReadOnlyList<T>? ReadList<T>(ref Utf8JsonReader reader, JsonSerializerOptions options)
      => JsonUtils.ReadList<T, T>(ref reader, options);

    /// <summary>Reads and converts JSON to a (read-only) list of <typeparamref name="TList"/>.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list of <typeparamref name="TList"/> containing the elements read, or <see langword="null"/> if the value was
    /// specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TElement">The type to use when deserializing the elements of the list.</typeparam>
    public static IReadOnlyList<TList>? ReadList<TList, TElement>(ref Utf8JsonReader reader, JsonSerializerOptions options) where TElement : TList {
      if (reader.TokenType == JsonTokenType.Null)
        return null;
      if (reader.TokenType != JsonTokenType.StartArray)
        throw new JsonException("Expected start of list not found.");
      reader.Read();
      // Shortcut for empty list
      if (reader.TokenType == JsonTokenType.EndArray)
        return Array.Empty<TList>();
      var elements = new List<TList>();
      while (reader.TokenType != JsonTokenType.EndArray) {
        elements.Add(JsonSerializer.Deserialize<TElement>(ref reader, options));
        reader.Read();
      }
      return elements;
    }

    /// <summary>Reads and converts JSON to a (read-only) list of <typeparamref name="T"/>.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list of <typeparamref name="T"/> containing the elements read, or <see langword="null"/> if the value was
    /// specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public static IReadOnlyList<T>? ReadList<T>(ref Utf8JsonReader reader, JsonSerializerOptions options, JsonConverter<T> converter)
      => JsonUtils.ReadList<T, T>(ref reader, options, converter);

    /// <summary>Reads and converts JSON to a (read-only) list of <typeparamref name="TList"/>.</summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list of <typeparamref name="TList"/> containing the elements read, or <see langword="null"/> if the value was
    /// specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TElement">The type to use when deserializing the elements of the list.</typeparam>
    public static IReadOnlyList<TList>? ReadList<TList, TElement>(ref Utf8JsonReader reader, JsonSerializerOptions options, JsonConverter<TElement> converter) where TElement : TList {
      if (reader.TokenType == JsonTokenType.Null)
        return null;
      if (reader.TokenType != JsonTokenType.StartArray)
        throw new JsonException("Expected start of list not found.");
      reader.Read();
      // Shortcut for empty list
      if (reader.TokenType == JsonTokenType.EndArray)
        return Array.Empty<TList>();
      var elements = new List<TList>();
      while (reader.TokenType != JsonTokenType.EndArray) {
        elements.Add(converter.Read(ref reader, typeof(TElement), options));
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

    /// <summary>Writes a list of values of type <typeparamref name="T"/> as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="values">The values to write.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public static void WriteList<T>(Utf8JsonWriter writer, IEnumerable<T> values, JsonSerializerOptions options) {
      if (values == null) {
        writer.WriteNullValue();
        return;
      }
      writer.WriteStartArray();
      foreach (var value in values)
        JsonSerializer.Serialize(writer, value, options);
      writer.WriteEndArray();
    }

    /// <summary>Writes a list of values of type <typeparamref name="TList"/> as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="values">The values to write.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <param name="converter">The specific converter to use for serialization.</param>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TConverter">The specific type used by the converter.</typeparam>
    public static void WriteList<TList,TConverter>(Utf8JsonWriter writer, IEnumerable<TList> values, JsonSerializerOptions options, JsonConverter<TConverter> converter) where TList : TConverter {
      if (values == null) {
        writer.WriteNullValue();
        return;
      }
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
    public static async Task WriteListAsync<T>(Utf8JsonWriter writer, IAsyncEnumerable<T> values, JsonSerializerOptions options) {
      if (values == null) {
        writer.WriteNullValue();
        return;
      }
      writer.WriteStartArray();
      await foreach (var value in values)
        JsonSerializer.Serialize(writer, value, options);
      writer.WriteEndArray();
    }

    /// <summary>Writes a list of values of type <typeparamref name="TList"/> as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="values">The values to write.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <param name="converter">The specific converter to use for serialization.</param>
    /// <returns>A task that performs the writes.</returns>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TConverter">The specific type used by the converter.</typeparam>
    public static async Task WriteListAsync<TList, TConverter>(Utf8JsonWriter writer, IAsyncEnumerable<TList> values, JsonSerializerOptions options, JsonConverter<TConverter> converter) where TList : TConverter {
      if (values == null) {
        writer.WriteNullValue();
        return;
      }
      writer.WriteStartArray();
      await foreach (var value in values)
        converter.Write(writer, value, options);
      writer.WriteEndArray();
    }

  }

}
