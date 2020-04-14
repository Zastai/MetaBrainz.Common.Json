using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

    /// <summary>Writes a list of values as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="values">The values to write.</param>
    /// <param name="options">The options to use for serialization.</param>
    public void WriteList(Utf8JsonWriter writer, IEnumerable<T> values, JsonSerializerOptions options) {
      if (values == null) {
        writer.WriteNullValue();
        return;
      }
      writer.WriteStartArray();
      foreach (var value in values)
        this.Write(writer, value, options);
      writer.WriteEndArray();
    }

    /// <summary>Writes a list of values as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="values">The values to write.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <returns>A task that performs the writes.</returns>
    public async Task WriteListAsync(Utf8JsonWriter writer, IAsyncEnumerable<T> values, JsonSerializerOptions options) {
      if (values == null) {
        writer.WriteNullValue();
        return;
      }
      writer.WriteStartArray();
      await foreach (var value in values)
        this.Write(writer, value, options);
      writer.WriteEndArray();
    }

  }

}
