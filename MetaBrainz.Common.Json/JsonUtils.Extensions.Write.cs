using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MetaBrainz.Common.Json;

public static partial class JsonUtils {

  /// <summary>Convenience extension methods on an UTF-8 JSON writer for easy writing of common things.</summary>
  /// <param name="writer">The writer to write to.</param>
  extension(Utf8JsonWriter writer) {

    /// <summary>Writes a list of values of type <typeparamref name="T"/> as JSON.</summary>
    /// <param name="values">The values to write.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public void WriteList<T>(IEnumerable<T> values, JsonSerializerOptions options) {
      writer.WriteStartArray();
      foreach (var value in values) {
        JsonSerializer.Serialize(writer, value, options);
      }
      writer.WriteEndArray();
    }

    /// <summary>Writes a list of values of type <typeparamref name="TList"/> as JSON.</summary>
    /// <param name="values">The values to write.</param>
    /// <param name="converter">The specific converter to use for serialization.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TConverter">The specific type used by the converter.</typeparam>
    public void WriteList<TList, TConverter>(IEnumerable<TList> values, JsonConverter<TConverter> converter,
                                             JsonSerializerOptions options) where TList : TConverter {
      writer.WriteStartArray();
      foreach (var value in values) {
        converter.Write(writer, value, options);
      }
      writer.WriteEndArray();
    }

    /// <summary>Writes a list of values of type <typeparamref name="T"/> as JSON.</summary>
    /// <param name="values">The values to write.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <returns>A task that performs the writes.</returns>
    /// <typeparam name="T">The element type for the list.</typeparam>
    public async Task WriteListAsync<T>(IAsyncEnumerable<T> values, JsonSerializerOptions options) {
      writer.WriteStartArray();
      await foreach (var value in values) {
        JsonSerializer.Serialize(writer, value, options);
      }
      writer.WriteEndArray();
    }

    /// <summary>Writes a list of values of type <typeparamref name="TList"/> as JSON.</summary>
    /// <param name="values">The values to write.</param>
    /// <param name="converter">The specific converter to use for serialization.</param>
    /// <param name="options">The options to use for serialization.</param>
    /// <returns>A task that performs the writes.</returns>
    /// <typeparam name="TList">The element type for the list.</typeparam>
    /// <typeparam name="TConverter">The specific type used by the converter.</typeparam>
    public async Task WriteListAsync<TList, TConverter>(IAsyncEnumerable<TList> values, JsonConverter<TConverter> converter,
                                                        JsonSerializerOptions options) where TList : TConverter {
      writer.WriteStartArray();
      await foreach (var value in values) {
        converter.Write(writer, value, options);
      }
      writer.WriteEndArray();
    }

  }

}
