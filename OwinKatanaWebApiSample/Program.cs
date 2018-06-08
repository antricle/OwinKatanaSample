using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinKatanaWebApiSample
{
    using Microsoft.Owin.Hosting;
    using System.IO;
    using System.Web.Http;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class Program
    {
        private static void Main(string[] args)
        {
            var uri = "http://localhost:8910";

            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine($"### Server started at {uri}");
                Console.WriteLine("### Press anykey to shut down server.");
                Console.ReadKey();
                Console.WriteLine("### Server shutting down.");
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureWebApi(app);

            app.Use<Middleware1>();

            app.Use((context, next) => context.Response.WriteAsync("This is not hello."));



        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            Console.WriteLine("Start of WebApi");

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            app.UseWebApi(config);
            Console.WriteLine("End of WebApi");

        }
    }

    public class Middleware1
    {
        private readonly AppFunc _next;

        public Middleware1(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            Console.WriteLine("Start of Middleware1");
            if (environment["owin.RequestPath"].ToString().Contains("hello"))
            {
                using (var writer = new StreamWriter((Stream)environment["owin.ResponseBody"]))
                {
                    await writer.WriteAsync("Salutations.");
                }
            }
            else
            {
                await _next.Invoke(environment);
            }
            Console.WriteLine("End of Middleware1");
        }
    }

}
