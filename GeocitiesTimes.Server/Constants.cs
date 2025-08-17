namespace GeocitiesTimes.Server
{
    public class Constants
    {
        /* This number was chosen after some very quick experimentation to see what delivered
         * best performance.  Please feel free to modify it.
         */
        public const int DefaultMaxConcurrency = 25;

        /* This number was chosen to be twice the amount of concurrent operations that will run
         * when GetStoryListFromCacheOrClient.  The ensures that the semaphore will be fully utilized.
         */
        public const int MaxBatchSize = DefaultMaxConcurrency * 2;
    }
}
