# ULibs.FullExceptionString release notes

## 1.0.1

### Features

- Supports being referenced with Package Reference

## 1.0.0

### Features

- Provides a comprehensive alternative to Exception.ToString(). The string output is more cleanly organised, so that for multiple exceptions it's clearer which messages belong to which exception. Special treatment is given to AggregateException and ReflectionTypeLoadException, which have their own set of inner exception properties. The library also includes convenience extension methods for both StringBuilder and TextWriter.