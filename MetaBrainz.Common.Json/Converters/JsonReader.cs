using System;
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

  }

}
