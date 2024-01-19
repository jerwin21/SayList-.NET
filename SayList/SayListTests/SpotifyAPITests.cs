using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using SayList.Engines;
using SayList.Things;
using SayList.Utilities;

namespace SayListTests
{
    public class SpotifyAPITests
    {
        private IConfigurationRoot _configuration;
        private IMemoryCache _memoryCache;

        private Mock<IHttpClientFactory> _httpClientFactory;


        public SpotifyAPITests()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.Development.json", true, true)
                .AddUserSecrets<SpotifyAPITests>()
                .Build();

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _httpClientFactory = new Mock<IHttpClientFactory>();
            var httpClient = new HttpClient();

            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
        }

        [Fact]
        public async Task GetAccessTokenTest()
        {
            SpotifyAuthorizationEngine engine = new SpotifyAuthorizationEngine(_configuration, _memoryCache, new HttpRequestUtility(_httpClientFactory.Object));
            SpotifyAccessTokenResponse response = await engine.GetAccessTokenAsync();

            Assert.NotNull(response);
            Assert.NotNull(response.AccessToken);

        }
    }
}