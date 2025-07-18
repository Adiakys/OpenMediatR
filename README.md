# OpenMediatR

**OpenMediatR** is a lightweight, open-source alternative to the popular [MediatR](https://github.com/jbogard/MediatR) library. It provides simple in-process messaging with support for `IRequest`/`IRequestHandler` and `INotification`/`INotificationHandler` patterns, aiming for clarity, minimalism, and zero external dependencies.

> ⚠️ **Preview Version:** This is version `0.0.2`, an early preview release intended for testing and feedback. APIs and structure may change.

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


### Requests

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

### Notifications

```csharp
// Define a notification
public class Alert : INotification
{
    public string Message { get; set; }
}

// Implement a handler
public class AlertHandler : INotificationHandler<Alert>
{
    public Task Handle(Alert notification, CancellationToken cancellationToken)
    {
        Console.WriteLine(notification.Message);
        return Task.CompletedTask;
    }
}
```

### Dispatching

```csharp
public class SomeService
{
    private readonly ISender _sender;
    private readonly IPublisher _publisher;

    public SomeService(ISender sender, IPublisher publisher)
    {
        _sender = sender;
        _publisher = publisher;
    }

    public async Task Run()
    {
        var response = await _sender.Send(new Ping());
        Console.WriteLine(response); // "Pong"

        await _publisher.Publish(new Alert { Message = "Something happened!" });
    }
}
```

---

## 📌 Roadmap

- [x] Notification support (`INotification`)
- [ ] Pipeline behaviors (`IRequestPipeline<,>`)
- [ ] Request validators (`IRequestValidator<>`)
- [ ] Custom behaviors and decorators
- [ ] NuGet packaging and CI/CD
- [ ] Performance benchmarks

---

## 🤝 Contributing

OpenMediatR is in its infancy — contributions, ideas, and issues are all welcome! Feel free to fork, raise issues, or suggest enhancements.

---

## 📄 License

This project is licensed under the [Apache License 2.0](LICENSE).