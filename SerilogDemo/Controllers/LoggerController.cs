using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SerilogDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class LoggerController : ControllerBase
{
    private readonly ILogger<LoggerController> _logger;

    public LoggerController(ILogger<LoggerController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public string Get()
    {
        _logger.LogInformation("<= is here and again here => {RequestId}. Very conventient");
        _logger.LogInformation("However, its a label and this kills loki");

        return "Hello world";
    }
}