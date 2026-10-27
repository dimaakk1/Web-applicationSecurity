using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using BANK.DAL.Data;
using BANK.DAL.Entity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Web_applicationSecurity.Tests;

public sealed class AccessTests : IDisposable
{
    private const string Accounts = "/api/BankAccounts";
    private readonly BankApiFactory _factory = new();

    [Fact(DisplayName = "01 Admin GET accounts -> 200")]
    public Task AdminCanReadAccounts() => AssertCanReadAsync("admin", "Admin123!");

    [Fact(DisplayName = "02 Manager GET accounts -> 200")]
    public Task ManagerCanReadAccounts() => AssertCanReadAsync("manager", "Manager123!");

    [Fact(DisplayName = "03 User GET accounts -> 200")]
    public Task UserCanReadAccounts() => AssertCanReadAsync("user", "User123!");

    [Fact(DisplayName = "04 Admin POST accounts -> 201")]
    public Task AdminCanCreateAccount() => AssertCreateAsync("admin", "Admin123!", HttpStatusCode.Created);

    [Fact(DisplayName = "05 Manager POST accounts -> 201")]
    public Task ManagerCanCreateAccount() => AssertCreateAsync("manager", "Manager123!", HttpStatusCode.Created);

    [Fact(DisplayName = "06 User POST accounts -> 403")]
    public Task UserCannotCreateAccount() => AssertCreateAsync("user", "User123!", HttpStatusCode.Forbidden);

    [Fact(DisplayName = "07 Admin PUT account -> 204")]
    public Task AdminCanUpdateAccount() => AssertUpdateAsync("admin", "Admin123!", HttpStatusCode.NoContent);

    [Fact(DisplayName = "08 User PUT account -> 403")]
    public Task UserCannotUpdateAccount() => AssertUpdateAsync("user", "User123!", HttpStatusCode.Forbidden);

    [Fact(DisplayName = "09 Manager DELETE account -> 403")]
    public Task ManagerCannotDeleteAccount() => AssertDeleteAsync("manager", "Manager123!", HttpStatusCode.Forbidden);

    [Fact(DisplayName = "10 Admin DELETE account -> 204")]
    public Task AdminCanDeleteAccount() => AssertDeleteAsync("admin", "Admin123!", HttpStatusCode.NoContent);

    private HttpClient CreateClient(string username, string password)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
        return client;
    }

    private async Task AddCsrfTokenAsync(HttpClient client)
    {
        using var response = await client.GetAsync(Accounts);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var token = Assert.Single(response.Headers.GetValues("X-CSRF-TOKEN"));
        Assert.False(string.IsNullOrWhiteSpace(token));
        client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);
        // The same client retains the matching antiforgery cookie.
    }

    private async Task AssertCanReadAsync(string username, string password)
    {
        using var client = CreateClient(username, password);
        var account = await SeedAccountAsync();
        using var response = await client.GetAsync(Accounts);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<BankAccount>>();
        Assert.NotNull(body);
        Assert.Contains(body, item => item.Id == account.Id && item.OwnerName == account.OwnerName);
    }

    private async Task AssertCreateAsync(string username, string password, HttpStatusCode expected)
    {
        using var client = CreateClient(username, password);
        await AddCsrfTokenAsync(client);
        using var response = await client.PostAsJsonAsync(Accounts,
            new BankAccount { OwnerName = "Created account", Balance = 5000 });
        Assert.Equal(expected, response.StatusCode);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (expected == HttpStatusCode.Created)
        {
            var body = await response.Content.ReadFromJsonAsync<BankAccount>();
            Assert.NotNull(body);
            Assert.EndsWith($"{Accounts}/{body.Id}", response.Headers.Location?.ToString());
            var saved = Assert.Single(db.BankAccounts);
            Assert.Equal(body.Id, saved.Id);
            Assert.Equal("Created account", saved.OwnerName);
            Assert.Equal(5000m, saved.Balance);
        }
        else
        {
            Assert.Empty(db.BankAccounts);
        }
    }

    private async Task AssertUpdateAsync(string username, string password, HttpStatusCode expected)
    {
        using var client = CreateClient(username, password);
        var account = await SeedAccountAsync();
        await AddCsrfTokenAsync(client);
        using var response = await client.PutAsJsonAsync($"{Accounts}/{account.Id}",
            new BankAccount { OwnerName = "Updated account", Balance = 8000 });
        Assert.Equal(expected, response.StatusCode);
        using var scope = _factory.Services.CreateScope();
        var saved = await scope.ServiceProvider.GetRequiredService<AppDbContext>()
            .BankAccounts.FindAsync(account.Id);
        Assert.NotNull(saved);
        Assert.Equal(expected == HttpStatusCode.NoContent ? "Updated account" : account.OwnerName, saved.OwnerName);
        Assert.Equal(expected == HttpStatusCode.NoContent ? 8000m : account.Balance, saved.Balance);
    }

    private async Task AssertDeleteAsync(string username, string password, HttpStatusCode expected)
    {
        using var client = CreateClient(username, password);
        var account = await SeedAccountAsync();
        await AddCsrfTokenAsync(client);
        using var response = await client.DeleteAsync($"{Accounts}/{account.Id}");
        Assert.Equal(expected, response.StatusCode);
        using var scope = _factory.Services.CreateScope();
        var saved = await scope.ServiceProvider.GetRequiredService<AppDbContext>()
            .BankAccounts.FindAsync(account.Id);
        if (expected == HttpStatusCode.NoContent)
            Assert.Null(saved);
        else
        {
            Assert.NotNull(saved);
            Assert.Equal(account.OwnerName, saved.OwnerName);
            Assert.Equal(account.Balance, saved.Balance);
        }
    }

    private async Task<BankAccount> SeedAccountAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var account = new BankAccount { OwnerName = "Original account", Balance = 1000 };
        db.BankAccounts.Add(account);
        await db.SaveChangesAsync();
        return account;
    }

    public void Dispose() => _factory.Dispose();
}
