using Microsoft.AspNetCore.Http;
using Veveve.Api.Domain.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Veveve.Api.Controllers;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Infrastructure.Middleware;

/// <summary>
/// middleware to catch exceptions and format the api response.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(httpContext, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var httpStatusCode = HttpStatusCode.InternalServerError;
        var errorCode = ErrorCodesEnum.GENERIC_INTERNAL;
        var messageParams = new string[0];

        
        if (exception is NotFoundException nfEx)
        {
            httpStatusCode = HttpStatusCode.NotFound;
            errorCode = nfEx.ErrorCode;
        }
        else if (exception is ConflictException cEx)
        {
            httpStatusCode = HttpStatusCode.Conflict;
            errorCode = cEx.ErrorCode;
        }
        else if (exception is BadFormatException bfEx)
        {
            httpStatusCode = HttpStatusCode.BadRequest;
            errorCode = bfEx.ErrorCode;
        }
        else if (exception is BusinessRuleException brEx)
        {
            httpStatusCode = HttpStatusCode.UnprocessableEntity;
            errorCode = brEx.ErrorCode;
        }

        if(exception is BusinessRuleException brEx2)
            messageParams = brEx2.MessageParams;

        var errorResponse = new ApiErrorResponse(errorCode, messageParams);
        var serializedResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        _logger.LogWarning("ExceptionMiddleware {errorResponse}", serializedResponse);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;
        return context.Response.WriteAsync(serializedResponse, context.RequestAborted);
    }
}
