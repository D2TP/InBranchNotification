using System;
using Convey;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


namespace Swagger.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private const string SectionName = "swagger";
        private const string RegistryName = "docs.swagger";

        public static IConveyBuilder AddSwaggerDocsExtended(this IConveyBuilder builder, string sectionName = SectionName)
        {
            var options = builder.GetOptions<SwaggerOptions>(sectionName);
            return builder.AddSwaggerDocsExtended(options);
        }

        public static IConveyBuilder AddSwaggerDocsExtended(this IConveyBuilder builder,
            Func<ISwaggerOptionsBuilder, ISwaggerOptionsBuilder> buildOptions)
        {
            var options = buildOptions(new SwaggerOptionsBuilder()).Build();
            return builder.AddSwaggerDocsExtended(options);
        }

        public static IConveyBuilder AddSwaggerDocsExtended(this IConveyBuilder builder, SwaggerOptions options)
        {
            if (!options.Enabled || !builder.TryRegister(RegistryName))
            {
                return builder;
            }

            builder.Services.AddSingleton(options);
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options.Name, new OpenApiInfo { Title = options.Title, Version = options.Version });
                if (options.IncludeSecurity)
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });
                }
            });

            return builder;
        }
        public static IApplicationBuilder UseSwaggerWithReverseProxySupport(this IApplicationBuilder builder)
        {

            var options = builder.ApplicationServices.GetService<SwaggerOptions>();
            var routePrefix = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "swagger" : options.RoutePrefix;


            builder.UseStaticFiles().UseSwagger(c => c.RouteTemplate = routePrefix + "/{documentName}/swagger.json");
            builder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{options.Name}/swagger.json", options.Title);
                c.RoutePrefix = routePrefix;
            });
            return builder;
        }
    }
}
//{routePrefix}/{options.Name