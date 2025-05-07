using DipApp.common;
using DipApp.stubs;

namespace DipApp.bad;

public class CongratsService
{
    public async Task SendCongratsEmailToActiveUsers(DbContext db, RabbitMq emailQueue)
    {
        var users = await db.Users();
        foreach (var user in users)
        {
            await SendCongratsEmailIfActive(db, user, emailQueue);
        }
    }

    private async Task SendCongratsEmailIfActive(DbContext db, User user, RabbitMq emailQueue)
    {
        var posts = await db.Posts(user.Id);

        if (posts.Count(p => p.DateTime > DateTime.Now.AddMonths(-1)) > 10)
        {
            await emailQueue.Enqueue(
                new Email(
                    emailAddress: user.EmailAddress,
                    msg: "congrats " + user.Name));
        }
    }
}