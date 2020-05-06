using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;
using System;
using System.IO;
using System.Web.Http;
using System.Web.Http.Cors;

[assembly: OwinStartup(typeof(Bksh_WebScrapping_Api.Startup))]

namespace Bksh_WebScrapping_Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            var cors = new EnableCorsAttribute("*","*","*");
            config.EnableCors(cors);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional});

            // Percaktimi i formatit te te dhenave ne json
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;

            config
                  .EnableSwagger(c => c.SingleApiVersion("v1", "API to collect data from BKSH Website"))
                  .EnableSwaggerUi();

            app.UseWebApi(config);


            string root = AppDomain.CurrentDomain.BaseDirectory;
            var physicalFileSystem = new PhysicalFileSystem(Path.Combine(root, "wwwroot"));

            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem,
                RequestPath = PathString.Empty
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[]
            {
                "index.html",
            };

            app.UseFileServer(options);

            //if (System.Diagnostics.Debugger.IsAttached)
            //    app.Run( async context =>
            //    {
            //        context.Response.Redirect("swagger/ui/index");
            //    });
        }

    }
}