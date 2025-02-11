using EmailService.Data;
using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class UserRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    public UserRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
    }

    private AppDbContext CreateDbContext() => new AppDbContext(_dbContextOptions);

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var user = new User { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = "hashedpassword" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var result = await repository.GetUserByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetUserByIdAsync(1));
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var users = new List<User>
        {
            new() { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = "hashedpassword" },
            new() { Id = 2, Username = "user2", Email = "user2@example.com", Role = "User", PasswordHash = "hashedpassword" }
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        var result = await repository.GetAllUsersAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Username == "user1");
        Assert.Contains(result, u => u.Username == "user2");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldAddUser()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var createUser = new CreateUser { Username = "user1", Password = "password", Role = "Admin", Email = "user1@example.com" };

        var result = await repository.CreateUserAsync(createUser);

        Assert.NotNull(result);
        Assert.Equal(createUser.Username, result.Username);
        Assert.Equal(createUser.Email, result.Email);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUser()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var user = new User { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = "hashedpassword" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        user.Username = "updatedUser";
        var result = await repository.UpdateUserAsync(user);

        Assert.True(result);
        var updatedUser = await context.Users.FindAsync(1);
        Assert.Equal("updatedUser", updatedUser.Username);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldRemoveUser()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var user = new User { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = "hashedpassword" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var result = await repository.DeleteUserAsync(1);

        Assert.True(result);
        var deletedUser = await context.Users.FindAsync(1);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task FindUserByUsernameAndPassword_ShouldReturnUser_WhenCredentialsAreValid()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password");
        var user = new User { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = passwordHash };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var result = await repository.FindUserByUsernameAndPassword("user1", "password");

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task FindUserByUsernameAndPassword_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password");
        var user = new User { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = passwordHash };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var result = await repository.FindUserByUsernameAndPassword("user1", "wrongpassword");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var user = new User { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = "hashedpassword" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var result = await repository.GetUserByEmailAsync("user1@example.com");

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);

        var result = await repository.GetUserByEmailAsync("nonexistent@example.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUsersWithEmailsSentAsync_ShouldReturnUsers_WhenEmailsSentTodayGreaterThanZero()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var users = new List<User>
        {
            new User { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = "hashedpassword", EmailsSentToday = 5 },
            new User { Id = 2, Username = "user2", Email = "user2@example.com", Role = "User", PasswordHash = "hashedpassword", EmailsSentToday = 0 }
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        var result = await repository.GetUsersWithEmailsSentAsync();

        Assert.Single(result);
        Assert.Equal("user1", result[0].Username);
    }

    [Fact]
    public async Task UpdateUsersAsync_ShouldUpdateMultipleUsers()
    {
        using var context = CreateDbContext();
        var repository = new UserRepository(context);
        var users = new List<User>
        {
            new User { Id = 1, Username = "user1", Email = "user1@example.com", Role = "Admin", PasswordHash = "hashedpassword" },
            new User { Id = 2, Username = "user2", Email = "user2@example.com", Role = "User", PasswordHash = "hashedpassword" }
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        users[0].Username = "updatedUser1";
        users[1].Username = "updatedUser2";
        var result = await repository.UpdateUsersAsync(users);

        Assert.True(result);
        var updatedUser1 = await context.Users.FindAsync(1);
        var updatedUser2 = await context.Users.FindAsync(2);
        Assert.Equal("updatedUser1", updatedUser1.Username);
        Assert.Equal("updatedUser2", updatedUser2.Username);
    }
}