[![NuGet](https://img.shields.io/nuget/v/NetStash.Core.svg)](https://www.nuget.org/packages/NetStash.Core/)

# NetStash.Core
A client for Logstash .NET Core (similar to [NetStash](https://github.com/iquirino/NetStash)) 

### Features
  * Sends events via TCP
  * Saves all events into a sqlite database to prevent loss from network issues
  * Automatic synchronization when network connection is stablished

## Install

Nugget Package: https://www.nuget.org/packages/NetStash.Core

```
PM > Install-Package NetStash.Core
```
or
```
dotnet add package NetStash.Core
```

## Initialize

```csharp
new NetStashLog({server}, {port}, {project}, {logger});
```
| Propriedade 	|                Descrição               	|  type | Required |
|-----------  	|---------------------------------------	| :------:	| :-----------:	|
| server    	| Name or server's IP                 	| Text  	| Yes         	|
| port       	| Server's port                      	| Number | Yes         	|
| project     	| Project's name to sends the logstash instance	| Text  	| Yes         	|
| logger      	| Project's component                  	| Text  	| Yes         	|

### Example
```csharp
var logger = new NetStashLog("127.0.0.1", 5030, "Project.Test", "Startup")
```

## Getting Started

Available log levels:
  - Verbose = 0,
  - Debug = 1,
  - Information = 2,
  - Warning = 3,
  - Error = 4,
  - Fatal = 5 

```csharp
var logger = new NetStashLog("127.0.0.1", 5030, "Project.Test", "Startup");

logger.Verbose("Starting the application");
logger.Debug("Loading dependencies");
logger.Information("Application successfully started!");

logger.Warning("Item not found");

var exception = new Exception();
logger.Error(exception);
logger.Error("Unexpected Error", exception);

var fatalException = new Exception();
logger.Fatal(fatalException);
logger.Fatal("Unexpected Error", fatalException);

```

It is possible to provide any additional data
```csharp
var logger = new NetStashLog("127.0.0.1", 5030, "Sistema.Teste", "Startup");

// Informações Adicionais
var vals = new Dictionary<string, string>();
vals.Add("itemid", "1235");

log.Information("Test", vals);

var exception = new Exception();
logger.Error(exception, vals);
logger.Error("Unexpected Error", exception, vals);
``` 

## Output
The log's output has the `json` format.
```json
{
  "timestamp": "",
  "machine-name": "",
  "index-name": "",
  "level": "",
  "source": "",
  "message": "",
  "error.message": "",
  "error.details": ""
}
```

> Additional informations are included in the message's body and the same level of the current properties

## Configuration example
### Logstash

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
