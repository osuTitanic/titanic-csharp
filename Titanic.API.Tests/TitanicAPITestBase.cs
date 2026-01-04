using System;

namespace Titanic.API.Tests
{
    /// <summary>
    /// Base class for all Titanic API tests.
    /// Provides a shared API client instance and common test utilities.
    /// </summary>
    public abstract class TitanicAPITest : IDisposable
    {
        protected readonly TitanicAPI Api;

        protected TitanicAPITest()
        {
            Api = new TitanicAPI();
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}
