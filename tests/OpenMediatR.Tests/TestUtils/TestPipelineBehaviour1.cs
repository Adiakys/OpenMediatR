namespace OpenMediatR.Tests;

internal sealed class TestPipelineBehaviour1 : IPipelineBehaviour<IRequest<string>, string>
{
    public int Count { get; private set; } = 0;
    
    public Task<string> Handle(IRequest<string> request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        Count++;
        return next(cancellationToken);
    }
}