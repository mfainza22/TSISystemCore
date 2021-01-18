using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace TSISystemCore
{
    public class Program
    {


        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
          return  Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://0.0.0.0:5000", "https://0.0.0.0:5001");
                    webBuilder.UseStartup<Startup>();
                });
        }


    }
}
