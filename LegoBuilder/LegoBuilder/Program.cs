using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LegoBuilder
{
    public class Program
    {
        // this uses the created host builder property and then builds and runs it, whatever that means
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // this is an object of type IHostBuilder being instantiated I think???
        // the fat arrow is saying when I instantiate it, return the Host class/object into the CreateHostBuilder variable
        // the Host object needs to have default stuff in it, that is what CreateDefaultBuilder does
                    //      this is where the appsettings json is picked up - depending on environment.
                    //      Though I think this might happen in startup's iConfiguration in this case
                    //      and where the program picks up environment information, and sets up a log
        // and then once it has defaults, we provide it with custom stuff
        //          configure web host defaults sets up web hosting info (the http stuff)
        // and the custom components like our JWT are added
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
