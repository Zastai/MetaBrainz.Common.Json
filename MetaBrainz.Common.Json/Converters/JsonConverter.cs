using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json.Converters;

/// <summary>
/// A JSON converter that delegates deserialization to a <see cref="JsonReader{T}"/> instance and serialization to a
/// <see cref="JsonWriter{T}"/> instance.
/// </summary>
/// <typeparam name="T">The type to be deserialized and/or serialized by this converter.</typeparam>
/// <typeparam name="TReader">The type of <see cref="JsonReader{T}"/> to use for deserialization.</typeparam>
/// <typeparam name="TWriter">The type of <see cref="JsonWriter{T}"/> to use for serialization.</typeparam>
[PublicAPI]
public class JsonConverter<T, TReader, TWriter> : JsonConverter<T>
  where TReader : JsonReader<T>, new()
  where TWriter : JsonWriter<T>, new() {

  /// <summary>Creates a new JSON converter, using a new default-constructed reader and writer.</summary>
  public JsonConverter() : this(new TReader(), new TWriter()) {
  }

  /// <summary>Creates a new JSON converter, using a specific reader instance and a new default-constructed writer.</summary>
  /// <param name="reader">The reader to use.</param>
  public JsonConverter(TReader reader) : this(reader, new TWriter()) {
  }

  /// <summary>Creates a new JSON converter, using a new default-constructed reader and a specific writer instance.</summary>
  /// <param name="writer">The writer to use.</param>
  public JsonConverter(TWriter writer) : this(new TReader(), writer) {
  }

  /// <summary>Creates a new JSON converter, using specific reader and writer instances.</summary>
  /// <param name="reader">The reader to use.</param>
  /// <param name="writer">The writer to use.</param>
  public JsonConverter(TReader reader, TWriter writer) {
    this.Reader = reader;
    this.Writer = writer;
  }

  private readonly TReader Reader;

  private readonly TWriter Writer;

  /// <summary>Reads and converts JSON to a value of type <typeparamref name="T"/>.</summary>
  /// <param name="reader">The reader to read from.</param>
  /// <param name="typeToConvert">The type of value to convert (ignored; assumed to be <typeparamref name="T"/>).</param>
  /// <param name="options">The options to use for deserialization.</param>
  /// <returns>The value of type <typeparamref name="T"/> that was read and converted.</returns>
  public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    => this.Reader.Read(ref reader, typeToConvert, options);

  /// <summary>Write a value as JSON.</summary>
  /// <param name="writer">The writer to write to.</param>
  /// <param name="value">The value to write.</param>
  /// <param name="options">The options to use for serialization.</param>
  public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    => this.Writer.Write(writer, value, options);

}
