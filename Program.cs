using System;
using System.Collections.Generic;
using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.Docs.Swagger;
using Convey.HTTP;
using Convey.LoadBalancing.Fabio;
using Convey.Logging;
using Convey.MessageBrokers.CQRS;
//using Convey.MessageBrokers.Outbox;
//using Convey.MessageBrokers.Outbox.Mongo;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Metrics.Prometheus;
////using Convey.Persistence.MongoDB;
using Convey.Persistence.Redis;
using InBranchNotification.Commands;
using InBranchNotification.Domain;
using InBranchNotification.Events.External;
using InBranchNotification.DTOs;
using InBranchNotification.Queries;
using Convey.Tracing.Jaeger;
using Convey.Tracing.Jaeger.RabbitMQ;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Convey.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Convey.Auth;
 
using Microsoft.Extensions.DependencyInjection;
using InBranchNotification.DbFactory;
using DbFactory;
using InBranchNotification.Extensions;
using InBranchNotification.Services;
using Swagger.Extension;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InBranchNotification
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                var jwtConfig = config.GetSection("JwtConfig").Get<JwtConfig>();


                webBuilder.ConfigureServices(services =>
                {
                    services.AddSwagger();
                    services.AddMvcCore().AddApiExplorer();
                    services.AddScoped<IConvertDataTableToObject, ConvertDataTableToObject>();
                    services.AddScoped<ITokenService, TokenService>();
                    services.AddScoped<IDbController, DbController>();
                    services.AddScoped<IValidateService, ValidateService>();
                    services.AddScoped<INotificationTypeService, NotificationTypeService>();
                    services.AddScoped<IServiceRequestService, ServiceRequestService>();
                    services.AddScoped<IAuthenticateRestClient, AuthenticateRestClient>();
                    services.AddScoped<IServiceRequestHistory, ServiceRequestHistory>();
                    services.AddScoped<IServiiceRequestTypeService, ServiiceRequestTypeService>();
                    services.AddScoped<IServiceRequestStatusService, ServiceRequestStatusService>();
                    services.AddScoped<IErrorList, ErrorList>();
                    services.AddScoped<IBaseUrlService, BaseUrlService>();
                    services.AddAuthorization();
                    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                    services.AddAutoMapper(typeof(Program));

                    // ValidateService : IValidateService IAuthenticateRestClient
                    services
                            .AddConvey()
                            //              .AddErrorHandler<ExceptionToResponseMapper>()
                            //.AddServices()
                            .AddHttpClient()
                            .AddCorrelationContextLogging()
                            .AddConsul()
                            .AddFabio()
                            .AddJaeger()
                            //   .AddMongo()
                            //.AddMongoRepository<Account, Guid>("accounts")
                            .AddCommandHandlers()
                            .AddEventHandlers()
                            .AddQueryHandlers()
                            .AddInMemoryCommandDispatcher()
                            .AddInMemoryEventDispatcher()
                            .AddInMemoryQueryDispatcher()
                            .AddPrometheus()
                            
                            //   .AddRedis()
                       //     .AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin())
                        //    .AddMessageOutbox(
                        //    o => o.AddMongo()
                    //    )
                           // .AddWebApi()
                           .AddSwaggerDocsExtended()
                                        .AddSwaggerDocs()
                            //.AddWebApiSwaggerDocs()
                            .AddJwt()
                            .Build();
                })
                        .Configure(app => app
                            .UseConvey()
                            .UserCorrelationContextLogging()
                           .UseMiddleware<CustomExcepExceptionExtensions>()
                            .UseMiddleware<AuthenticationMiddleware>()
                            .UsePrometheus()
                            .UseRouting()
                            .UseAuthentication()
                            .UseAuthorization()
                            //.UseCertificateAuthentication()
                            .UseEndpoints(r => r.MapControllers())
                            //.UseDispatcherEndpoints(endpoints => endpoints
                            //        .Get("", ctx => ctx.Response.WriteAsync("Accounts Service"))
                            //        .Get("ping", ctx => ctx.Response.WriteAsync("pong"))
                            //        .Get<GetAccounts, IEnumerable<AccountDto>>("accounts/{customerId}")
                            //        //G. OMONI my attempt below, but not able top validate and  swagger result not valid
                            //        .Get<LoginWithAdQuery, bool>("accounts/{LoginDTO}")
                            //        .Get<GetJwtToken, string>("login")
                            //        .Get<GetAccount, AccountDto>("accounts/{customerId}/{accountNo}", auth: true)
                            //        .Post<CreateAccount>("accounts",
                            //            afterDispatch: (cmd, ctx) => ctx.Response.Created($"accounts/{cmd.CustomerId}/{cmd.AccountNo}"))
                            //        .Put<CompleteAccountOpening>("accounts",
                            //            afterDispatch: (cmd, ctx) => ctx.Response.Created($"accounts/{cmd.CustomerId}/{cmd.AccountNo}/complete"))
                            //        .Post<CreateAccount>("accounts",
                            //            afterDispatch: (cmd, ctx) => ctx.Response.Created($"accounts/{cmd.CustomerId}/{cmd.AccountNo}"))
                            //        .Put<ChangeAccountStatus>("accounts",
                            //            afterDispatch: (cmd, ctx) => ctx.Response.Created($"accounts/{cmd.CustomerId}/{cmd.AccountNo}")))
                            .UseJaeger()
                        //     .UseSwaggerDocs()
                        .UseSwaggerWithReverseProxySupport()
                            //.UseRabbitMq()
                            //.SubscribeEvent<PaymentMade>()
                            )
                        .UseLogging();
                //.UseVault();
            });
        }
    }
}
