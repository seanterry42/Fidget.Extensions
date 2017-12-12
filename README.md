# Identifiable
Generates GUIDs based on current time (UUIDv1), or based on a unique name using a cryptographic hash (UUIDv3 using MD5 or UUIDv5 using SHA1). Includes a non-standard variant layout of time-based GUIDs  for near-sequential sorting in SQL Server.

### TimeGuid.Create
Generates time-based GUIDs for use in database clustered indexes. The following algorithms are implemented:
* `Standard` generates a time-based GUID that conforms to [RFC 4122](https://tools.ietf.org/html/rfc4122#section-4) without exposing a MAC address.
* `SqlServer` generates a similar identifier that is optimally arranged for use with SQL Server clustered indexes and can be transposed to/from a valid Version 1 identifier. In this arrangement, the variant will be zero to avoid mis-identification as an invalid GUID version.

**Why not use a random GUID?**
Random GUIDs have [well-known performance issues](http://www.informit.com/articles/printerfriendly/25862) when used in clustered indexes.

**Why not use NEWSEQUENTIALID or UuidCreateSequential?**
* `NEWSEQUENTIALID` is only usable as a default constraint and the values it generates are not a valid GUID version (bytes 6/7 are reversed, causing the version to be in the wrong position).
* `UuidCreateSequential` requires P/Invoke and is only available on Windows.
* `UuidCreateSequential` and `NEWSEQUENTIALID` are only sequential when generated from the same system (the time component is in the least-significant bytes for SQL Server sorting).
* `UuidCreateSequential` and `NEWSEQUENTIALID` both include the MAC address which may be undesirable or non-unique (a pervasive problem with virtual machines).

**Why not use a COMB?**
The values generated by this method are, in fact, COMBs. [RFC 4122 Section 4.5](https://tools.ietf.org/html/rfc4122#section-4.5) provides for cryptographically random values to be used as the node instead of the MAC address, as well as using a random value for the clock sequence when the node value changes, effectively allowing up to 61 bits of random entropy per time-slice in each identifier.

### NamedGuid.Create
Generates a deterministic name-based GUID as described in [RFC 4122 Section 4.3](https://tools.ietf.org/html/rfc4122#section-4.3). The following algorithms are implemented:
* `MD5` generates an MD5 hash of the namespace and the name and yields a version 3 UUID.
* `SHA1` generates a SHA-1 hash of the namespace and the name and yields a version 5 UUID.