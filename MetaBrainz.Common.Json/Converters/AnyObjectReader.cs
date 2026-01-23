using System;
using System.Collections.Generic;
using System.Text.Json;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json.Converters;

/// <summary>A JSON reader that handles fields of type <see cref="object"/> using the most appropriate framework type.</summary>
[PublicAPI]
public sealed class AnyObjectReader : JsonReader<object> {

  /// <summary>A global instance, for easy use without unnecessary object allocation.</summary>
  /// <remarks>This reader is stateless, so this single instance can be used everywhere.</remarks>
  public static readonly AnyObjectReader Instance = new();

  /// <summary>Reads and converts JSON to the most appropriate .NET framework type.</summary>
  /// <param name="reader">The reader to read from.</param>
  /// <param name="options">The options to use for deserialization.</param>
  /// <param name="allowNullInArrays">
  /// Specifies whether <see langword="null"/> should be allowed as an element in any arrays nested in the returned object.
  /// </param>
  /// <param name="allowNullAsPropertyValue">
  /// Specifies whether <see langword="null"/> should be allowed as a property value anywhere in the returned object.
  /// </param>
  /// <returns>
  /// The object converted from JSON. The specific type depends on the JSON token; see the table below.
  /// <list type="table">
  ///   <listheader>
  ///     <term>JSON Token</term>
  ///     <description>Mapped Object Type</description>
  ///   </listheader>
  ///   <item>
  ///     <term><c>null</c></term>
  ///     <description>
  ///       <see cref="object"/> (<see langword="null"/>).<br/>
  ///       Note: only when <see langword="null"/> is allowed via <paramref name="allowNullInArrays"/> or
  ///       <paramref name="allowNullAsPropertyValue"/>.<br/>
  ///       The top-level value is never allowed to be <see langword="null"/>; to allow that, use <see cref="OptionalObjectReader"/>
  ///       instead.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term><c>true</c></term>
  ///     <description><see cref="bool"/> (<see langword="true"/>).</description>
  ///   </item>
  ///   <item>
  ///     <term><c>false</c></term>
  ///     <description><see cref="bool"/> (<see langword="false"/>).</description>
  ///   </item>
  ///   <item>
  ///     <term>a string</term>
  ///     <description>
  ///       <para>If it can be recognized as a date/time value: <see cref="DateTimeOffset"/>.</para>
  ///       <para>If it can be recognized as a UUID: <see cref="Guid"/>.</para>
  ///       <para>If it can be recognized as an absolute URI: <see cref="Uri"/>.</para>
  ///       <para>Otherwise: <see cref="string"/>.</para>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>a number</term>
  ///     <description>
  ///       <para>If an integral value that fits in a 32-bit signed integer: <see cref="int"/>.</para>
  ///       <para>If an integral value that fits in a 64-bit signed integer: <see cref="long"/>.</para>
  ///       <para>If an integral value that fits in a 64-bit unsigned integer: <see cref="ulong"/>.</para>
  ///       <para>If a value that fits in a .NET decimal: <see cref="decimal"/>.</para>
  ///       <para>
  ///         Otherwise, if it can be represented as a 64-bit double-precision floating point value: <see cref="double"/>.
  ///       </para>
  ///       <para>Otherwise: <see cref="string"/>.</para>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>an array</term>
  ///     <description>
  ///       <para>If empty: an <see cref="Array.Empty{T}()">empty array</see> of <see cref="object"/>.</para>
  ///       <para>If all elements are the same type (or null, if it's a reference type): an array of that type.</para>
  ///       <para>
  ///       If all elements are the same value type or <see langword="null"/>: an array of a
  ///       <see cref="Nullable{T}">nullable version</see> of that value type.
  ///       </para>
  ///       <para>Otherwise: an array of <see cref="object"/>.</para>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>an object</term>
  ///     <description>
  ///       <see cref="Dictionary{TKey,TValue}"/>, with <see cref="string"/> as key and (nullable) <see cref="object"/> as value.
  ///     </description>
  ///   </item>
  /// </list>
  /// </returns>
  /// <remarks>
  /// Unfortunately, <see cref="Utf8JsonReader.TryGetDecimal"/> will happily truncate decimals, even when a <c>double</c> would have
  /// preserved more digits. The degenerate case (where the <c>decimal</c> would be 0) is detected, but in other cases the less
  /// precise <c>decimal</c> will be used.
  /// </remarks>
  public static object Read(ref Utf8JsonReader reader, JsonSerializerOptions options, bool allowNullInArrays = true,
                            bool allowNullAsPropertyValue = true) => reader.TokenType switch {
    // Easy cases
    JsonTokenType.False => false,
    JsonTokenType.True => true,
    // Cases requiring further deduction
    JsonTokenType.Number => AnyObjectReader.ReadValueFromNumber(ref reader),
    JsonTokenType.String => AnyObjectReader.ReadValueFromString(ref reader),
    JsonTokenType.StartArray
      => AnyObjectReader.ReadArrayOfValues(ref reader, options, allowNullInArrays, allowNullAsPropertyValue),
    JsonTokenType.StartObject
      => AnyObjectReader.ReadObjectAsDictionary(ref reader, options, allowNullInArrays, allowNullAsPropertyValue),
    _ => throw new JsonException($"Token ({reader.TokenType}: {reader.GetRawStringValue()}) cannot be converted to an object.")
  };

