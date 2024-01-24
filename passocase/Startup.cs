using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using passo.Service;
using passo.Service.Interfaces;
using passocase.Data;


namespace passocase
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
            services.AddControllers();
            services.AddDbContext<passocase.Data.OrderDbContext>(
               options =>
               {
                   options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                       opt => opt.EnableRetryOnFailure(3));
               },
               ServiceLifetime.Transient);

            //services.AddDbContextFactory<OrderDbContext, OrderDBContextFactory>();
            services.AddScoped<ICustomerOrderService, CustomerOrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            services.AddSwaggerGen();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
