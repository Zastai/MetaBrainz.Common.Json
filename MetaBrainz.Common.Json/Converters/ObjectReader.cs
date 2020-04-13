using System;
using System.Text.Json;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json.Converters {

  /// <summary>A <see cref="JsonReader{T}"/> for objects.</summary>
  /// <typeparam name="T">The type for which deserialization is implemented by this reader.</typeparam>
  [PublicAPI]
  public abstract class ObjectReader<T> : JsonReader<T> {

    /// <summary>Reads and converts JSON to an object of type <typeparamref name="T"/>.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="typeToConvert">The type of object to convert (ignored; assumed to be <typeparamref name="T"/>).</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>The object of type <typeparamref name="T"/> that was read and converted.</returns>
    public sealed override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      if (reader.TokenType != JsonTokenType.StartObject)
        throw new JsonException("Expected start of object not found.");
      reader.Read();
      var obj = this.ReadObjectContents(ref reader, options);
      if (reader.TokenType != JsonTokenType.EndObject)
        throw new JsonException("Expected end of object not found.");
      return obj;
    }

    /// <summary>Reads and converts the contents of an object of type <typeparamref name="T"/> from JSON.</summary>
    /// <param name="reader">The reader to read JSON data from.</param>
    /// <param name="options">The options to use for deserialization.</param>
    /// <returns>The object of type <typeparamref name="T"/> that was read and converted.</returns>
    protected abstract T ReadObjectContents(ref Utf8JsonReader reader, JsonSerializerOptions options);

  }

}
