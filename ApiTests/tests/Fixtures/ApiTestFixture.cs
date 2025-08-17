using Microsoft.Extensions.Configuration;
using ApiTests.ApiClient;
using System;
using System.Threading.Tasks;

//Manages test setup and teardown for all tests.
public class ApiTestFixture : IDisposable // Implements IDisposable to allow cleanup
{
    public ApiClient Client { get; }
    public IConfiguration Config { get; }

    public ApiTestFixture()
    {
        //Reads appsettings.json via ConfigurationBuilder
        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        //Creates and exposes a single instance of ApiClient
        Client = new ApiClient(Config);
    }

    public void Dispose()
    {
        Client?.Dispose();
    }
}
