using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using EGT.ApiGateway.ApplicationServices;

using Autofac;
//using StackExchange.Redis;
using BeetleX.Redis;

namespace EGT.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostingEnvironment { get; }


        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {

            var redisConfiguration = new RedisConfiguration();
            Configuration.Bind(nameof(RedisConfiguration), redisConfiguration);

            var sessionConfiguration = new SessionConfiguration();
            Configuration.Bind(nameof(SessionConfiguration), sessionConfiguration);

            //builder.RegisterInstance(ConnectionMultiplexer.Connect(redisConfiguration.Host + ":" + redisConfiguration.Port))
            //   .As<ConnectionMultiplexer>()
            //   .SingleInstance();
            var redisDB = DefaultRedis.Instance;
            redisDB.DataFormater = new JsonFormater();
            redisDB.Host.AddWriteHost(redisConfiguration.Host, redisConfiguration.Port);

            builder.RegisterInstance(redisDB)
                .As<RedisDB>()
                .SingleInstance();

            // builder.RegisterType<InMemorySessionService>().As<ISessionService>().SingleInstance();
            //builder.RegisterType<StackExchangeRedisSessionService>()
            //    .As<ISessionService>()
            //    .SingleInstance();
            builder.RegisterType<SessionServiceBeetleXRedis>()
                .As<ISessionService>()
                .SingleInstance();

            //builder.RegisterType<StatisticsServiceStackExchangeRedis>()
            //    .As<IStatisticsService>()
            //    .SingleInstance();
            builder.RegisterType<StatisticsServiceBeetleXRedis>()
                .As<IStatisticsService>()
                .SingleInstance();

            builder.RegisterInstance(sessionConfiguration)
                .As<SessionConfiguration>()
                .SingleInstance();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddOptions();
            //services.Configure<RedisConfiguration>(Configuration.GetSection(nameof(RedisConfiguration)));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
