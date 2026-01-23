using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json;

/// <summary>Utility class, providing various methods to ease the use of System.Text.Json.</summary>
[PublicAPI]
public static partial class JsonUtils {

  #region General Utilities

  /// <summary>Deserializes JSON to an object of the specified type, using default options.</summary>
  /// <param name="json">The JSON to deserialize.</param>
  /// <typeparam name="T">The type of object to deserialize.</typeparam>
  /// <returns>A newly deserialized object of type <typeparamref name="T"/>.</returns>
  /// <remarks>The options used match those returned by <see cref="CreateReaderOptions()"/>.</remarks>
  public static T? Deserialize<T>(string json) => JsonUtils.Deserialize<T>(json, JsonUtils.ReaderOptions);

  /// <summary>Deserializes JSON to an object of the specified type.</summary>
  /// <param name="json">The JSON to deserialize.</param>
  /// <param name="options">The options to use for deserialization.</param>
  /// <typeparam name="T">The type of object to deserialize.</typeparam>
  /// <returns>A newly deserialized object of type <typeparamref name="T"/>.</returns>
  public static T? Deserialize<T>(string json, JsonSerializerOptions options) => JsonSerializer.Deserialize<T>(json, options);

  /// <summary>Deserializes JSON to an object of the specified type, using default options.</summary>
  /// <param name="json">The JSON to deserialize.</param>
  /// <typeparam name="T">The type of object to deserialize.</typeparam>
  /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
  /// <returns>A newly deserialized object of type <typeparamref name="T"/>.</returns>
  /// <remarks>The options used match those returned by <see cref="CreateReaderOptions()"/>.</remarks>
  public static ValueTask<T?> DeserializeAsync<T>(Stream json, CancellationToken cancellationToken = default)
    => JsonUtils.DeserializeAsync<T>(json, JsonUtils.ReaderOptions, cancellationToken);

  /// <summary>Deserializes JSON to an object of the specified type.</summary>
  /// <param name="json">The JSON to deserialize.</param>
  /// <param name="options">The options to use for deserialization.</param>
  /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
  /// <typeparam name="T">The type of object to deserialize.</typeparam>
  /// <returns>A newly deserialized object of type <typeparamref name="T"/>.</returns>
  public static ValueTask<T?> DeserializeAsync<T>(Stream json, JsonSerializerOptions options,
                                                  CancellationToken cancellationToken = default)
    => JsonSerializer.DeserializeAsync<T>(json, options, cancellationToken);

  /// <summary>Deserializes an object from the JSON content of an HTTP response.</summary>
  /// <param name="response">The response to process.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <typeparam name="T">The specific type to deserialize.</typeparam>
  /// <returns>The deserialized object.</returns>
  /// <exception cref="JsonException">
  /// When an object of type <typeparamref name="T"/> could not be deserialized from the contents of <paramref name="response"/>.
  /// </exception>
  /// <remarks>The options used match those returned by <see cref="CreateReaderOptions()"/>.</remarks>
  public static Task<T> GetJsonContentAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
    => JsonUtils.GetJsonContentAsync<T>(response, JsonUtils.ReaderOptions, cancellationToken);

  /// <summary>Deserializes an object from the JSON content of an HTTP response.</summary>
  /// <param name="response">The response to process.</param>
  /// <param name="options">The JSON serializer options to apply.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <typeparam name="T">The specific type to deserialize.</typeparam>
  /// <returns>The deserialized object.</returns>
  /// <exception cref="JsonException">
  /// When an object of type <typeparamref name="T"/> could not be deserialized from the contents of <paramref name="response"/>.
  /// </exception>
  public static async Task<T> GetJsonContentAsync<T>(HttpResponseMessage response, JsonSerializerOptions options,
                                                     CancellationToken cancellationToken = default) {
    var content = response.Content;
    var headers = content.Headers;
    var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    await using var _ = stream.ConfigureAwait(false);
    if (stream is null || stream.Length == 0) {
      throw new JsonException("No content received.");
    }
    var characterSet = headers.GetContentEncoding();
    var tracingRequested = JsonUtils.TraceSource.Switch.ShouldTrace(TraceEventType.Verbose);
    T? result;
    if (characterSet == "utf-8" && !tracingRequested) {
      // Directly use the stream
      result = await JsonUtils.DeserializeAsync<T>(stream, options, cancellationToken).ConfigureAwait(false);
    }
    else {
      using var sr = new StreamReader(stream, Encoding.GetEncoding(characterSet), false, 1024, true);
      var json = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
      if (tracingRequested) {
        JsonUtils.TraceSource.TraceEvent(TraceEventType.Verbose, 1, "JSON CONTENT: {0}", JsonUtils.Prettify(json));
      }
      result = JsonUtils.Deserialize<T>(json, options);
    }
    return result ?? throw new JsonException("The received content was null.");
  }

