namespace SerilogDemo.Controllers;

public class RequestIdMiddleware
{
    private readonly RequestDelegate _requestDelegate;

    public RequestIdMiddleware(RequestDelegate nextRequestDelegate)
    {
        _requestDelegate = nextRequestDelegate;
    }
    
    public async Task InvokeAsync(
        HttpContext context,
        ILogger<RequestIdMiddleware> logger)
    {
        using IDisposable scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = Guid.NewGuid()
        });
        logger.LogInformation("<= is here and again here => {RequestId}. Very conventient");
        await _requestDelegate.Invoke(context);
    }
}