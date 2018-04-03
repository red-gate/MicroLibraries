# ULibs.TinyJsonDeser release notes

## 1.0.0

### Features

- A lightweight thread-safe JsonDeserializer class that can deserialise json text to .NET types. It does not support binding to specific .NET types. Rather, objects are deserialised to instances of `IDictionary<string, object>`, arrays are deserialised to instances of `object[]`, and literals are deserialised to either a `bool`, `double`, `string` or `null`.