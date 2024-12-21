using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

public class ModelStateValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ModelStateValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        if (endpoint != null)
        {
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (actionDescriptor != null)
            {
                var modelState = context.RequestServices.GetService(typeof(ModelStateDictionary)) as ModelStateDictionary;
                if (modelState != null && !modelState.IsValid)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json"; // Set the content type
                    var validationProblemDetails = new ValidationProblemDetails(modelState);
                    await JsonSerializer.SerializeAsync(context.Response.Body, validationProblemDetails); // Use JsonSerializer to write JSON
                    return;
                }
            }
        }

        await _next(context);
    }
}
