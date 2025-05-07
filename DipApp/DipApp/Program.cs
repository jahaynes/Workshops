using DipApp.bad;
using DipApp.stubs;

namespace DipApp;

public abstract class Program
{
    private static async Task Main()
    {
        var controller = new CongratsController(new DbContext(), new RabbitMq(), new CongratsService());
        await controller.SendCongratsEmailToActiveUsers();
        Console.WriteLine("Done");
    }
}


