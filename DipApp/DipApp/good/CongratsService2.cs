using DipApp.common;
using DipApp.stubs;

namespace DipApp.good;

public class CongratsServiceDelegateStyle
{
    private readonly FetchUsers _fetchUsers;
    private readonly FetchPostsForUserId _fetchPostsForUserId;
    private readonly EnqueueEmail _enqueueEmail;

    public CongratsServiceDelegateStyle(DbContext db, RabbitMq emailQueue)
    {
        _fetchUsers = db.Users;
        _fetchPostsForUserId = db.Posts;
        _enqueueEmail = email => emailQueue.Enqueue(email);
    }

    public CongratsServiceDelegateStyle(
        FetchUsers fetchUsers,
        FetchPostsForUserId fetchPostsForUserId,
        EnqueueEmail enqueueEmail)
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

    public delegate Task<IEnumerable<User>> FetchUsers();

    public delegate Task<IEnumerable<Post>> FetchPostsForUserId(Guid id);

    public delegate void EnqueueEmail(Email email);
}