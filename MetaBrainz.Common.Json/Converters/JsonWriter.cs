using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json.Converters {

  /// <summary>A <see cref="JsonConverter{T}"/> that only supports serialization.</summary>
  /// <typeparam name="T">The type for which serialization is implemented by this converter.</typeparam>
  [PublicAPI]
  public abstract class JsonWriter<T> : JsonConverter<T> {

    /// <summary>Not supported.</summary>
    /// <param name="reader">Ignored.</param>
    /// <param name="typeToConvert">Ignored.</param>
    /// <param name="options">Ignored.</param>
    /// <returns>Never returns.</returns>
    /// <exception cref="NotSupportedException">Always.</exception>
    public sealed override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      => throw new NotSupportedException("This converter is for serialization only.");

  }

}
