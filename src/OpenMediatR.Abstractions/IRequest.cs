namespace OpenMediatR;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest
{
}