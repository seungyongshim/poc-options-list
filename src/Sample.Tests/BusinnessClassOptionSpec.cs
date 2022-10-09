using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.HighPerformance;
using Xunit;

namespace Sample.Tests;

public class Business
{
    public string S3Arn { get; set; }
    public IList<string> SnsArn { get; set; }
}

public class BusinnessClassOptionSpec
{
    [Fact]
    public async Task OptionsSpec()
    {
        using var appsettingsStream = Encoding.Default.GetBytes("""
        {
            "Business" : {
                "S3Arn" : "s3arn",
                "SnsArn" : ["snsarn1", "snsarn2"],
                "Region" : "ap-northeast-1"
            }
        }
        """).AsMemory().AsStream();

        var host = Host.CreateDefaultBuilder()
                       .ConfigureAppConfiguration(config => config.AddJsonStream(appsettingsStream))
                       .ConfigureServices(services =>
                       {
                           services.AddOptions<Business>()
                                   .BindConfiguration("Business")
                                   .PostConfigure(o =>
                                   {
                                       o.S3Arn = "Hello";
                                   });
                       })
                       .Build();

        await host.StartAsync();

        var ret = host.Services.GetService<IOptions<Business>>();

        Assert.Equal(2, ret.Value.SnsArn.Count);
        Assert.Equal("Hello", ret.Value.S3Arn);

        await host.StopAsync();

    }
}
