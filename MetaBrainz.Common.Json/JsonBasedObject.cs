using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json {

  /// <summary>A JSON-based object, holding any properties not handled by normal deserialization.</summary>
  [PublicAPI]
  public abstract class JsonBasedObject : IJsonBasedObject {

    IReadOnlyDictionary<string, object?>? IJsonBasedObject.UnhandledProperties => this.UnhandledProperties;

    /// <inheritdoc cref="IJsonBasedObject.UnhandledProperties"/>
    /// <remarks>This is only public because the default processing of <see cref="JsonSerializer"/> requires it.</remarks>
    [JsonExtensionData]
    public Dictionary<string, object?>? UnhandledProperties { get; set; }

  }

}
