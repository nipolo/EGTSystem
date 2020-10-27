using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using EGT.ApiGateway.ApplicationServices;
using EGT.ApiGateway.Dto;
using EGT.ApiGateway.DomainModels;

namespace EGT.ApiGateway
{
    [ApiController]
    public class XMLApiController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly SessionConfiguration _sessionConfiguration;

        public XMLApiController(
            ISessionService sessionService, 
            SessionConfiguration sessionConfiguration)
        {
            _sessionService = sessionService;
            _sessionConfiguration = sessionConfiguration;
        }

        [HttpPost("xml_api/command")]
        public async Task<ActionResult> PostCommand()
        {
            using var reader = new StreamReader(Request.Body);

            var commandPostItem = await reader.ReadToEndAsync();

            var command = XMLCommandFactory.CreateXMLCommand(commandPostItem);

            if (command is XMLCommandEnterSessionDto)
            {
                var enterSessionDto = command as XMLCommandEnterSessionDto;
                _sessionService.CreateSession(new UserSession()
                {
                    RequestId = enterSessionDto.Id,
                    Player = enterSessionDto.Player,
                    SessionId = enterSessionDto.SessionId,
                    Timestamp = enterSessionDto.Timestamp
                }, _sessionConfiguration.TTL);

                return Ok();
            }

            if (command is XMLCommandGetSessionDto)
            {
                var enterSessionDto = command as XMLCommandGetSessionDto;
                var session = _sessionService.GetSession(enterSessionDto.SessionId);

                return Ok(session);
            }

            return Ok();
        }
    }
}
