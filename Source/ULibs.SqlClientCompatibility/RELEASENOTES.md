# ULibs.SqlClientCompatibility release notes

## 1.0.2

### Fixes
- Fixed SetBackwardsCompatibleTrustServerCertificateValue when a SqlConnectionStringBuilder from Microsoft.Data.SqlClient v4+ is passed in
- Removed ShouldTrustServerCertificate from the public API, as it wasn't intended to be exposed and isn't used externally anywhere

## 1.0.1

### Fixes
- Removed workaround for an MSBuild bug with WPF projects that appears to no longer be needed

## 1.0.0

### Features
- Breaking change: extension method AddTrustServerCertificateForCompatibility has been renamed to SetBackwardsCompatibleTrustServerCertificateValue

## 0.1.1

### Features
- If the SMARTASSEMBLY compile constant is set, DoNotCaptureVariables attribute will be applied methods that process connection strings.

## 0.1.0

### Features
- Extension method AddTrustServerCertificateForCompatibility for SqlConnectionStringBuilder, DbConnectionStringBuilder, and connection string.
