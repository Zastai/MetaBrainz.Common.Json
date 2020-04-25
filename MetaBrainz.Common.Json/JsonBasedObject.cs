using System.Collections.Generic;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json {

  /// <summary>A JSON-based object, holding any properties not handled by normal deserialization.</summary>
  [PublicAPI]
  public abstract class JsonBasedObject : IJsonBasedObject {

    IReadOnlyDictionary<string, object?>? IJsonBasedObject.UnhandledProperties => this.UnhandledProperties;

    /// <summary>A dictionary containing all properties not otherwise handled.</summary>
    public Dictionary<string, object?>? UnhandledProperties { get; set; }

  }

}