  /// <summary>Pretty-prints a JSON string.</summary>
  /// <param name="json">The JSON string to pretty-print.</param>
  /// <returns>
  /// An indented version of <paramref name="json"/>. If anything goes wrong, <paramref name="json"/> is returned unchanged.
  /// </returns>
  public static string Prettify(string json) {
    try {
      return JsonSerializer.Serialize(JsonDocument.Parse(json).RootElement, JsonUtils.PrettifyOptions);
    }
    catch {
      return json;
    }
  }

  /// <summary>The trace source (named 'MetaBrainz.Common.Json.JsonUtils') used by this class.</summary>
  public static readonly TraceSource TraceSource = new("MetaBrainz.Common.Json.JsonUtils", SourceLevels.Off);

  #endregion

  #region Options

  private static readonly JsonSerializerOptions PrettifyOptions = new() {
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    WriteIndented = true,
  };

  private static readonly JsonSerializerOptions ReaderOptions = JsonUtils.CreateReaderOptions();

  /// <summary>
  /// Indicates whether the options created via <see cref="CreateWriterOptions()"/> and its overloads have the
  /// <see cref="JsonSerializerOptions.WriteIndented"/> property set to <see langword="true"/> by default.
  /// </summary>
  public static bool WriteIndentedByDefault { get; set; }

  /// <summary>Creates JSON serializer options for reading (deserialization).</summary>
  /// <returns>JSON serializer options for reading (deserialization).</returns>
  public static JsonSerializerOptions CreateReaderOptions() => new() {
    AllowTrailingCommas = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    PropertyNameCaseInsensitive = false,
  };

  /// <summary>Creates JSON serializer options for reading (deserialization).</summary>
  /// <param name="readers">JSON converters to register in the options.</param>
  /// <returns>JSON serializer options for reading (deserialization).</returns>
  public static JsonSerializerOptions CreateReaderOptions(IEnumerable<JsonConverter> readers) {
    var options = JsonUtils.CreateReaderOptions();
    foreach (var reader in readers) {
      options.Converters.Add(reader);
    }
    return options;
  }

  /// <summary>Creates JSON serializer options for reading (deserialization).</summary>
  /// <param name="readers">JSON converters to register in the options.</param>
  /// <returns>JSON serializer options for reading (deserialization).</returns>
  public static JsonSerializerOptions CreateReaderOptions(params JsonConverter[] readers)
    => JsonUtils.CreateReaderOptions((IEnumerable<JsonConverter>) readers);

  /// <summary>Creates JSON serializer options for reading (deserialization).</summary>
  /// <param name="readers">JSON converters to register in the options.</param>
  /// <param name="moreReaders">More JSON converters to register in the options.</param>
  /// <returns>JSON serializer options for reading (deserialization).</returns>
  public static JsonSerializerOptions CreateReaderOptions(IEnumerable<JsonConverter> readers, params JsonConverter[] moreReaders)
    => JsonUtils.CreateReaderOptions(readers.Concat(moreReaders));

  /// <summary>Creates JSON serializer options for writing (serialization).</summary>
  /// <returns>JSON serializer options for writing (serialization).</returns>
  public static JsonSerializerOptions CreateWriterOptions() => new() {
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    IgnoreReadOnlyProperties = false,
    WriteIndented = JsonUtils.WriteIndentedByDefault,
  };

  /// <summary>Creates JSON serializer options for writing (serialization).</summary>
  /// <param name="writers">JSON converters to register in the options.</param>
  /// <returns>JSON serializer options for writing (serialization).</returns>
  public static JsonSerializerOptions CreateWriterOptions(IEnumerable<JsonConverter> writers) {
    var options = JsonUtils.CreateWriterOptions();
    foreach (var reader in writers) {
      options.Converters.Add(reader);
    }
    return options;
  }

  /// <summary>Creates JSON serializer options for writing (serialization).</summary>
  /// <param name="writers">JSON converters to register in the options.</param>
  /// <returns>JSON serializer options for writing (serialization).</returns>
  public static JsonSerializerOptions CreateWriterOptions(params JsonConverter[] writers)
    => JsonUtils.CreateWriterOptions((IEnumerable<JsonConverter>) writers);

  /// <summary>Creates JSON serializer options for writing (serialization).</summary>
  /// <param name="writers">JSON converters to register in the options.</param>
  /// <param name="moreWriters">More JSON converters to register in the options.</param>
  /// <returns>JSON serializer options for writing (serialization).</returns>
  public static JsonSerializerOptions CreateWriterOptions(IEnumerable<JsonConverter> writers, params JsonConverter[] moreWriters)
    => JsonUtils.CreateWriterOptions(writers.Concat(moreWriters));

  #endregion

}
