[![NuGet](https://img.shields.io/nuget/v/NetStash.Core.svg)](https://www.nuget.org/packages/NetStash.Core/)

# NetStash.Core
Cliente Logstash .NET  Core (Baseado no projeto [NetStash](https://github.com/iquirino/NetStash))

Funcionalidades
  * Envio de eventos via TCP
  * Salva todos eventos em um banco SQLite para previnir problemas durante o envio das mensagens
  * Sincronizaçao automatica quando a rede é reestabelecida

## Instalação

Nugget Package: https://www.nuget.org/packages/NetStash.Core

```
PM > Install-Package NetStash.Core
```
or
```
dotnet add package NetStash.Core
```

## Inicialização  

```csharp
new NetStashLog({servidor}, {porta}, {sistema}, {logger});
```
| Propriedade 	|                Descrição               	|  Tipo  	| Obrigatório 	|
|-----------  	|---------------------------------------	|:------:	|:-----------:	|
| servidor    	| Nome ou IP do servidor                 	| Texto  	| Sim         	|
| porta       	| Porta do servidor                      	| Número 	| Sim         	|
| sistema     	| No do sistema que está efetuando o Log 	| Texto  	| Sim         	|
| logger      	| Componente do sistema                  	| Texto  	| Sim         	|

### Exemplo  
```csharp
var logger = new NetStashLog("127.0.0.1", 5030, "Sistema.Teste", "Startup")
```

## Uso
Niveis de log disponiveis:  
  - Verbose = 0,
  - Debug = 1,
  - Information = 2,
  - Warning = 3,
  - Error = 4,
  - Fatal = 5 

```csharp
var logger = new NetStashLog("127.0.0.1", 5030, "Sistema.Teste", "Startup");

logger.Verbose("Iniciando aplicação");
logger.Debug("Carregando dependências");
logger.Information("Aplicação inicializada com sucesso");

logger.Warning("Item não encontrado");

var exception = new Exception();
logger.Error(exception);
logger.Error("Erro inesperado", exception);

var fatalException = new Exception();
logger.Fatal(fatalException);
logger.Fatal("Erro inesperado", fatalException);

```

Em todos os níveis é possível informar dados adicionais
```csharp
var logger = new NetStashLog("127.0.0.1", 5030, "Sistema.Teste", "Startup");

// Informações Adicionais
var vals = new Dictionary<string, string>();
vals.Add("itemid", "1235");

log.Information("Teste", vals);

var exception = new Exception();
logger.Error(exception, vals);
logger.Error("Erro inesperado", exception, vals);
``` 

## Saída
A saida do log para o servidor é no formato `json`
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
> Informações adicionais são incluídas no corpo da mensagem no mesmo nível das propriedades atuais

## Exemplo de configuração
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