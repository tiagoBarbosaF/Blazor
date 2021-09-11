using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApplication01
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      var hostname = builder.Configuration["Hostname"];
      var http = new HttpClient()
      {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
      };
      var vehicleData = new Dictionary<string, string>()
      {
        {"color", "blue"},
        {"type", "car"},
        {"wheels:count", "3"},
        {"wheels:brand", "Blazin"},
        {"wheels:brand:type", "rally"},
        {"wheels:year", "2008"},
      };

      var memoryConfig = new MemoryConfigurationSource {InitialData = vehicleData};

      builder.Configuration.Add(memoryConfig);
      builder.Services.AddScoped(sp => http);

      using var response = await http.GetAsync("cars.json");
      using var stream = await response.Content.ReadAsStreamAsync();

      builder.Configuration.AddJsonStream(stream);

      builder.RootComponents.Add<App>("#app");

      builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

      builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

      await builder.Build().RunAsync();
    }
  }
}