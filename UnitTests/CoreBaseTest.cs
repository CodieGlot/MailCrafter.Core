﻿using MailCrafter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests;
public abstract class CoreBaseTest
{
    protected IServiceProvider ServiceProvider { get; }
    protected CoreBaseTest()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(@"C:\MailCrafter\Development\Core\appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();

        ServiceProvider = new ServiceCollection()
            .AddCoreServices()
            .AddSingleton<IConfiguration>(configuration)
            .BuildServiceProvider();
    }
}