Helper methods to make Microsoft.Data.SqlClient behave more like System.Data.SqlClient regarding validating server certificates:

When connecting to SQL Server, the connection can be encrypted at the request of the client or the server.
* System.Data.SqlClient only validates server certificates when the client has requested encryption by setting Encrypt to true, but does not validate server certificates where only the server has requested encryption.
* Microsoft.Data.SqlClient always validates server certificates, regardless of which side requested encryption.

The helper method in this microlibrary adds Trust Server Certificate to a connection string to bypass validation when (1) the client did not request encryption, (2) the authentication method is not an Azure type, and (3) the server being connected to is on the LAN.
