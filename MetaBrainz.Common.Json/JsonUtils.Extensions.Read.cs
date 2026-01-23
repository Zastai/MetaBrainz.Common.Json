using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using MetaBrainz.Common.Json.Converters;

namespace MetaBrainz.Common.Json;

public static partial class JsonUtils {

  /// <summary>Convenience extension methods on an UTF-8 JSON reader for easy reading of common things.</summary>
  /// <param name="reader">The reader to use.</param>
  extension(ref Utf8JsonReader reader) {

    /// <summary>Reads and converts JSON to an object of type <typeparamref name="T"/>.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>The object of type <typeparamref name="T"/> that was read.</returns>
    /// <typeparam name="T">The type to read.</typeparam>
    public T GetObject<T>(JsonConverter<T> converter, JsonSerializerOptions options)
      => converter.Read(ref reader, typeof(T), options) ?? throw new JsonException("The converter read a null object.");

    /// <summary>Reads and converts JSON to an appropriate object.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>The object that was read (using <see cref="AnyObjectReader"/>.</returns>
    public object GetObject(JsonSerializerOptions options)
      => AnyObjectReader.Instance.Read(ref reader, typeof(object), options);

    /// <summary>Reads and converts JSON to an object of type <typeparamref name="T"/>.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>The object of type <typeparamref name="T"/> that was read.</returns>
    /// <typeparam name="T">The type to read.</typeparam>
    public T GetObject<T>(JsonSerializerOptions options)
      => JsonSerializer.Deserialize<T>(ref reader, options) ?? throw new JsonException("A null object was found.");

    /// <summary>Reads and converts JSON to a boolean value, allowing null.</summary>
    /// <returns>The boolean value that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public bool? GetOptionalBoolean() => reader.TokenType == JsonTokenType.Null ? null : reader.GetBoolean();

    /// <summary>Reads and converts JSON to an 8-bit unsigned integer, allowing null.</summary>
    /// <returns>The 8-bit unsigned integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public byte? GetOptionalByte() => reader.TokenType == JsonTokenType.Null ? null : reader.GetByte();

    /// <summary>Reads and converts JSON to a <see cref="DateTimeOffset"/>, allowing null.</summary>
    /// <returns>The <see cref="DateTimeOffset"/> that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public DateTimeOffset? GetOptionalDateTimeOffset()
      => reader.TokenType == JsonTokenType.Null ? null : reader.GetDateTimeOffset();

    /// <summary>Reads and converts JSON to a decimal value, allowing null.</summary>
    /// <returns>The decimal value that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public decimal? GetOptionalDecimal() => reader.TokenType == JsonTokenType.Null ? null : reader.GetDecimal();

    /// <summary>Reads and converts JSON to a double-precision floating-point value, allowing null.</summary>
    /// <returns>
    /// The double-precision floating-point value that was read, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    public double? GetOptionalDouble() => reader.TokenType == JsonTokenType.Null ? null : reader.GetDouble();

    /// <summary>Reads and converts JSON to a <see cref="Guid">UUID</see>, allowing null.</summary>
    /// <returns>The <see cref="Guid">UUID</see> that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public Guid? GetOptionalGuid() => reader.TokenType == JsonTokenType.Null ? null : reader.GetGuid();

    /// <summary>Reads and converts JSON to a 16-bit signed integer, allowing null.</summary>
    /// <returns>The 16-bit signed integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public short? GetOptionalInt16() => reader.TokenType == JsonTokenType.Null ? null : reader.GetInt16();

    /// <summary>Reads and converts JSON to a 32-bit signed integer, allowing null.</summary>
    /// <returns>The 32-bit signed integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public int? GetOptionalInt32() => reader.TokenType == JsonTokenType.Null ? null : reader.GetInt32();

    /// <summary>Reads and converts JSON to a 64-bit signed integer, allowing null.</summary>
    /// <returns>The 64-bit signed integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public long? GetOptionalInt64() => reader.TokenType == JsonTokenType.Null ? null : reader.GetInt64();

