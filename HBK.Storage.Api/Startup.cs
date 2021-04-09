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

namespace HBK.Storage.Api
{
    /// <summary>
    /// �����i�J�I
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ���o�]�w��
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// �����i�J�I�غc�禡
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.EnableEndpointRouting = false;
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
            services.AddScoped<StorageService>();
            services.AddScoped<FileAccessTokenService>();
            services.AddScoped<StorageProviderService>();
            services.AddScoped<StorageGroupService>();
            services.AddScoped<FileEntityService>();
            services.AddSingleton<FileSystemFactory>();

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HBK Storage Api v1"));
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
