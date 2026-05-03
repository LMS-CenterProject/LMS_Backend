using LMS.Application.Features.Users.Queries.GetAllUsers;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using Moq;

namespace LMS.Application.UnitTests.Features.Users.Queries.GetAllUsers;

[TestClass]
public sealed class GetAllUsersQueryHandlerTests
{
    [TestMethod]
    public async Task Handle_WithNoSearch_ReturnsPagedUsers()
    {
        var ct = CancellationToken.None;

        var firstUser = User.CreateLocal("Alice Admin", "alice@example.com", "hashed-password", "01000000000");
        firstUser.ChangeRole(UserRole.Admin);

        var secondUser = User.CreateLocal("Ibrahim Instructor", "ibrahim@example.com", "hashed-password", "01111111111");
        secondUser.ChangeRole(UserRole.Instructor);
        secondUser.Deactivate();

        var usersRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        usersRepositoryMock
            .Setup(r => r.GetAllPaginatedAsync(1, 50, null, UserRole.Instructor, ct))
            .ReturnsAsync((new[] { firstUser, secondUser }, 2))
            .Verifiable();

        var unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWorkMock.SetupGet(u => u.Users).Returns(usersRepositoryMock.Object);

        var handler = new GetAllUsersQueryHandler(unitOfWorkMock.Object);

        var result = await handler.Handle(
            new GetAllUsersQuery(Page: 0, PageSize: 999, Search: null, Role: UserRole.Instructor),
            ct);

        Assert.IsTrue(result.IsSuccess);

        var payload = result.Value;
        var returnedUsers = payload.Users.ToList();

        Assert.AreEqual(2, payload.TotalCount);
        Assert.AreEqual(1, payload.Page);
        Assert.AreEqual(50, payload.PageSize);
        Assert.AreEqual(1, payload.TotalPages);

        Assert.AreEqual(2, returnedUsers.Count);
        Assert.AreEqual(firstUser.Id, returnedUsers[0].Id);
        Assert.AreEqual("Alice Admin", returnedUsers[0].FullName);
        Assert.AreEqual("Admin", returnedUsers[0].Role);
        Assert.IsTrue(returnedUsers[0].IsActive);

        Assert.AreEqual(secondUser.Id, returnedUsers[1].Id);
        Assert.AreEqual("Ibrahim Instructor", returnedUsers[1].FullName);
        Assert.AreEqual("Instructor", returnedUsers[1].Role);
        Assert.IsFalse(returnedUsers[1].IsActive);

        usersRepositoryMock.VerifyAll();
    }
}