    /// <summary>Reads and converts JSON to an object of type <typeparamref name="T"/>, allowing null.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>
    /// The object of type <typeparamref name="T"/> that was read, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    /// <typeparam name="T">The reference type to read.</typeparam>
    public T? GetOptionalObject<T>(JsonConverter<T> converter, JsonSerializerOptions options) where T : class
      => reader.TokenType == JsonTokenType.Null ? null : reader.GetObject(converter, options);

    /// <summary>Reads and converts JSON to an appropriate object, allowing null.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// The object that was read (using <see cref="AnyObjectReader"/>, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    public object? GetOptionalObject(JsonSerializerOptions options)
      => reader.TokenType == JsonTokenType.Null ? null : reader.GetObject(options);

    /// <summary>Reads and converts JSON to an object of type <typeparamref name="T"/>, allowing null.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// The object of type <typeparamref name="T"/> that was read, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    /// <typeparam name="T">The reference type to read.</typeparam>
    public T? GetOptionalObject<T>(JsonSerializerOptions options) where T : class
      => reader.TokenType == JsonTokenType.Null ? null : reader.GetObject<T>(options);

    /// <summary>Reads and converts JSON to an 8-bit signed integer, allowing null.</summary>
    /// <returns>The 8-bit signed integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public sbyte? GetOptionalSByte() => reader.TokenType == JsonTokenType.Null ? null : reader.GetSByte();

    /// <summary>Reads and converts JSON to a single-precision floating-point value, allowing null.</summary>
    /// <returns>
    /// The single-precision floating-point value that was read, or <see langword="null"/> if a JSON null value was found.
    /// </returns>
    public float? GetOptionalSingle() => reader.TokenType == JsonTokenType.Null ? null : reader.GetSingle();

    /// <summary>Reads and converts JSON to a 16-bit unsigned integer, allowing null.</summary>
    /// <returns>The 16-bit unsigned integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public ushort? GetOptionalUInt16() => reader.TokenType == JsonTokenType.Null ? null : reader.GetUInt16();

    /// <summary>Reads and converts JSON to a 32-bit unsigned integer, allowing null.</summary>
    /// <returns>The 32-bit unsigned integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public uint? GetOptionalUInt32() => reader.TokenType == JsonTokenType.Null ? null : reader.GetUInt32();

    /// <summary>Reads and converts JSON to a 64-bit unsigned integer, allowing null.</summary>
    /// <returns>The 64-bit unsigned integer that was read, or <see langword="null"/> if a JSON null value was found.</returns>
    public ulong? GetOptionalUInt64() => reader.TokenType == JsonTokenType.Null ? null : reader.GetUInt64();

    /// <summary>
    /// Takes the next JSON token value from the specified reader and parses it as an absolute <see cref="Uri">URI</see>.
    /// </summary>
    /// <returns>The URI, if the entire UTF-8 encoded token value was successfully parsed.</returns>
    /// <exception cref="JsonException">
    /// When the current JSON token value is not a string, or could not be parsed as an absolute <see cref="Uri">URI</see>.
    /// </exception>
    public Uri? GetOptionalUri() => reader.TokenType == JsonTokenType.Null ? null : reader.GetUri();

    /// <summary>Reads and converts JSON to a value of type <typeparamref name="T"/>, allowing null.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>
    /// The (nullable) value of type <typeparamref name="T"/> that was read, or <see langword="null"/> if a JSON null value was
    /// found.
    /// </returns>
    /// <typeparam name="T">The value type to read.</typeparam>
    public T? GetOptionalValue<T>(JsonConverter<T> converter, JsonSerializerOptions options) where T : struct
      => reader.TokenType == JsonTokenType.Null ? null : converter.Read(ref reader, typeof(T), options);

    /// <summary>Gets the value for a property name node.</summary>
    /// <returns>The property name node's value.</returns>
    public string GetPropertyName()
      => reader.GetString() ?? throw new JsonException("Reader returned null for a PropertyName token.");

