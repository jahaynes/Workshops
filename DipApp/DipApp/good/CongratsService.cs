using DipApp.common;
using DipApp.stubs;

namespace DipApp.good;

public class CongratsService
{
    private readonly Func<Task<IEnumerable<User>>> _fetchUsers;
    private readonly Func<Guid, Task<IEnumerable<Post>>> _fetchPostsForUserId;
    private readonly Action<Email> _enqueueEmail;

    public CongratsService(DbContext db, RabbitMq emailQueue)
    {
        _fetchUsers = db.Users;
        _fetchPostsForUserId = db.Posts;
        _enqueueEmail = email => emailQueue.Enqueue(email);
    }

    public CongratsService(
        Func<Task<IEnumerable<User>>> fetchUsers,
        Func<Guid, Task<IEnumerable<Post>>> fetchPostsForUserId,
        Action<Email> enqueueEmail)
    {
        _fetchUsers = fetchUsers;
        _fetchPostsForUserId = fetchPostsForUserId;
        _enqueueEmail = enqueueEmail;
    }

    public async Task SendCongratsEmailToActiveUsers()
    {
        var users = await _fetchUsers();
        foreach (var user in users)
        {
            var posts = await _fetchPostsForUserId(user.Id);
            var result = CongratsEmailForUser(user, posts.ToList());
            if (result is IEmailResponse.GeneratedEmail generatedEmail)
            {
                _enqueueEmail(generatedEmail.Email);
            }
        }
    }

    private static IEmailResponse CongratsEmailForUser(User user, List<Post> posts)
    {
        if (posts.Count(p => p.DateTime > DateTime.Now.AddMonths(-1)) >= 3)
        {
            var email = new Email(
                emailAddress: user.EmailAddress,
                msg: $"Congratulations on being active! You made {posts.Count} posts in the last month");

            return new IEmailResponse.GeneratedEmail(email);
        }

        return new IEmailResponse.NoEmail();
    }

    private interface IEmailResponse
    {
        struct GeneratedEmail(Email email) : IEmailResponse
        {
            public readonly Email Email = email;
        }

        struct NoEmail : IEmailResponse;
    }
}