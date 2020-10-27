using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Autofac.Extensions.DependencyInjection;

namespace EGT.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            //var xmlCommand = XMLCommandFactory.XMLCommandFactory.CreateXMLCommand(
            //    "<command id=\"1234-8785\" >" +
            //    "<get session=\"13617162\" />" +
            //    "</command>");

            //var sessionService = new InMemorySessionService();
            //Debug.Assert(sessionService.GetSession(1234) == null);

            //var session = new UserSession() { Id = "1", Player = 2, SessionId = 3, Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() };
            //Debug.Assert(sessionService.CreateSession(session) != null);

            //Debug.Assert(sessionService.GetSession(session.SessionId) != null);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