    /// <summary>Decodes the current raw JSON value as a string.</summary>
    /// <returns>The raw value as a string.</returns>
    public string GetRawStringValue() {
      var value = "";
      if (reader.HasValueSequence) {
        foreach (var memory in reader.ValueSequence) {
          value += Encoding.UTF8.GetString(memory.Span);
        }
      }
      else {
        value = Encoding.UTF8.GetString(reader.ValueSpan);
      }
      return value;
    }

    /// <summary>Gets the value for a string node.</summary>
    /// <returns>The string node's value.</returns>
    public string GetStringValue() => reader.GetString() ?? throw new JsonException("Reader returned null for a String token.");

    /// <summary>
    /// Takes the next JSON token value from the specified reader and parses it as an absolute <see cref="Uri">URI</see>.
    /// </summary>
    /// <returns>The URI, if the entire UTF-8 encoded token value was successfully parsed.</returns>
    /// <exception cref="JsonException">
    /// When the current JSON token value is not a string, or could not be parsed as an absolute <see cref="Uri">URI</see>.
    /// </exception>
    public Uri GetUri() {
      if (reader.TryGetUri(out var uri)) {
        return uri ?? throw new JsonException("Expected a URI but received null.");
      }
      var message = $"Expected a URI but received a JSON token of type '{reader.TokenType}' ({reader.GetRawStringValue()}).";
      throw new JsonException(message);
    }

    /// <summary>Reads and converts JSON to a value of type <typeparamref name="T"/>.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <returns>The value of type <typeparamref name="T"/> that was read.</returns>
    /// <typeparam name="T">The value type to read.</typeparam>
    public T GetValue<T>(JsonConverter<T> converter, JsonSerializerOptions options) where T : struct
      => converter.Read(ref reader, typeof(T), options);

    /// <summary>Reads and converts JSON to a (read-only) dictionary.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) dictionary of containing the key/value pairs read, or <see langword="null"/> if the value was specified as
    /// <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the dictionary.</typeparam>
    public IReadOnlyDictionary<string, T>? ReadDictionary<T>(JsonSerializerOptions options) => reader.ReadDictionary<T, T>(options);

    /// <summary>Reads and converts JSON to a (read-only) dictionary.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) dictionary of containing the key/value pairs read, or <see langword="null"/> if the value was specified as
    /// <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the dictionary.</typeparam>
    /// <typeparam name="TValue">The type to use when deserializing the dictionary values.</typeparam>
    public IReadOnlyDictionary<string, T>? ReadDictionary<T, TValue>(JsonSerializerOptions options) where TValue : T {
      if (reader.TokenType == JsonTokenType.Null) {
        return null;
      }
      if (reader.TokenType != JsonTokenType.StartObject) {
        throw new JsonException("Expected start of dictionary not found.");
      }
      reader.Read();
      var elements = new Dictionary<string, T>();
      while (reader.TokenType != JsonTokenType.EndObject) {
        if (reader.TokenType != JsonTokenType.PropertyName) {
          throw new JsonException("Expected key value not found.");
        }
        var key = reader.GetPropertyName();
        reader.Read();
        var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        elements.Add(key, value ?? throw new JsonException("A dictionary value was null."));
        reader.Read();
      }
      return elements;
    }

    /// <summary>Reads and converts JSON to a (read-only) dictionary.</summary>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) dictionary of containing the key/value pairs read, or <see langword="null"/> if the value was specified as
    /// <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the dictionary.</typeparam>
    public IReadOnlyDictionary<string, T>? ReadDictionary<T>(JsonConverter<T> converter, JsonSerializerOptions options)
      => reader.ReadDictionary<T, T>(converter, options);

