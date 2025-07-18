# OpenMediatR

**OpenMediatR** is a lightweight, open-source alternative to the popular [MediatR](https://github.com/jbogard/MediatR) library. It provides simple in-process messaging with support for `IRequest`/`IRequestHandler` patterns, aiming for clarity, minimalism, and zero external dependencies.

> ⚠️ **Preview Version:** This is version `0.0.1`, an early preview release intended for testing and feedback. APIs and structure may change.

![OpenMediatR](icon.png)

---

## 🧩 Why "OpenMediatR"?

Because messaging should be simple, open, and yours to control — without magic, and without cost.

---

## ✨ Features

- ✅ Clean, minimal abstraction layer
- ✅ Zero external dependencies
- ✅ Simple integration with .NET DI
- ✅ Clear separation of contracts and implementation
- ✅ Unit-test friendly architecture
- ✅ Familiar `IRequest`, `IRequestHandler`, and `ISender` patterns

---

## 🧠 How It Works

OpenMediatR allows you to define requests and handlers:

```csharp
// Define a request
public class Ping : IRequest<string> { }

// Implement a handler
public class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken cancellationToken)
        => Task.FromResult("Pong");
}
```

And then use `ISender` to dispatch:

```csharp
public class SomeService
{
    private readonly ISender _sender;

    public SomeService(ISender sender)
    {
        _sender = sender;
    }

    public async Task Run()
    {
        var response = await _sender.Send(new Ping());
        Console.WriteLine(response); // "Pong"
    }
}
```

---

## 📌 Roadmap

- [ ] Pipeline behaviors
- [ ] Request validators
- [ ] Notification support (like `INotification`)
- [ ] Custom behaviors and decorators
- [ ] NuGet packaging and CI/CD
- [ ] Performance benchmarks

---

## 🤝 Contributing

OpenMediatR is in its infancy — contributions, ideas, and issues are all welcome! Feel free to fork, raise issues, or suggest enhancements.

---

## 📄 License

This project is licensed under the [Apache License 2.0](LICENSE).