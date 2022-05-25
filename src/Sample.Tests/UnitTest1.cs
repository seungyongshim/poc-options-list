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
using static Sample.Prelude;

namespace Sample.Tests;

public class Business
{
    public string S3Arn { get; set; }
    public IList<string> SnsArn { get; set; }
}

public class PreludeSpec
{
    [Fact]
    public async Task OptionsSpec()
    {
        using var appsettingsStream = Encoding.Default.GetBytes(@"
        {
            'Business' : {
                'S3Arn' : 's3arn',
                'SnsArn' : ['snsarn1', 'snsarn2'],
                'Region' : 'ap-northeast-1'
            }
        }".Replace('\'', '\"')).AsMemory().AsStream();

        var host = Host.CreateDefaultBuilder()
                       .ConfigureAppConfiguration(config => config.AddJsonStream(appsettingsStream))
                       .ConfigureServices(services =>
                       {
                           services.AddOptions<Business>()
                                   .BindConfiguration("Business");
                       })
                       .Build();

        await host.StartAsync();

        var ret = host.Services.GetService<IOptions<Business>>();

        Assert.Equal(2, ret.Value.SnsArn.Count());

        await host.StopAsync();

    }
}
