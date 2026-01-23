using System;
using System.Collections.Generic;
using System.Text.Json;

using JetBrains.Annotations;

namespace MetaBrainz.Common.Json.Converters;

/// <summary>
/// A JSON reader that handles nullable fields of type <see cref="object"/> using the most appropriate framework type.
/// </summary>
[PublicAPI]
public sealed class OptionalObjectReader : JsonReader<object?> {

  /// <summary>A global instance, for easy use without unnecessary object allocation.</summary>
  /// <remarks>This reader is stateless, so this single instance can be used everywhere.</remarks>
  public static readonly OptionalObjectReader Instance = new();

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
  ///       The top-level value is always allowed to be <see langword="null"/>; to disallow that, use <see cref="AnyObjectReader"/>
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
  public static object? Read(ref Utf8JsonReader reader, JsonSerializerOptions options, bool allowNullInArrays = true,
                             bool allowNullAsPropertyValue = true) => reader.TokenType switch {
    JsonTokenType.Null => null,
    _ => AnyObjectReader.Read(ref reader, options, allowNullInArrays, allowNullAsPropertyValue)
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
  ///       The top-level value is always allowed to be <see langword="null"/>; to disallow that, use <see cref="AnyObjectReader"/>
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
  public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    => OptionalObjectReader.Read(ref reader, options);

}
