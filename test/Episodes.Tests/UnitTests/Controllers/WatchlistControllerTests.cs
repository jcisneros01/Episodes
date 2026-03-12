using System.Security.Claims;
using Episodes.Controllers;
using Episodes.Data;
using Episodes.Models;
using Episodes.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Episodes.Tests.UnitTests.Controllers;

[TestFixture]
public class WatchlistControllerTests
{
    [SetUp]
    public void SetUp()
    {
        _watchlistService = Substitute.For<IWatchlistService>();
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _sut = new WatchlistController(_watchlistService, _userManager);
    }

    private IWatchlistService _watchlistService;
    private UserManager<ApplicationUser> _userManager;
    private WatchlistController _sut;

    private void SetUserId(string userId)
    {
        var claims = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId)
        ], "Test"));

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claims }
        };

        _userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns(userId);
    }

    private void SetNoUserId()
    {
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        _userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns((string?)null);
    }

    [Test]
    public async Task GetWatchlist_WhenUserAuthenticated_ReturnsOk()
    {
        // Arrange
        SetUserId("1");
        var expected = new List<WatchlistItem>
        {
            new(1396, "Breaking Bad", "/poster.jpg", "Current", DateTime.UtcNow)
        };
        _watchlistService.GetWatchlistAsync(1, Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _sut.GetWatchlist(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(expected);
        await _watchlistService.Received(1).GetWatchlistAsync(1, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetWatchlist_WhenUserIdNotParseable_ReturnsUnauthorized()
    {
        // Arrange
        SetNoUserId();

        // Act
        var result = await _sut.GetWatchlist(CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
        await _watchlistService.DidNotReceive().GetWatchlistAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task AddShow_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        SetUserId("1");
        var expected = new AddShowResult
        {
            Success = true,
            Item = new WatchlistItem(1396, "Breaking Bad", "/poster.jpg", "Current", DateTime.UtcNow)
        };
        _watchlistService.AddShowAsync(1, 1396, Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _sut.AddShow(1396, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(expected.Item);
        await _watchlistService.Received(1).AddShowAsync(1, 1396, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task AddShow_WhenShowNotFound_ReturnsNotFound()
    {
        // Arrange
        SetUserId("1");
        _watchlistService.AddShowAsync(1, 9999, Arg.Any<CancellationToken>())
            .Returns(new AddShowResult { Error = AddShowError.ShowNotFound });

        // Act
        var result = await _sut.AddShow(9999, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task AddShow_WhenAlreadyOnWatchlist_ReturnsConflict()
    {
        // Arrange
        SetUserId("1");
        _watchlistService.AddShowAsync(1, 1396, Arg.Any<CancellationToken>())
            .Returns(new AddShowResult { Error = AddShowError.AlreadyOnWatchlist });

        // Act
        var result = await _sut.AddShow(1396, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ConflictResult>();
    }

    [Test]
    public async Task AddShow_WhenUserIdNotParseable_ReturnsUnauthorized()
    {
        // Arrange
        SetNoUserId();

        // Act
        var result = await _sut.AddShow(1396, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
        await _watchlistService.DidNotReceive()
            .AddShowAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task RemoveShow_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        SetUserId("1");

        // Act
        var result = await _sut.RemoveShow(1396, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        await _watchlistService.Received(1).RemoveShowAsync(1, 1396, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task RemoveShow_WhenUserIdNotParseable_ReturnsUnauthorized()
    {
        // Arrange
        SetNoUserId();

        // Act
        var result = await _sut.RemoveShow(1396, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
        await _watchlistService.DidNotReceive()
            .RemoveShowAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}
