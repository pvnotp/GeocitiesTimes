namespace GeocitiesTimes.Server
{
    public class Constants
    {
        /* This number was chosen to protect the Hacker News API from being swamped with requests.  
         * You can raise it if you need better performance, but please keep in mind that the API
         * has no rate limiter so we must be our own rate limiter.
         */
        public const int DefaultMaxConcurrency = 5;

        /* This number was chosen to ensure there is always a full queue ready to go for GetStoryListFromCacheOrClient
         * to process.  The semaphore will always be full.  However, we keep our batches small enough that we will
         * know when it is time to early exit before processing too many resources.
         */
        public const int MaxBatchSize = DefaultMaxConcurrency * 5;
    }
}
