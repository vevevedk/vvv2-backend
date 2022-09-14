

using MediatR;

public interface IJobRunnerService
{
    Task DoStuff();
}

public class JobRunnerService : IJobRunnerService
{
    private readonly IMediator _mediator;

    public JobRunnerService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DoStuff()
    {
        // _mediator.Send(new DoStuffCommand());
    }
}
