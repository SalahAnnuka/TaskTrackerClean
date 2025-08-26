using Logger.Domain.Data;
using Logger.Domain.Entities;
using MongoDB.Driver;
using Logger.Application.Dtos;

namespace Logger.Application.Services
{
    public class ExceptionLoggerService
    {
        private readonly MongoDbService _mongoDbService;

        public ExceptionLoggerService(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<List<ExceptionLogEntity>> FindAsync(FilterDefinition<ExceptionLogEntity> filter)
        {
            try
            {
                var logs = await _mongoDbService.ExceptionLogs
                    .Find(filter)
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving logs.", ex);
            }
        }

        public async Task<ExceptionLogEntity?> FindByTraceId(string traceId)
        {
            try
            {

                var log = await _mongoDbService.ExceptionLogs
                    .Find(log => log.TraceId == traceId)
                    .FirstOrDefaultAsync();

                return log;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving log by ID.", ex);
            }
        }

        public async Task LogExceptionAsync(ExceptionLogDto logDto )
        {
            try
            {
                var logEntry = new ExceptionLogEntity
                {
                    Timestamp = logDto.Timestamp ?? DateTime.UtcNow,
                    Service = logDto.Service,
                    Level = logDto.Level,
                    Message = logDto.Message,
                    Exception = logDto.Exception,
                    TraceId = logDto.TraceId
                };

                await _mongoDbService.ExceptionLogs.InsertOneAsync(logEntry);
            }
            catch (Exception ex)
            {
                throw new Exception("Error logging exception.", ex);
            }
        }
    }
}
