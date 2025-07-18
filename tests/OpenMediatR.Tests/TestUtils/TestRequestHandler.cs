namespace OpenMediatR.Tests;

internal sealed class TestRequestHandler : IRequestHandler<TestRequest, string>
{
    public Task<string> Handle(TestRequest request, CancellationToken cancellationToken)
        => Task.FromResult("Test");
}