using Common.Contracts.Dtos;
using Logger.Application.Services;
using Logger.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Logger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExceptionLogsController : ControllerBase
    {
        private readonly ExceptionLoggerService _loggerService;

        public ExceptionLogsController(ExceptionLoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ExceptionLogMessage message)
        {

            try
            {
                if (message == null)
                {
                    return BadRequest("Log entry is required.");
                }

                await _loggerService.LogExceptionAsync(message);

                return CreatedAtAction(nameof(GetByTraceId), new { id = message.TraceId }, message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? level, [FromQuery] string? traceId)
        {
            try
            {
                var filterBuilder = Builders<ExceptionLogEntity>.Filter;
                var filter = filterBuilder.Empty;

                if (!string.IsNullOrEmpty(level))
                {
                    filter &= filterBuilder.Eq(log => log.Level, level);
                }

                if (!string.IsNullOrEmpty(traceId))
                {
                    filter &= filterBuilder.Eq(log => log.TraceId, traceId);
                }

                var logs = await _loggerService.FindAsync(filter);

                if (logs == null || logs.Count == 0)
                {
                    return NotFound("No logs found.");
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByTraceId(string id)
        {
            try
            {
                var log = await _loggerService.FindByTraceId(id);

                if (log == null)
                {
                    return NotFound("Log not found.");
                }

                return Ok(log);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
