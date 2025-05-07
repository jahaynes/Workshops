namespace DipApp.good;

public class CongratsController(CongratsService congratsService)
{
    public async Task SendCongratsEmailToActiveUsers()
    {
        await congratsService.SendCongratsEmailToActiveUsers();
    }
}