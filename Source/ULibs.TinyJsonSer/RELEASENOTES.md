# ULibs.TinyJsonSer release notes

## 1.0.4

### Features

- Supports being referenced with Package Reference

## 1.0.3

### Bug fixes

- Bug 10: Correctly serialise a dynamic `ExpandoObject` as a json object rather than a json array.

## 1.0.2

### Improvements

- Removed use of `DictionaryEntry` type, to improve cross-platform compatibility.

## 1.0.1

### Improvements

- Refactoring to simplify the handling of unrecognised enum values.
- More correct usage of temporary StringWriter.

## 1.0.0

### Features

- A lightweight thread-safe JsonSerializer class that can serialise arbitrary objects to either indented or compact json. Dates are converted to a standard ISO 8601 UTC date format (`yyyy-MM-ddTHH:mm:ssZ`). Property and field names of objects are converted to lower camel case, though dictionary keys are serialised as-is.