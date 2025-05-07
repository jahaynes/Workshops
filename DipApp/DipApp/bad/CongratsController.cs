using DipApp.stubs;

namespace DipApp.bad;

public class CongratsController(DbContext db, RabbitMq emailQueue, CongratsService congratsService)
{
    public async Task SendCongratsEmailToActiveUsers()
    {
        await congratsService.SendCongratsEmailToActiveUsers(db, emailQueue);
    }
}