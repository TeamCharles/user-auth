using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using user_auth.Models;

namespace user_auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ApplicationUser user = new ApplicationUser();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();

        }
    }
}
