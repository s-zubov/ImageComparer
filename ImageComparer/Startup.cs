using ImageComparer.Algorithms;
using ImageComparer.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ImageComparer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            
            services.AddTransient<IImageComparerAlgorithm>(provider =>
                new GridImageComparerAlgorithm(
                    new PixelByPixelImageComparerAlgorithm(new ArgbPixelComparerAlgorithm()), 32,
                    32));
            services.AddTransient<IImageStorage, InnerImageStorage>();
            services.AddTransient<IImagePainter, ImagePainter>();
            services.AddTransient<IImageComparerManager, ImageComparerManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}