using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json {

  /// <summary>Utility class, providing various methods to ease the use of System.Text.Json.</summary>
  [PublicAPI]
  public static class JsonUtils {

    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions {
      // @formatter:off
      AllowTrailingCommas         = false,
      IgnoreNullValues            = false,
      IgnoreReadOnlyProperties    = true,
      PropertyNameCaseInsensitive = false,
      WriteIndented               = true,
      // @formatter:on
    };

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
    public static T Deserialize<T>(string json) {
      return JsonSerializer.Deserialize<T>(json, JsonUtils.Options);
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

    /// <summary>Pretty-prints a JSON string.</summary>
    /// <param name="json">The JSON string to pretty-print.</param>
    /// <returns>An indented version of <paramref name="json"/>.</returns>
    public static string Prettify(string json) {
      try {
        return JsonSerializer.Serialize(JsonDocument.Parse(json).RootElement, JsonUtils.Options);
      }
      catch {
        return json;
      }
    }

    /// <summary>Reads and converts JSON to a (read-only) list of <typeparamref name="T"/>.</summary>
    /// <param name="reader">The reader to get the JSON data from.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// A (read-only) list of <typeparamref name="T"/> containing the elements read, or <see langword="null"/> if the value was
    /// specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public static IReadOnlyList<T>? ReadList<T>(ref Utf8JsonReader reader, JsonSerializerOptions options)
      => JsonUtils.ReadList<T, T>(ref reader, options);

    /// <summary>Reads and converts JSON to a (read-only) list of <typeparamref name="TList"/>.</summary>
    /// <param name="reader">The reader to get the JSON data from.</param>
    /// <param name="options">The serializer options to use.</param>
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
    /// <param name="size">The required size for the list.</param>
    /// <param name="reader">The reader to get the JSON data from.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>A (read-only) list of <typeparamref name="T"/> containing the <paramref name="size"/> elements read.</returns>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public static IReadOnlyList<T> ReadList<T>(int size, ref Utf8JsonReader reader, JsonSerializerOptions options)
      => JsonUtils.ReadList<T, T>(size, ref reader, options);

    /// <summary>Reads and converts JSON to a (read-only) list of <typeparamref name="TList"/>.</summary>
    /// <param name="size">The required size for the list.</param>
    /// <param name="reader">The reader to get the JSON data from.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>A (read-only) list of <typeparamref name="TList"/> containing the <paramref name="size"/> elements read.</returns>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TElement">The type to use when deserializing the elements of the list.</typeparam>
    public static IReadOnlyList<TList> ReadList<TList, TElement>(int size, ref Utf8JsonReader reader, JsonSerializerOptions options) where TElement : TList {
      if (reader.TokenType == JsonTokenType.Null)
        throw new JsonException($"Expected a list of {size} elements, but got a null literal.");
      if (reader.TokenType != JsonTokenType.StartArray)
        throw new JsonException("Expected start of list not found.");
      reader.Read();
      if (reader.TokenType == JsonTokenType.EndArray) {
        if (size == 0)
          return Array.Empty<TList>();
        throw new JsonException($"Expected a list of {size} elements, but got an empty list instead.");
      }
      var list = new TList[size];
      var pos = 0;
      while (pos < size && reader.TokenType != JsonTokenType.EndArray) {
        list[pos++] = JsonSerializer.Deserialize<TElement>(ref reader, options);
        reader.Read();
      }
      if (pos < size)
        throw new JsonException($"Expected a list of {size} elements, but only {pos} elements were present.");
      if (reader.TokenType != JsonTokenType.EndArray)
        throw new JsonException($"Expected a list of {size} elements, but more elements were present.");
      return list;
    }

    /// <summary>Reads and converts JSON to a (read-only) list of <typeparamref name="T"/>.</summary>
    /// <param name="size">The required size for the list, if known.</param>
    /// <param name="reader">The reader to get the JSON data from.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// A (read-only) list of <typeparamref name="T"/>. If <paramref name="size"/> was specified, it will contain exactly that
    /// many items. Otherwise, it can be <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public static IReadOnlyList<T>? ReadList<T>(int? size, ref Utf8JsonReader reader, JsonSerializerOptions options)
      => JsonUtils.ReadList<T, T>(size, ref reader, options);

    /// <summary>Reads and converts JSON to a (read-only) list of <typeparamref name="TList"/>.</summary>
    /// <param name="size">The required size for the list, if known.</param>
    /// <param name="reader">The reader to get the JSON data from.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// A (read-only) list of <typeparamref name="TList"/>. If <paramref name="size"/> was specified, it will contain exactly that
    /// many items. Otherwise, it can be <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TElement">The type to use when deserializing the elements of the list.</typeparam>
    public static IReadOnlyList<TList>? ReadList<TList, TElement>(int? size, ref Utf8JsonReader reader, JsonSerializerOptions options) where TElement : TList {
      return size.HasValue
        ? JsonUtils.ReadList<TList, TElement>(size.Value, ref reader, options)
        : JsonUtils.ReadList<TList, TElement>(ref reader, options);
    }

  }

}
