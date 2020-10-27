using System.Xml;

namespace EGT.ApiGateway.Dto
{
    public class XMLCommandFactory
    {
        public static XMLCommandDtoBase CreateXMLCommand(string xmlContent)
        {
            var xmlDocument = new XmlDocument();

            XMLCommandDtoBase result = null;
            try
            {
                xmlDocument.LoadXml(xmlContent);

                result = ParseXMLCommandEnterSession(xmlDocument);
                if (result == null)
                {
                    result = ParseXMLCommandGetSession(xmlDocument);
                }
            }
            catch (XmlException)
            {
            }

            return result;
        }

        private static XMLCommandEnterSessionDto ParseXMLCommandEnterSession(XmlDocument xmlDocument)
        {
            var result = new XMLCommandEnterSessionDto();

            var commandElement = xmlDocument.SelectSingleNode("//command");
            if (commandElement == null)
            {
                return null;
            }

            var commandIdString = commandElement.Attributes["id"]?.Value;
            if (commandIdString == null)
            {
                return null;
            }
            result.Id = commandIdString;

            var sessionValue = xmlDocument.SelectSingleNode("//command/enter")?.Attributes["session"]?.Value;
            if (sessionValue == null)
            {
                return null;
            }
            if (!int.TryParse(sessionValue, out var sessionValueParsed))
            {
                return null;
            }
            result.SessionId = sessionValueParsed;

            var timestampValue = xmlDocument.SelectSingleNode("//command/enter/timestamp")?.InnerText;
            if (timestampValue == null)
            {
                return null;
            }
            if (!long.TryParse(timestampValue, out var timestampValueParsed))
            {
                return null;
            }
            result.Timestamp = timestampValueParsed;

            var playerValue = xmlDocument.SelectSingleNode("//command/enter/player")?.InnerText;
            if (playerValue == null)
            {
                return null;
            }
            if (!int.TryParse(playerValue, out var playerValueParsed))
            {
                return null;
            }
            result.Player = playerValueParsed;

            return result;
        }


        private static XMLCommandGetSessionDto ParseXMLCommandGetSession(XmlDocument xmlDocument)
        {
            var result = new XMLCommandGetSessionDto();

            var commandElement = xmlDocument.SelectSingleNode("//command");
            if (commandElement == null)
            {
                return null;
            }

            var commandIdString = commandElement.Attributes["id"]?.Value;
            if (commandIdString == null)
            {
                return null;
            }
            result.Id = commandIdString;

            var sessionValue = xmlDocument.SelectSingleNode("//command/get")?.Attributes["session"]?.Value;
            if (sessionValue == null)
            {
                return null;
            }
            if (!int.TryParse(sessionValue, out var sessionValueParsed))
            {
                return null;
            }
            result.SessionId = sessionValueParsed;

            return result;
        }
    }
}
