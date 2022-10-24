using Autofac;
using Localstack.S3;
using Localstack.S3.Console;

static class Program
{
    private static IContainer CompositionRoot()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<Application>();
        builder.RegisterModule<S3Module>();
        return builder.Build();
    }

    public static async Task Main()  //Main entry point
    {
        var app = CompositionRoot().Resolve<Application>();
        await app.Run();
    }
}