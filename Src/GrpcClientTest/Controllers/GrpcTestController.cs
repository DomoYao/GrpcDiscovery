using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static GrpcServiceTest.Greeter;

namespace GrpcClientTest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GrpcTestController : ControllerBase
    {
        GreeterClient _greeterClient;
        private readonly ILogger _logger;

        public GrpcTestController(GreeterClient greeterClient, ILoggerFactory loggerFactory)
        {
            _greeterClient= greeterClient;
            _logger= loggerFactory.CreateLogger<GrpcTestController>();
        }

        /// <summary>
        /// gprc 服务发现压测.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> SayHello()
        {
            try
            {

                var res = await _greeterClient.SayHelloAsync(new GrpcServiceTest.HelloRequest { Name = DateTime.Now.ToString() }).ConfigureAwait(false);
                return res.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
