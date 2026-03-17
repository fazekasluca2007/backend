using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EcoTripBackendTest
{
    public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }



        // Bejelentkezés valós adatokkal
        [Fact]
        public async Task Login_WithValidCredentials()
        {
            // Regisztráció a teszthez
            var register = new
            {
                username = "testuser",
                email = "test@test.com",
                password = "123456",
                fullName = "Test User"
            };

            await _client.PostAsJsonAsync("/api/auth/register", register);

            // Bejelentkezés
            var login = new
            {
                username = "testuser",
                password = "123456"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", login);

            // Ellenõrzés
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("token");
        }


        // Bejelentkezés rossz adatokkal
        [Fact]
        public async Task Login_WithInvalidCredentials()
        {
            // Bejelentkezés rossz adatokkal
            var login = new
            {
                username = "rossz",
                password = "123"
            };

            // Ellenõrzés
            var response = await _client.PostAsJsonAsync("/api/auth/login", login);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }


    }
}