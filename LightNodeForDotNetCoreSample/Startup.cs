using LightNode;
using LightNode.Formatter;
using LightNode.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;

namespace LightNodeForDotNetCoreSample
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
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Map("/api", builder =>
            {
                builder.UseLightNode(typeof(Startup));

                builder.UseLightNode(new LightNodeOptions(AcceptVerbs.Get | AcceptVerbs.Post, new JsonContentFormatter())
                {
                    ParameterEnumAllowsFieldNameParse = true, // If you want to use enums human readable display on Swagger, set to true
                    ErrorHandlingPolicy = ErrorHandlingPolicy.ReturnInternalServerErrorIncludeErrorDetails,
                    OperationMissingHandlingPolicy = OperationMissingHandlingPolicy.ReturnErrorStatusCodeIncludeErrorDetails
                });
            });

            app.Map("/swagger", builder =>
            {
                var xmlName = "LightNode.Sample.GlimpseUse.xml";
                var xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), xmlName);

                builder.UseLightNodeSwagger(new LightNode.Swagger.SwaggerOptions("AspNetCoreSample", "/api")
                {
                    XmlDocumentPath = xmlPath,
                    IsEmitEnumAsString = true
                });
            });
        }

        public class Toriaezu : LightNodeContract
        {
            public string Echo(string x)
            {
                return x;
            }
        }


        /// <summary>
        /// aaa
        /// </summary>
        public class MyClass : LightNodeContract
        {
            /// <summary>
            /// HogeHoge
            /// </summary>
            public string Hoge(string i)
            {
                return "hogehogehoge!!!";
            }

            [Get]
            public int[] ArraySendTestGet(int[] xs)
            {
                return xs;
            }

            [Post]
            public int[] ArraySendTestPost(int[] xs)
            {
                return xs;
            }
        }
    }
}
