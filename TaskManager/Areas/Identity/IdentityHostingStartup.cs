using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(TaskManager.Areas.Identity.IdentityHostingStartup))]
namespace TaskManager.Areas.Identity
{
  public class IdentityHostingStartup : IHostingStartup
  {
    public void Configure(IWebHostBuilder builder)
    {
      builder.ConfigureServices((context, services) => { });
    }
  }
}