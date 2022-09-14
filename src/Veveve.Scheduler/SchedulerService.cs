

using MediatR;

public interface ISchedulerService
{
    Task DoStuff();
}

public class SchedulerService : ISchedulerService
{
    private readonly IMediator _mediator;

    public SchedulerService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DoStuff()
    {
        // _mediator.Send(new DoStuffCommand());
    }
}
