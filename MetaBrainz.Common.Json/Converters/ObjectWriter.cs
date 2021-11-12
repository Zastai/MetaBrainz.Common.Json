using System.Text.Json;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json.Converters;

/// <summary>A <see cref="JsonWriter{T}"/> for objects.</summary>
/// <typeparam name="T">The type for which serialization is implemented by this writer.</typeparam>
[PublicAPI]
public abstract class ObjectWriter<T> : JsonWriter<T> {

  /// <summary>Write an object as JSON.</summary>
  /// <param name="writer">The writer to write to.</param>
  /// <param name="value">The value to write.</param>
  /// <param name="options">The options to use for serialization.</param>
  public sealed override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
    if (value == null) { // Will never be the case for normal JsonSerializer processing, but handle it anyway.
      writer.WriteNullValue();
      return;
    }
    writer.WriteStartObject();
    this.WriteObjectContents(writer, value, options);
    writer.WriteEndObject();
  }

  /// <summary>Write the contents of an object as JSON.</summary>
  /// <param name="writer">The writer to write to.</param>
  /// <param name="value">The value to write the contents of.</param>
  /// <param name="options">The options to use for serialization.</param>
  protected abstract void WriteObjectContents(Utf8JsonWriter writer, T value, JsonSerializerOptions options);

}
