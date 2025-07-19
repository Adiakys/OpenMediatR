namespace OpenMediatR.Tests;

public sealed record TestRequest2(bool Input) : IRequest<bool>;