  /// <summary>Reads and converts JSON to the most appropriate .NET framework type.</summary>
  /// <param name="reader">The reader to read from.</param>
  /// <param name="typeToConvert">The type of object to convert (ignored; assumed to be <see cref="object"/>).</param>
  /// <param name="options">The options to use for deserialization.</param>
  /// <returns>
  /// The object converted from JSON. The specific type depends on the JSON token; see the table below.
  /// <list type="table">
  ///   <listheader>
  ///     <term>JSON Token</term>
  ///     <description>Mapped Object Type</description>
  ///   </listheader>
  ///   <item>
  ///     <term><c>null</c></term>
  ///     <description>
  ///       <see cref="object"/> (<see langword="null"/>).<br/>
  ///       Note: only for elements in nested arrays and nested property values.<br/>
  ///       The top-level value is never allowed to be <see langword="null"/>; to allow that, use <see cref="OptionalObjectReader"/>
  ///       instead.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term><c>true</c></term>
  ///     <description><see cref="bool"/> (<see langword="true"/>).</description>
  ///   </item>
  ///   <item>
  ///     <term><c>false</c></term>
  ///     <description><see cref="bool"/> (<see langword="false"/>).</description>
  ///   </item>
  ///   <item>
  ///     <term>a string</term>
  ///     <description>
  ///       <para>If it can be recognized as a date/time value: <see cref="DateTimeOffset"/>.</para>
  ///       <para>If it can be recognized as a UUID: <see cref="Guid"/>.</para>
  ///       <para>If it can be recognized as an absolute URI: <see cref="Uri"/>.</para>
  ///       <para>Otherwise: <see cref="string"/>.</para>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>a number</term>
  ///     <description>
  ///       <para>If an integral value that fits in a 32-bit signed integer: <see cref="int"/>.</para>
  ///       <para>If an integral value that fits in a 64-bit signed integer: <see cref="long"/>.</para>
  ///       <para>If an integral value that fits in a 64-bit unsigned integer: <see cref="ulong"/>.</para>
  ///       <para>If a value that fits in a .NET decimal: <see cref="decimal"/>.</para>
  ///       <para>
  ///         Otherwise, if it can be represented as a 64-bit double-precision floating point value: <see cref="double"/>.
  ///       </para>
  ///       <para>Otherwise: <see cref="string"/>.</para>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>an array</term>
  ///     <description>
  ///       <para>If empty: an <see cref="Array.Empty{T}()">empty array</see> of <see cref="object"/>.</para>
  ///       <para>If all elements are the same type (or null, if it's a reference type): an array of that type.</para>
  ///       <para>
  ///       If all elements are the same value type or <see langword="null"/>: an array of a
  ///       <see cref="Nullable{T}">nullable version</see> of that value type.
  ///       </para>
  ///       <para>Otherwise: an array of <see cref="object"/>.</para>
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term>an object</term>
  ///     <description>
  ///       <see cref="Dictionary{TKey,TValue}"/>, with <see cref="string"/> as key and (nullable) <see cref="object"/> as value.
  ///     </description>
  ///   </item>
  /// </list>
  /// </returns>
  /// <remarks>
  /// Unfortunately, <see cref="Utf8JsonReader.TryGetDecimal"/> will happily truncate decimals, even when a <c>double</c> would have
  /// preserved more digits. The degenerate case (where the <c>decimal</c> would be 0) is detected, but in other cases the less
  /// precise <c>decimal</c> will be used.
  /// </remarks>
  public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    => AnyObjectReader.Read(ref reader, options);

