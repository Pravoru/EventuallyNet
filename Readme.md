## Eventually everything could success 

Eventually is a library for integration tests inspired by Scalatest Eventually. 
 
## Goal

Integration tests are tests when sometime you need to wait some event. Sometime you need to wait alot. 
For example selenium tests mainly consists of waiting. Scalatest eventually is the most useful library it that case and the porpose of this project is to port is to dotnet.

## Instalation

```
dotnet add package EventuallyNet
```

## Using

You can use it through static import

```csharp
using static EventuallyNet.EventuallyStatic;

Eventually(() => "It works!");
```

Or by creating instans yourself.

```csharp
using EventuallyNet;

var patienceConfig = new PatienceConfig(TimeSpan.FromMilliseconds(150), TimeSpan.FromMilliseconds(15));
var ev = new EventuallyClass(patienceConfig);
ev.Eventually(() => "It works!");
```

You find more examples in tests.
 