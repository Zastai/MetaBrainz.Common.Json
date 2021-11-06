# MetaBrainz.Common.Json [![Build Status](https://img.shields.io/appveyor/build/zastai/metabrainz-common-json)](https://ci.appveyor.com/project/Zastai/metabrainz-common-json) [![NuGet Version](https://img.shields.io/nuget/v/MetaBrainz.Common.Json)](https://www.nuget.org/packages/MetaBrainz.Common.Json)

JSON-related helper classes, for use by the other `MetaBrainz.*` packages.

## Release Notes

### v4.0.2 (not yet released)

### v4.0.1 (2021-11-06)

- Changed the license to MIT (from MS-PL)
- Changed target frameworks to `netstandard2.0`, `netstandard2.1`, and `net472`

#### Dependency Updates

- JetBrainz.Annotations → 2021.3.0
- MetaBrainz.Build.Sdk → 1.0.1
- System.Text.Json → 5.0.2

### v4.0.0 (2020-12-23)

- Switch to a NuGet SDK package (MetaBrainz.Build.Sdk) instead of a Git submodule

#### API Additions

- Two non-null variations of `Utf8JsonReader.GetString()` were added:
  - `GetStringValue()` for `String` nodes
  - `GetPropertyName()` for `PropertyName` nodes

#### Dependency Updates

- System.Text.Json → 5.0.0

### v3.0.1 (2020-05-10)

- This makes the output of `Prettify()` pretty again
  - Indented writing was previously only enabled in debug builds, so essentially never when consuming the NuGet package

### v3.0.0 (2020-04-25)

#### API Additions

- New Extension Method Overload: `JsonUtils.GetObject<T>()` without a specific converter to use
- New Extension Method Overload: `JsonUtils.GetOptionalObject<T>()` without a specific converter to use

#### API Changes

- JsonBasedObject: this once again has a regular `Dictionary` as `UnhandledProperties`
  - this allows implementation types to modify the contents after the initial object creation
  - *this is, unfortunately, a breaking change*

### v2.0.0 (2020-04-24)

#### API Additions

- New Method: `JsonUtils.CreateReaderOptions()`, optionally specifying a set of converters to register
- New Method: `JsonUtils.CreateWriterOptions()`, optionally specifying a set of converters to register
- New Method: `JsonUtils.DeserializeAsync()`
- New Extension Method: `JsonUtils.GetObject()`
- New Extension Method: `JsonUtils.GetOptionalBoolean()`
- New Extension Method: `JsonUtils.GetOptionalByte()`
- New Extension Method: `JsonUtils.GetOptionalDateTimeOffset()`
- New Extension Method: `JsonUtils.GetOptionalDecimal()`
- New Extension Method: `JsonUtils.GetOptionalDouble()`
- New Extension Method: `JsonUtils.GetOptionalGuid()`
- New Extension Method: `JsonUtils.GetOptionalInt16()`
- New Extension Method: `JsonUtils.GetOptionalInt32()`
- New Extension Method: `JsonUtils.GetOptionalInt64()`
- New Extension Method: `JsonUtils.GetOptionalObject()`
- New Extension Method: `JsonUtils.GetOptionalSbyte()`
- New Extension Method: `JsonUtils.GetOptionalSingle()`
- New Extension Method: `JsonUtils.GetOptionalUInt16()`
- New Extension Method: `JsonUtils.GetOptionalUInt32()`
- New Extension Method: `JsonUtils.GetOptionalUInt64()`
- New Extension Method: `JsonUtils.GetOptionalUri()`
- New Extension Method: `JsonUtils.GetOptionalValue()`
- New Extension Method: `JsonUtils.GetUri()`
- New Extension Method: `JsonUtils.GetValue()`
- New Extension Method: `JsonUtils.ReadDictionary()`
- New Extension Method: `JsonUtils.TryGetUri()`

#### API Changes

- `AnyObjectReader` is now a `JsonReader<object>` instead of `JsonReader<object?>` and will no longer deserialize a JSON `null`
  - *this is a breaking change*
  - note: it will still handle `null`s inside arrays or objects it deserializes
- `JsonBasedObject.UnhandledProperties` is now an `IReadOnlyDictionary<string, object?>?` instead of `Dictionary<string, object?>?`
  and is no longer marked `[JsonExtensionData]`
  - *this is a breaking change*
- `JsonUtils.ReadList()` is now an extension method
- `JsonUtils.WriteList()` is now an extension method
- `JsonUtils.ReadList()` now takes the serializer options as the last argument
  - *this is a breaking change*

#### API Removals

- `AnyObjectConverter` has been removed
  - it has been superseded by `AnyObjectReader`
- `InterfaceConverter` and `ReadOnlyListOfInterfaceConverter` have been removed
  - the MetaBrainz libraries are switching to custom converters for everything, removing the need for these

### v1.1.1 (2020-04-16)

This version fixes a build issue causing the XML documentation to be missing from the NuGet package.

### v1.1.0 (2020-04-15)

#### API Changes

- `AnyObjectConverter` is now marked as `[Obsolete]`
  - it is superseded by `AnyObjectReader` (see below)

#### API Additions

- New Class: `JsonReader<T>`
  - a deserialization-only `JsonConverter<T>`
- New Class: `JsonWriter<T>`
  - a serialization-only `JsonConverter<T>`
- New Class: `JsonConverter<T, TReader, TWriter>`
  - a `JsonConverter<T>` that forwards to a `JsonReader<T>` and `JsonWriter<T>`
- New Class: `ObjectReader<T>` - a `JsonReader<T>` that handles values serialized as JSON objects
  - has an abstract `ReadObjectContents` method to override
- New Class: `ObjectWriter<T>` - a `JsonWriter<T>` that handles values serialized as JSON objects
  - has an abstract `WriteObjectContents` method to override
- New Class: `AnyObjectReader` - a `JsonReader<object?>` that handles any value as a .NET object
  - replaces `AnyObjectConverter`
  - slightly different behaviour:
    - numbers will use `int` when possible (instead of starting from `long`)
    - decimal numbers will avoid `decimal` in degenerate cases
    - JSON objects are mapped to `Dictionary<string,object?>` instead of `JsonElement`
- New Method: `JsonUtils.ReadList()` (several overloads)
  - simplifies deserialisation of JSON arrays
- New Methods: `JsonUtils.WriteList()` and `JsonUtils.WriteListAsync()` (several overloads)
  - simplifies serialisation of JSON arrays

#### Other Changes

- Tweaks and updates to the build system
- Marked more types as `[PublicAPI]` (dev-time change only)

#### Dependency Updates

- JetBrainz.Annotations → 2020.1.0
- System.Text.Json → 4.7.1

### v1.0.0 (2020-03-20)

First release.

This package contains some JSON-related helper classes that were used by multiple projects of
the [MusicBrainz](https://github.com/Zastai/MusicBrainz) repository, which has been split into
multiple repositories (aiming for one NuGet package == one repository).
