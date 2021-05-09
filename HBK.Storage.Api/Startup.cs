using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.Swagger;
using HBK.Storage.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http.Features;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Models;
using HBK.Storage.Core;
using HBK.Storage.Api.Middlewares;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.AspNet.OData.Builder;
using System.Reflection;
using HBK.Storage.Adapter.DataAnnotations;
using Microsoft.OData.UriParser;
using Microsoft.OData;
using Microsoft.AspNet.OData.Query.Expressions;
using HBK.Storage.Api.OData;
using HBK.Storage.Api.Factories;
using HBK.Storage.Api.FileAccessHandlers;

namespace HBK.Storage.Api
{
    /// <summary>
    /// �����i�J�I
    /// </summary>
    public class Startup
    {
        private readonly string _corsPolicyName = "_corsPolicy";
        /// <summary>
        /// ���o�]�w��
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// ���o���Ҹ�T
        /// </summary>
        public IWebHostEnvironment HostingEnvironment { get; }

        /// <summary>
        /// �����i�J�I�غc�禡
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.HostingEnvironment = environment;

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy(_corsPolicyName, builder =>
                {
                    builder.WithOrigins(this.Configuration.GetSection("CorsOrigins").Get<string[]>())
                        .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                        .WithHeaders("Content-Type", "Authorization", "Cache-Control", "X-Requested-With", "Accept");
                });
            });
            // HSTS
            if (!this.HostingEnvironment.IsDevelopment())
            {
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(60);
                });
            }

            services.AddControllers(options =>
            {
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.EnableEndpointRouting = false; // AllowAnonymous �b .Net Core 3.1 �|���� https://blog.csdn.net/elvismile/article/details/104003004
            })
            .AddNewtonsoftJson(options =>
            {
                // �j��ϥΤp�g���u�ݩʦW��
                var snakeCaseNamingStrategy = new SnakeCaseNamingStrategy();
                options.SerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = snakeCaseNamingStrategy };
                options.SerializerSettings.Converters.Add(new StringEnumConverter(snakeCaseNamingStrategy));
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    if (actionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        // �мg ExistInDatabaseAttribute ���ҥ��Ѫ����~�N�X
                        var parameters = controllerActionDescriptor.MethodInfo.GetParameters().Select(param => param.Name).ToList();
                        var invalidNames = actionContext.ModelState
                            .Where(state => state.Value.ValidationState == ModelValidationState.Invalid)
                            .Select(kvp => kvp.Key);
                        if (invalidNames.Any(name => parameters.Contains(name)))
                        {
                            return new NotFoundResult(); // 404
                        }
                    }
                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            // ����
            services.AddRouting(options => options.LowercaseUrls = true);

            // ODATA
            services.AddOData();
            services.AddMvcCore(options =>
            {
                // Swagger workaround https://github.com/OData/WebApi/issues/1177#issuecomment-358659774
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(formatter => formatter.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(formatter => formatter.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });

            // IIS �]�w
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "HBK Storage Api", Version = "v1" });

                // �[�J XML ����
                options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, $"HBK.Storage.Api.xml"));
                options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, $"HBK.Storage.Core.xml"));
                options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, $"HBK.Storage.Adapter.xml"));

                // �[�J����
                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "HBKey",
                    Description = "�s�� HBK Storage Api �ɻݱa�J�� API Key",
                });

                options.CustomSchemaIds(SwaggerSchemaIdGenerator.SchemaIdSelector);

                options.OperationFilter<SecurityRequirementsOperationFilter>();
                options.OperationFilter<ModelValidationOperationFilter>();
                options.OperationFilter<ExampleParameterOperationFilter>();
                options.OperationFilter<ODataOperationFilter>();
                options.SchemaFilter<FlagEnumSchemaFilter>();
                options.SchemaFilter<EnumDescriptorSchemaFilter>();
                options.SchemaFilter<ExampleValueSchemaFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();

            // Form �ШD�]�w
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });

            // JWT �]�w��
            IConfiguration jwtConfiguration = this.Configuration.GetSection("JWTOption");
            services.AddSingleton<JWTOption>((sp) =>
            {
                JWTOption jwtOption = new JWTOption();
                jwtConfiguration.Bind(jwtOption);
                return jwtOption;
            });

            // ��Ʈw
            services.AddDbContext<HBKStorageContext>(options =>
                options.UseSqlServer(this.Configuration["Database:ConnectionString"]));

            // �֤��޿�
            services.AddScoped<FileAccessTokenService>();
            services.AddScoped<FileAccessTokenFactory>();
            services.AddHBKStorageService();

            // �ɮ׳B�z��
            services.AddScoped<FileAccessHandlerBase, M3U8FileAccessHandler>();
            services.AddScoped<FileAccessHandlerProxy>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="db"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, HBKStorageContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseMiddleware<GlobalExceptionMiddleware>();
            }

            if (bool.Parse(this.Configuration["UseSwagger"].ToString()))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HBK Storage Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                var edmModel = this.GetEdmModel(app.ApplicationServices);
                endpoints.MapControllers();
                // OData
                endpoints.Filter().OrderBy().MaxTop(100);
                endpoints.EnableDependencyInjection(action =>
                {
                    action.AddService<IEdmModel>(Microsoft.OData.ServiceLifetime.Singleton, sp => edmModel);
                    action.AddService<ODataUriResolver>(Microsoft.OData.ServiceLifetime.Singleton, sp => new SnakeCaseODataUriResolver());
                    action.AddService<FilterBinder>(Microsoft.OData.ServiceLifetime.Transient, sp => new SnakeCaseFilterBinder(sp));
                });
                if (env.IsDevelopment() || env.IsStaging())
                {
                    endpoints.MapODataRoute("odata", "odata", edmModel);
                }
            });

            db.Database.EnsureCreated();
        }

        /// <summary>
        /// �إ� OData �� EDM �ҫ�
        /// </summary>
        /// <param name="serviceProvider">�A�ȴ��Ѫ�</param>
        /// <returns></returns>
        private IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            SnakeCaseNamingStrategy snakeCaseNamingStrategy = new SnakeCaseNamingStrategy();
            var builder = new ODataConventionModelBuilder(serviceProvider, true);

            // Models
            builder.EntitySet<StorageProvider>("StorageProviders");

            // ODataConventionModelBuilder �|�۰ʥ[�J�����p���ҫ�
            builder.OnModelCreating = builder =>
            {
                foreach (var type in builder.EnumTypes.Where(type => type.Namespace == "HBK.Storage.Adapter.Enums" || type.Namespace == "HBK.Storage.Core.Enums"))
                {
                    type.Namespace = "Enums";
                    // ���� Enum ����
                    if (type.Name.EndsWith("Enum"))
                    {
                        type.Name = type.Name[0..^4];
                    }
                    // �אּ�p�g���u�R�W
                    foreach (var member in type.Members)
                    {
                        member.Name = snakeCaseNamingStrategy.GetPropertyName(member.Name, false);
                    }
                }

                foreach (var property in builder.StructuralTypes.SelectMany(type => type.Properties))
                {
                    property.NotCountable = true;
                    property.NotExpandable = true;
                    property.NotNavigable = true;
                    property.NotSortable = (property.PropertyInfo.GetCustomAttribute<SortableAttribute>() == null);
                    property.NotFilterable = (property.PropertyInfo.GetCustomAttribute<FilterableAttribute>() == null);
                }
            };

            return builder.GetEdmModel();
        }
    }
}
