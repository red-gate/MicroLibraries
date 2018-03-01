# ULibs.TinyJsonSer release notes

## 0.1.0

### Features

- A lightweight thread-safe JsonSerializer class that can serialise arbitrary objects to either indented or compact json. Dates are converted to a standard ISO 8601 UTC date format (`yyyy-MM-ddTHH:mm:ssZ`). Property and field names of objects are converted to lower camel case, though dictionary keys are serialised as-is.