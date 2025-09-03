using Logger.Domain.Data;
using Logger.Domain.Entities;
using MongoDB.Driver;
using Common.Contracts.Dtos;


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

        public async Task LogExceptionAsync(ExceptionLogMessage message)
        {
            try
            {
                var logEntry = new ExceptionLogEntity
                {
                    Timestamp = message.Timestamp ?? DateTime.UtcNow,
                    Service = message.Service,
                    Level = message.Level,
                    Message = message.Message,
                    Exception = message.Exception,
                    TraceId = message.TraceId
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
