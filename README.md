# MetaBrainz.Common.Json  [![Build Status](https://img.shields.io/appveyor/build/zastai/metabrainz-common-json)](https://ci.appveyor.com/project/Zastai/metabrainz-common-json) [![NuGet Version](https://img.shields.io/nuget/v/MetaBrainz.Common.Json)](https://www.nuget.org/packages/MetaBrainz.Common.Json)

JSON-related helper classes, for use by the other `MetaBrainz.*` packages.

## Release Notes

### v1.2.0 (_work in progress_)

#### API Changes

#### API Additions

- New Extension Method: `JsonUtils.GetUri()`
- New Extension Method: `JsonUtils.TryGetUri()`

#### Other Changes

#### Dependency Updates


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

- tweaks and updates to the build system
- marked more types as `[PublicAPI]` (dev-time change only)

#### Dependency Updates

- JetBrainz.Annotations → 2020.1.0
- System.Text.Json → 4.7.1


### v1.0.0 (2020-03-20)

First release.

This package contains some JSON-related helper classes that were used by multiple projects of
the [MusicBrainz](https://github.com/Zastai/MusicBrainz) repository, which has been split into
multiple repositories (aiming for one NuGet package == one repository).
