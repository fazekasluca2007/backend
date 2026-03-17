using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace EcoTripBackendTest
{
    public class BookingTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BookingTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Foglalások lekérése token nélkül
        [Fact]
        public async Task GetMyBookings_WithoutToken()
        {
            // Endpoint meghívása token nélkül
            var response = await _client.GetAsync("/api/bookings/my");

            // Ellenőrzés
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        // Foglalások lekérése token-nel
        [Fact]
        public async Task GetMyBookings_WithToken()
        {
            // Regisztráció
            var register = new
            {
                username = "bookinguser",
                email = "booking@test.com",
                password = "123456",
                fullName = "Booking User"
            };

            await _client.PostAsJsonAsync("/api/auth/register", register);

            // Bejelentkezés, hogy megkapjuk a token-t
            var login = new
            {
                username = "bookinguser",
                password = "123456"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", login);
            var content = await loginResponse.Content.ReadAsStringAsync();

            // Token megszerzése
            var token = content.Split("\"token\":\"")[1].Split("\"")[0];

            // Token elküldése
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Edpoint meghívása
            var response = await _client.GetAsync("/api/bookings/my");

            // Ellenőrzés
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


    }
}
