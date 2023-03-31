namespace GrpcClientTest.Base
{
    /// <summary>
    /// grpc 无用
    /// </summary>
    public class TestDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<TestDelegatingHandler> _logger;
        public TestDelegatingHandler(ILogger<TestDelegatingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentUri = request.RequestUri;
            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.LogDebug(ex, "Exception during SendAsync()");
                throw;
            }
            finally
            {
                request.RequestUri = currentUri;
            }
        }
    }
}
