using Microsoft.Extensions.Configuration;
using ApiTests.ApiClient;
using System;
using System.Threading.Tasks;


public class ApiTestFixture : IDisposable
{
    public ApiClient Client { get; }
    public IConfiguration Config { get; }

    public ApiTestFixture()
    {
        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Client = new ApiClient(Config);
    }

    public void Dispose()
    {
        Client?.Dispose();
    }
}
