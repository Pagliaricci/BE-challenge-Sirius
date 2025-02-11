using Xunit;
using Moq;
using FluentAssertions;
using EmailService.Modules.Stats.Models;
using EmailService.Modules.Stats.Services;
using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class StatsServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly StatsService _statsService;
    private readonly List<User> _users;

    public StatsServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _statsService = new StatsService(_userRepositoryMock.Object);
        _users = TestData.GetTestUsers();
    }

    [Fact]
    public async Task GetAllStatsAsync_ShouldReturnStatsForUsersWithEmailsSent()
    {
        _users[0].LastEmailReset = DateTime.UtcNow;
        _users[0].EmailsSentToday = 5;
        _users[1].EmailsSentToday = 0;
        _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(_users);
        _userRepositoryMock.Setup(repo => repo.GetUsersWithEmailsSentAsync()).ReturnsAsync(new List<User> { _users[0] });
        var stats = await _statsService.GetAllStatsAsync();

        stats.Should().HaveCount(1);
        stats[0].Username.Should().Be(_users[0].Username);
        stats[0].EmailsSentToday.Should().Be(5);
    }

    [Fact]
    public async Task GetAllStatsAsync_ShouldReturnEmptyList_WhenNoUsersHaveSentEmails()
    {
        _users.ForEach(user => user.EmailsSentToday = 0);
        _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(_users);
        var stats = await _statsService.GetAllStatsAsync();
        stats.Should().BeEmpty();
    }
    
}
