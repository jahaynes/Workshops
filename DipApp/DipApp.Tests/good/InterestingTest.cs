using System.Collections.Generic;
using System.Threading.Tasks;
using DipApp.common;
using DipApp.good;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DipApp.Tests.good;

[TestClass]
[TestSubject(typeof(CongratsService))]
public class InterestingTest
{
    [TestMethod]
    public async Task TestController()
    {
        // Output
        var sentEmails = new List<Email>();

        // When
        var controller = new CongratsController(congratsService:
            new CongratsService(
                fetchUsers: () => Task.FromResult(TestData.Users),
                fetchPostsForUserId: TestData.FetchPostsForUserId,
                enqueueEmail: sentEmails.Add));

        await controller.SendCongratsEmailToActiveUsers();

        // Then
        Assert.AreEqual(1, sentEmails.Count);
        Assert.AreEqual("activeUser@bokio.se", sentEmails[0].EmailAddress);
        Assert.AreEqual("Congratulations on being active! You made 3 posts in the last month", sentEmails[0].Msg);
    }
/*
    [TestMethod]
    public async Task TestExceptions()
    {
        var controller = new CongratsController(congratsService:
            new CongratsService(
                fetchUsers: () => Task.FromResult(TestData.Users),
                fetchPostsForUserId: TestData.FetchPostsForUserId,
                enqueueEmail: _ => throw new Exception("Problem sending email")));

        await controller.SendCongratsEmailToActiveUsers();
    }
    */
}