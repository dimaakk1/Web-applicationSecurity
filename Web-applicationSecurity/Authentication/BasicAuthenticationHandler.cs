using Azure.Core;
using BANK.DAL.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BANK.BLL.Services
{
    public class BasicAuthenticationHandler
            : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            UserManager<ApplicationUser> userManager)
            : base(options, logger, encoder)
        {
            _userManager = userManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(
                    Request.Headers.Authorization!);

                if (!authHeader.Scheme.Equals(
                        "Basic",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return AuthenticateResult.Fail(
                        "Invalid authentication scheme");
                }

                if (string.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    return AuthenticateResult.Fail(
                        "Credentials are missing");
                }

                var credentialBytes =
                    Convert.FromBase64String(authHeader.Parameter);

                var credentials =
                    Encoding.UTF8
                        .GetString(credentialBytes)
                        .Split(':', 2);

                if (credentials.Length != 2)
                {
                    return AuthenticateResult.Fail(
                        "Invalid credentials format");
                }

                var username = credentials[0];
                var password = credentials[1];

                var user =
                    await _userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return AuthenticateResult.Fail(
                        "Invalid username or password");
                }

                var validPassword =
                    await _userManager.CheckPasswordAsync(
                        user,
                        password);

                if (!validPassword)
                {
                    return AuthenticateResult.Fail(
                        "Invalid username or password");
                }

                var claims = new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id),

                    new Claim(
                        ClaimTypes.Name,
                        user.UserName!)
                };

                var roles =
                    await _userManager.GetRolesAsync(user);

                foreach (var role in roles)
                {
                    claims.Add(
                        new Claim(
                            ClaimTypes.Role,
                            role));
                }

                var identity =
                    new ClaimsIdentity(
                        claims,
                        Scheme.Name);

                var principal =
                    new ClaimsPrincipal(identity);

                var ticket =
                    new AuthenticationTicket(
                        principal,
                        Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (FormatException)
            {
                return AuthenticateResult.Fail(
                    "Invalid Base64 credentials");
            }
            catch
            {
                return AuthenticateResult.Fail(
                    "Invalid Authorization header");
            }
        }
    }
}
