using PhoneAuthDemo;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseIISIntegration();
        webBuilder.UseStartup<Startup>();
    }).Build().Run();