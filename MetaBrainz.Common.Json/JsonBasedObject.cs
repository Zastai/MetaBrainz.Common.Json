using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json {

  /// <summary>A JSON-based object, holding any properties not handled by normal deserialization.</summary>
  [PublicAPI]
  public abstract class JsonBasedObject : IJsonBasedObject {

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, object?>? UnhandledProperties { get; set; }

  }

}
