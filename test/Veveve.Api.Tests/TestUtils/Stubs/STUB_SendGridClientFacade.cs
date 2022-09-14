using Veveve.Domain.Services;

namespace Veveve.Api.Tests.TestUtils.Stubs;

public class STUB_SendGridClientFacade : ISendGridClientFacade
{

    Task ISendGridClientFacade.SendEmail(SendGridMailDto mailDto)
    {
        return Task.CompletedTask;
    }
}