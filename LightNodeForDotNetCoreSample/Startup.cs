using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightNode;
using LightNode.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LightNode;

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
            app.UseLightNode(typeof(Startup));
            //app.Map("/api", builder =>
            //{

            //    builder.UseLightNode(typeof(Startup));
            //});

            //app.Map("/swagger", builder =>
            //{
            //    var xmlName = "AspNetCoreSample.xml";
            //    var xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), xmlName);

            //    builder.UseLightNodeSwagger(new LightNode.Swagger.SwaggerOptions("AspNetCoreSample", "/api")
            //    {
            //        XmlDocumentPath = xmlPath,
            //        IsEmitEnumAsString = true
            //    });
            //});
        }
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
