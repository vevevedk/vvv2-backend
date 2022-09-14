using System;
using System.Threading.Tasks;

namespace Veveve.Domain.Services;

public interface ISendGridClientFacade
{
    Task SendEmail(SendGridMailDto mailDto);
}