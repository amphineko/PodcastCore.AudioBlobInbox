using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PodcastCore.MediaInfoServer.Services;

namespace PodcastCore.MediaInfoServer
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogging(builder => builder.AddConsole());
            services.AddMvc();

            services.AddSingleton(_ =>
            {
                var config = new Configuration();
                _configuration.Bind(config);

                return new AmazonS3Client(config.AccessKey, config.SecretKey, new AmazonS3Config
                {
                    ForcePathStyle = true,
                    RegionEndpoint = RegionEndpoint.APEast1,
                    ServiceURL = config.ServiceUrl
                });
            });
            services.AddSingleton<BlobMediaInfoService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}