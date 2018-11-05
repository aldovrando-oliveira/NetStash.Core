[![NuGet](https://img.shields.io/nuget/v/NetStash.Core.svg)](https://www.nuget.org/packages/NetStash.Core/)

# NetStash.Core
Logstash sender for .NET (based on the [NetStash](https://github.com/iquirino/NetStash) project)

Send events to logstash instance via TCP

Saves all events into a sqlite database to prevent loss from network issues

Automatic synchronization when network connection is stablished

## Installation

Nugget Package: https://www.nuget.org/packages/NetStash.Core

```
PM > Install-Package NetStash.Core
```
or
```
dotnet add package NetStash.Core
```

## Usage

```
NetStashLog log = new NetStashLog("brspomelkq01.la.imtn.com", 1233, "NSTest", "NSTestLog");

Dictionary<string, string> vals = new Dictionary<string, string>();
//Additional fields
vals.Add("customerid", "1235");

log.Error("Testing", vals);
```

## Logstash config

```
input {
  tcp {
    port => 1233
    host => "10.32.12.52"
    codec => json
  }
}
filter {
  mutate { gsub => ["message", "@($NL$)@", "\r\n"] }
}
output {
  elasticsearch {

  }
}

```