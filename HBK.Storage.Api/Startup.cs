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
using HBK.Storage.Api.FileProcessHandlers;
using System.IO;
using Microsoft.AspNetCore.HttpOverrides;
using HBK.Storage.Api.FileAccessHandlers;
using Microsoft.IdentityModel.Logging;
using HBK.Storage.Core.Cryptography;

namespace HBK.Storage.Api
{
    /// <summary>
    /// 網站進入點
    /// </summary>
    public class Startup
    {
        private readonly string _corsPolicyName = "_corsPolicy";
        /// <summary>
        /// 取得設定檔
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 取得環境資訊
        /// </summary>
        public IWebHostEnvironment HostingEnvironment { get; }

        /// <summary>
        /// 網站進入點建構函式
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
                    builder.AllowAnyOrigin()
                        .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                        .WithHeaders("Content-Type", "Authorization", "Cache-Control", "X-Requested-With", "Accept", "HBKey");
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
                options.EnableEndpointRouting = false; // AllowAnonymous 在 .Net Core 3.1 會失效 https://blog.csdn.net/elvismile/article/details/104003004
            })
            .AddNewtonsoftJson(options =>
            {
                // 強制使用小寫底線屬性名稱
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
                        // 覆寫 ExistInDatabaseAttribute 驗證失敗的錯誤代碼
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

            // 路由
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

            // IIS 設定
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "HBK Storage Api", Version = "v1" });

                // 加入 XML 註解
                options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, "HBK.Storage.Api.xml"));
                options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, "HBK.Storage.Core.xml"));
                options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, "HBK.Storage.Adapter.xml"));

                // 加入驗證
                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "HBKey",
                    Description = "存取 HBK Storage Api 時需帶入的 API Key",
                });

                options.CustomSchemaIds(SwaggerSchemaIdGenerator.SchemaIdSelector);

                options.OperationFilter<SecurityRequirementsOperationFilter>();
                options.OperationFilter<ModelValidationOperationFilter>();
                options.OperationFilter<ExampleParameterOperationFilter>();
                options.OperationFilter<ODataOperationFilter>();
                options.OperationFilter<FileStreamTypeOperationFilter>();
                options.OperationFilter<HeaderParameterOperationFilter>();
                options.SchemaFilter<FlagEnumSchemaFilter>();
                options.SchemaFilter<EnumDescriptorSchemaFilter>();
                options.SchemaFilter<ExampleValueSchemaFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();

            // Form 請求設定
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });

            // JWT 設定檔
            IConfiguration jwtConfiguration = this.Configuration.GetSection("JWTOption");
            services.AddSingleton<JWTOption>((sp) =>
            {
                JWTOption jwtOption = new JWTOption();
                jwtConfiguration.Bind(jwtOption);
                return jwtOption;
            });

            // 資料庫
            services.AddDbContext<HBKStorageContext>(options =>
                options.UseSqlServer(this.Configuration["Database:ConnectionString"]));

            // 核心邏輯
            services.AddScoped<FileAccessTokenService>();
            services.AddScoped<FileAccessTokenFactory>();
            services.AddHBKStorageService();

            // 存取處理器
            services.AddScoped<FileAccessHandlerProxy>();

            // 檔案處理器
            services.AddScoped<FileProcessHandlerBase, M3U8FileProcessHandler>();
            services.AddScoped<FileProcessHandlerBase, DecryptProcessHandler>();
            services.AddScoped<FileProcessHandlerBase, EncryptProcessHandler>();
            services.AddScoped<FileProcessHandlerBase, WatermarkDefaultProcessHandler>();
            services.AddScoped<FileProcessHandlerProxy>();

            // 加密提供者
            services.AddScoped<ICryptoProvider, AESCryptoProvider>();

            Directory.SetCurrentDirectory(this.HostingEnvironment.ContentRootPath);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="authorizeKeyService"></param>
        /// <param name="storageProviderService"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AuthorizeKeyService authorizeKeyService, StorageProviderService storageProviderService)
        {
            if (env.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseMiddleware<GlobalExceptionMiddleware>();
            }

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HBK Storage Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(_corsPolicyName);

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

            this.GenerateInitializationDataAsync(authorizeKeyService, storageProviderService).Wait();
        }

        private async Task GenerateInitializationDataAsync(AuthorizeKeyService authorizeKeyService, StorageProviderService storageProviderService)
        {
            if (this.Configuration.GetValue<bool>("RootKey:EnsureCreated") && (await authorizeKeyService.FindByKeyValueAsync(this.Configuration["RootKey:Key"])) == null)
            {
                await authorizeKeyService.AddAsync(new AuthorizeKey()
                {
                    KeyValue = this.Configuration["RootKey:Key"],
                    Name = this.Configuration["RootKey:Name"],
                    Type = Adapter.Enums.AuthorizeKeyTypeEnum.Root
                });
            }

            if (this.Configuration.GetValue<bool>("DefaultStorageProvider:EnsureCreated") && (await storageProviderService.FindByIdAsync(this.Configuration.GetValue<Guid>("DefaultStorageProvider:ProviderId"))) == null)
            {
                await storageProviderService.AddAsync(new StorageProvider()
                {
                    StorageProviderId = this.Configuration.GetValue<Guid>("DefaultStorageProvider:ProviderId"),
                    Name = this.Configuration["DefaultStorageProvider:ProviderName"],
                    Status = Adapter.Enums.StorageProviderStatusEnum.None,
                    StorageGroup = new List<StorageGroup>()
                    {
                        new StorageGroup()
                        {
                            StorageGroupId = Guid.NewGuid(),
                            Name = this.Configuration["DefaultStorageProvider:StorageGroupName"],
                            Type = Adapter.Enums.StorageTypeEnum.Local,
                            Status = Adapter.Enums.StorageGroupStatusEnum.None,
                            SyncMode = Adapter.Enums.SyncModeEnum.Never,
                            ClearMode = Adapter.Enums.ClearModeEnum.Stop,
                            UploadPriority = 1,
                            DownloadPriority = 1,
                            Storage = new List<Adapter.Storages.Storage>()
                            {
                                new Adapter.Storages.Storage()
                                {
                                    StorageId = Guid.NewGuid(),
                                    Name =  this.Configuration["DefaultStorageProvider:StorageName"],
                                    SizeLimit = this.Configuration.GetValue<long>("DefaultStorageProvider:SizeLimit"),
                                    Status = Adapter.Enums.StorageStatusEnum.None,
                                    Type = Adapter.Enums.StorageTypeEnum.Local,
                                    Credentials = new Adapter.StorageCredentials.LocalStorageCredentials()
                                    {
                                        StorageType = Adapter.Enums.StorageTypeEnum.Local,
                                        Directory = this.Configuration["DefaultStorageProvider:Location"]
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 建立 OData 的 EDM 模型
        /// </summary>
        /// <param name="serviceProvider">服務提供者</param>
        /// <returns></returns>
        private IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            SnakeCaseNamingStrategy snakeCaseNamingStrategy = new SnakeCaseNamingStrategy();
            var builder = new ODataConventionModelBuilder(serviceProvider, true);

            // Models
            builder.EntitySet<StorageProvider>("StorageProviders");

            builder.EntitySet<ChildFileEntity>("ChildFileEntitys");
            builder.EntitySet<StorageGroupExtendProperty>("StorageGroupExtendProperties");
            builder.EntitySet<StorageExtendProperty>("StorageExtendProperties");

            // ODataConventionModelBuilder 會自動加入相關聯的模型
            builder.OnModelCreating = builder =>
            {
                foreach (var type in builder.EnumTypes.Where(type => type.Namespace == "HBK.Storage.Adapter.Enums" || type.Namespace == "HBK.Storage.Core.Enums"))
                {
                    type.Namespace = "Enums";
                    // 移除 Enum 結尾
                    if (type.Name.EndsWith("Enum"))
                    {
                        type.Name = type.Name[0..^4];
                    }
                    // 改為小寫底線命名
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
