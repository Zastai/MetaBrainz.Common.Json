using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json.Converters {

  /// <summary>A <see cref="JsonConverter{T}"/> that only supports deserialization.</summary>
  /// <typeparam name="T">The type for which deserialization is implemented by this converter.</typeparam>
  [PublicAPI]
  public abstract class JsonReader<T> : JsonConverter<T> {

    /// <summary>Not supported.</summary>
    /// <param name="writer">Ignored.</param>
    /// <param name="value">Ignored.</param>
    /// <param name="options">Ignored.</param>
    /// <exception cref="NotSupportedException">Always.</exception>
    public sealed override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
      => throw new NotSupportedException("This converter is for deserialization only.");

    /// <summary>Reads and converts JSON to a (read-only) list of values of type <typeparamref name="T"/>.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="typeToConvert">The type of value to convert (ignored; assumed to be <typeparamref name="T"/>).</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>
    /// The objects of type <typeparamref name="T"/> that were read and converted, or <see langword="null"/> if the JSON contained a
    /// null literal instead of a list.
    /// </returns>
    public IReadOnlyList<T>? ReadList(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
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
        elements.Add(this.Read(ref reader, typeToConvert, options));
        reader.Read();
      }
      return elements;
    }

  }

}
