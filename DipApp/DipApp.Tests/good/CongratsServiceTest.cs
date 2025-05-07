using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DipApp.common;
using DipApp.good;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DipApp.Tests.good;

[TestClass]
[TestSubject(typeof(CongratsService))]
public class CongratsServiceTest
{
    [TestMethod]
    public async Task TestNoUsers()
    {
        var service = new CongratsService(
            fetchUsers: () => Task.FromResult(Enumerable.Empty<User>()),
            fetchPostsForUserId: _ => throw new Exception("Should not fetch posts!"),
            enqueueEmail: _ => throw new Exception("Should not send email!"));

        await service.SendCongratsEmailToActiveUsers();
    }

    [TestMethod]
    public async Task TestNoPosts()
    {
        var service = new CongratsService(
            fetchUsers: () => Task.FromResult(TestData.Users),
            fetchPostsForUserId: _ => Task.FromResult(Enumerable.Empty<Post>()),
            enqueueEmail: _ => throw new Exception("Should not send email!"));

        await service.SendCongratsEmailToActiveUsers();
    }

    [TestMethod]
    public async Task TestSendEmailToActiveUserOnly()
    {
        // Output
        var sentEmails = new List<Email>();

        // When
        var service = new CongratsService(
            fetchUsers: () => Task.FromResult(TestData.Users),
            fetchPostsForUserId: TestData.FetchPostsForUserId,
            enqueueEmail: sentEmails.Add);
        await service.SendCongratsEmailToActiveUsers();

        // Then
        Assert.AreEqual(1, sentEmails.Count);
        Assert.AreEqual("activeUser@bokio.se", sentEmails[0].EmailAddress);
        Assert.AreEqual("Congratulations on being active! You made 3 posts in the last month", sentEmails[0].Msg);
    }
}

internal static class TestData
{
    private static readonly Guid LazyUserId = Guid.NewGuid();
    private static readonly Guid ActiveUserId = Guid.NewGuid();

    public static readonly IEnumerable<User> Users =
    [
        new(id: LazyUserId, name: "lazyUser", emailAddress: "lazyUser@bokio.se"),
        new(id: ActiveUserId, name: "activeUser", emailAddress: "activeUser@bokio.se")
    ];

    private static readonly IEnumerable<Post> Posts =
    [
        new(id: Guid.NewGuid(), DateTime.Now, msg: "Good morning"),
        new(id: Guid.NewGuid(), DateTime.Now, msg: "Hello"),
        new(id: Guid.NewGuid(), DateTime.Now, msg: "How are you")
    ];

    public static Task<IEnumerable<Post>> FetchPostsForUserId(Guid userId)
    {
        if (userId == LazyUserId)
        {
            return Task.FromResult(Enumerable.Empty<Post>());
        }

        if (userId == ActiveUserId)
        {
            return Task.FromResult(Posts);
        }

        throw new Exception("Bad test data");
    }
}