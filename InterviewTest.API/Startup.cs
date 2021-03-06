using System;
using AutoMapper;
using InterviewTest.API.DTOs;
using InterviewTest.BusinessLogic.Managers;
using InterviewTest.Contracts.Managers;
using InterviewTest.Contracts.UnitOfWork;
using InterviewTest.DataAccess;
using InterviewTest.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InterviewTest.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration = new ConfigurationBuilder()
                                .AddEnvironmentVariables()
                                .AddJsonFile("appsettings.json")
                                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                .Build());
            });

            services.AddControllers();

            var connection = string.Format(
                "Server={0};Database={1};Trusted_Connection=True;MultipleActiveResultSets=True;Integrated Security=true;",
                Environment.GetEnvironmentVariable("SERVER") ?? Configuration["Database:SERVER"],
                Environment.GetEnvironmentVariable("DATABASE") ?? Configuration["Database:DATABASE"]
                );

            services.AddDbContext<InterviewTestContext>(options => options.UseSqlServer(connection));

            var config = new MapperConfiguration(mapperConfig =>
            {
                mapperConfig.CreateMap<Leave, LeaveDTO>();
                mapperConfig.CreateMap<LeaveDTO, Leave>();
                mapperConfig.CreateMap<LeaveType, LeaveTypeDTO>();
                mapperConfig.CreateMap<LeaveTypeDTO, LeaveType>();
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddTransient<ILeaveManager, LeaveManager>();
            services.AddTransient<ILeaveTypeManager, LeaveTypeManager>();
            services.AddAutoMapper(typeof(Startup));
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
            app.UseCors("CorsPolicy");
        }
    }
}
