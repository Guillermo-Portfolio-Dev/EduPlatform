using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Academic.Infrastructure.Resilience
{
    public static class PollyPolicies
    {
        public static AsyncRetryPolicy GetSqlRetryPolicy(ILogger logger)
        {
            return Policy
                .Handle<Exception>(ex => ex is SqlException || ex is TimeoutException)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        logger.LogWarning(
                            "Fallo temporal detectado. Reintentando por la vez #{RetryCount} en {TimeSpan} segundos debido a: {Message}",
                        retryCount,
                        timeSpan.TotalSeconds,
                        exception.Message);
                    }
                );
        }
    }
}
