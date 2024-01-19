using Microsoft.AspNetCore.Mvc;
using SayList.Interfaces;
using SayList.Things;

namespace SayList.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeneratePlaylistController : ControllerBase
    {
        private readonly IPlaylistGeneratorManager _manager;

        private readonly ILogger<GeneratePlaylistController> _logger;

        public GeneratePlaylistController(ILogger<GeneratePlaylistController> logger, IPlaylistGeneratorManager playlistGeneratorManager)
        {
            _manager = playlistGeneratorManager;
            _logger = logger;
        }

        [HttpPost(Name = "BuildPlaylist")]
        public async Task<SaylistBuildResult> Post([FromBody] string userMessage)
        {
            try
            {
                SaylistBuildResult response = await _manager.BuildPlaylist(userMessage);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
