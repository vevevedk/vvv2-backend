using System;
using System.Threading.Tasks;

namespace Veveve.Api.Domain.Services;

public interface ISendGridClientFacade
{
    Task SendEmail(SendGridMailDto mailDto);
}