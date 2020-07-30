# ULibs.TinyJsonDeser release notes

## 1.0.3

### Features

- Supports C# 8.0 reference type nullability  

## 1.0.2

### Features

- Supports being referenced with Package Reference

## 1.0.1

### Features

- Improved code readability for parsing `true`, `false` and `null`.

## 1.0.0

### Features

- A lightweight thread-safe JsonDeserializer class that can deserialize json text to .NET types. It does not support binding to specific .NET types. Rather, objects are deserialized to instances of `IDictionary<string, object>`, arrays are deserialized to instances of `object[]`, and literals are deserialized to either a `bool`, `double`, `string` or `null`.
