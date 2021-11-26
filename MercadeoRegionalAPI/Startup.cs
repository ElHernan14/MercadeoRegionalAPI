using MercadeoRegionalAPI.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>//el sitio web valida con cookie
                {
                       options.LoginPath = "/Usuarios/Login";
                       options.LogoutPath = "/Usuarios/Logout";
                       options.AccessDeniedPath = "/Home/Index";
                   })
                   .AddJwtBearer(options =>
                   {
                       options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                       {
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ValidateIssuerSigningKey = true,
                           ValidIssuer = Configuration["TokenAuthentication:Issuer"],
                           ValidAudience = Configuration["TokenAuthentication:Audience"],
                           IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
                               Configuration["TokenAuthentication:SecretKey"])),
                       };
                   });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "Empleado"));
                options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
            });
            services.AddSignalR();
            services.AddDbContext<DataContext>(options => options.UseSqlServer(
                this.Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MercadeoRegionalAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MercadeoRegionalAPI v1"));
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