  private static object ReadArrayOfValues(ref Utf8JsonReader reader, JsonSerializerOptions options, bool allowNullInArrays,
                                          bool allowNullAsPropertyValue) {
    reader.Read();
    if (reader.TokenType == JsonTokenType.EndArray) {
      return Array.Empty<object?>();
    }
    var elements = new List<object?>();
    while (reader.TokenType != JsonTokenType.EndArray) {
      var element = reader.TokenType switch {
        JsonTokenType.Null when allowNullInArrays => null,
        _ => AnyObjectReader.Read(ref reader, options, allowNullInArrays, allowNullAsPropertyValue)
      };
      elements.Add(element);
      reader.Read();
    }
    try {
      // if all elements are the same type (or null), try to map it to an array of that type
      Type? elementType = null;
      var hasNulls = false;
      foreach (var element in elements) {
        if (element is null) {
          hasNulls = true;
          continue;
        }
        var t = element.GetType();
        // ignore nullability
        t = Nullable.GetUnderlyingType(t) ?? t;
        if (elementType is null) {
          elementType = t;
        }
        else if (elementType != t) {
          elementType = null;
          break;
        }
      }
      if (elementType is not null) {
        if (elementType.IsValueType && hasNulls) {
          // make it nullable
          elementType = typeof(Nullable<>).MakeGenericType(elementType);
        }
        var len = elements.Count;
        var typedArray = Array.CreateInstance(elementType, len);
        for (var i = 0; i < len; ++i) {
          typedArray.SetValue(Convert.ChangeType(elements[i], elementType), i);
        }
        return typedArray;
      }
    }
    catch {
      // No worries, we still have the basic object array to return
    }
    return elements.ToArray();
  }

  private static Dictionary<string, object?> ReadObjectAsDictionary(ref Utf8JsonReader reader, JsonSerializerOptions options,
                                                                    bool allowNullInArrays, bool allowNullAsPropertyValue) {
    reader.Read();
    var obj = new Dictionary<string, object?>();
    while (reader.TokenType != JsonTokenType.EndObject) {
      if (reader.TokenType != JsonTokenType.PropertyName) {
        throw new JsonException($"Expected a JSON object property, but received a {reader.TokenType} token instead.");
      }
      var prop = reader.GetPropertyName();
      if (obj.ContainsKey(prop)) {
        throw new JsonException($"Encountered a duplicate JSON object property ('{prop}').");
      }
      reader.Read();
      obj[prop] = reader.TokenType switch {
        JsonTokenType.Null when allowNullAsPropertyValue => null,
        _ => AnyObjectReader.Read(ref reader, options, allowNullInArrays, allowNullAsPropertyValue)
      };
      reader.Read();
    }
    return obj;
  }

  private static object ReadValueFromNumber(ref Utf8JsonReader reader) {
    if (reader.TryGetInt32(out var i32)) {
      return i32;
    }
    if (reader.TryGetInt64(out var i64)) {
      return i64;
    }
    if (reader.TryGetUInt64(out var u64)) {
      return u64;
    }
    {
      decimal? dec;
      double? fp;
      if (reader.TryGetDecimal(out var decVal)) {
        dec = decVal;
      }
      else {
        dec = null;
      }
      if (reader.TryGetDouble(out var floatVal) && double.IsFinite(floatVal)) {
        fp = floatVal;
      }
      else {
        fp = null;
      }
      if (!dec.HasValue && fp.HasValue) {
        // only double worked -> use it
        return fp;
      }
      if (dec.HasValue && fp.HasValue) {
        // check for a degenerate case: 1E-29 converts successfully to 0.0000000000000000000000000000m
        // FIXME: 12E-29 converts to 0.0000000000000000000000000001m and 1.2E-28; ideally, we'd pick the double then too
        if (dec.Value == 0 && Math.Abs(fp.Value) > double.Epsilon) {
          return fp;
        }
        // otherwise, assume the decimal will be better
        return dec;
      }
      // else: if converted to decimal but not double: should be impossible, so probably bad conversion: use fallback
      // else: if neither decimal nor double: use fallback
    }
    return reader.GetRawStringValue();
  }

  private static object ReadValueFromString(ref Utf8JsonReader reader) {
    // Note: NOT TryGetBytesFromBase64() because that has many false positives (e.g. most ISRC values).
    if (reader.TryGetDateTimeOffset(out var dto)) {
      return dto;
    }
    if (reader.TryGetGuid(out var guid)) {
      return guid;
    }
    var text = reader.GetStringValue();
    if (Uri.TryCreate(text, UriKind.Absolute, out var uri)) {
      return uri;
    }
    // FIXME: Are the other "special" strings we should recognize?
    return text;
  }

}