    /// <summary>Reads and converts JSON to a (read-only) dictionary.</summary>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) dictionary of containing the key/value pairs read, or <see langword="null"/> if the value was specified as
    /// <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the dictionary.</typeparam>
    /// <typeparam name="TValue">The type to use when deserializing the dictionary values.</typeparam>
    public IReadOnlyDictionary<string, T>? ReadDictionary<T, TValue>(JsonConverter<TValue> converter,
                                                                     JsonSerializerOptions options) where TValue : T {
      if (reader.TokenType == JsonTokenType.Null) {
        return null;
      }
      if (reader.TokenType != JsonTokenType.StartObject) {
        throw new JsonException("Expected start of dictionary not found.");
      }
      reader.Read();
      var elements = new Dictionary<string, T>();
      while (reader.TokenType != JsonTokenType.EndObject) {
        if (reader.TokenType != JsonTokenType.PropertyName) {
          throw new JsonException("Expected key value not found.");
        }
        var key = reader.GetPropertyName();
        reader.Read();
        var value = converter.Read(ref reader, typeof(TValue), options);
        elements.Add(key, value ?? throw new JsonException("A dictionary value was null."));
        reader.Read();
      }
      return elements;
    }

    /// <summary>Reads and converts JSON to a (read-only) list.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list containing the values read, or <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public IReadOnlyList<T>? ReadList<T>(JsonSerializerOptions options) => reader.ReadList<T, T>(options);

    /// <summary>Reads and converts JSON to a (read-only) list.</summary>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list containing the values read, or <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the list.</typeparam>
    /// <typeparam name="TValue">The type to use when deserializing the list values.</typeparam>
    public IReadOnlyList<T>? ReadList<T, TValue>(JsonSerializerOptions options) where TValue : T {
      if (reader.TokenType == JsonTokenType.Null) {
        return null;
      }
      if (reader.TokenType != JsonTokenType.StartArray) {
        throw new JsonException("Expected start of list not found.");
      }
      reader.Read();
      // Shortcut for empty list
      if (reader.TokenType == JsonTokenType.EndArray) {
        return Array.Empty<T>();
      }
      var elements = new List<T>();
      while (reader.TokenType != JsonTokenType.EndArray) {
        var element = JsonSerializer.Deserialize<TValue>(ref reader, options);
        elements.Add(element ?? throw new JsonException("A list element was null."));
        reader.Read();
      }
      return elements;
    }

    /// <summary>Reads and converts JSON to a (read-only) list.</summary>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list containing the values read, or <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the list.</typeparam>
    public IReadOnlyList<T>? ReadList<T>(JsonConverter<T> converter, JsonSerializerOptions options)
      => reader.ReadList<T, T>(converter, options);

    /// <summary>Reads and converts JSON to a (read-only) list.</summary>
    /// <param name="converter">The specific converter to use for deserialization.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// A (read-only) list containing the elements read, or <see langword="null"/> if the value was specified as <c>null</c>.
    /// </returns>
    /// <typeparam name="T">The value type for the list.</typeparam>
    /// <typeparam name="TValue">The type to use when deserializing the list values.</typeparam>
    public IReadOnlyList<T>? ReadList<T, TValue>(JsonConverter<TValue> converter, JsonSerializerOptions options) where TValue : T {
      if (reader.TokenType == JsonTokenType.Null) {
        return null;
      }
      if (reader.TokenType != JsonTokenType.StartArray) {
        throw new JsonException("Expected start of list not found.");
      }
      reader.Read();
      // Shortcut for empty list
      if (reader.TokenType == JsonTokenType.EndArray) {
        return Array.Empty<T>();
      }
      var elements = new List<T>();
      while (reader.TokenType != JsonTokenType.EndArray) {
        var element = converter.Read(ref reader, typeof(TValue), options);
        elements.Add(element ?? throw new JsonException("A list element was null."));
        reader.Read();
      }
      return elements;
    }

    /// <summary>
    /// Tries to parse the given reader's current JSON token value as an absolute <see cref="Uri">URI</see> and returns a value that
    /// indicates whether the operation succeeded.
    /// </summary>
    /// <param name="value">When this method returns, contains the parsed value.</param>
    /// <returns>
    /// <see langword="true"/> if the entire UTF-8 encoded token value can be successfully parsed as an absolute
    /// <see cref="Uri">URI</see>; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryGetUri(out Uri? value) => Uri.TryCreate(reader.GetString(), UriKind.Absolute, out value);

  }

}
