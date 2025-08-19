using Xunit;
using FluentAssertions;
using ApiTests.ApiClient;
using ApiTests.Models;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace ApiTests.Tests.Albums
{
    //Uses IClassFixture<ApiTestFixture> to inject a shared ApiClient for making HTTP calls
    public class AlbumsTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTests.ApiClient.ApiClient _client;

        public AlbumsTests(ApiTestFixture fixture)
        {
            _client = fixture.Client;
        }

        //get test (all)
        [Fact]
        public async Task Get_All_Albums_Should_Return_List()
        {
            var albums = await _client.GetAsync<List<Album>>("albums");

            albums.Should().NotBeNull();
            albums.Should().HaveCountGreaterThan(0);
            albums[0].id.Should().BePositive();
        }

        //get test (specific id)
        [Fact]
        public async Task Get_Album_By_Id_Should_Return_Album()
        {
            var album = await _client.GetAsync<Album>("albums/1");

            album.Should().NotBeNull();
            album!.id.Should().Be(1);
            album.userId.Should().BePositive();
            album.title.Should().NotBeNullOrEmpty();
        }

        //get test (invalid id)
        [Fact]
        public async Task Get_Album_Invalid_Id_Should_Return_404()
        {
            using var response = await _client.GetWithResponseAsync<Album>("albums/999999");

            response.IsSuccess.Should().BeFalse();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
            response.Body.Should().BeNull();
        }

        //post test (create album)
        [Fact]
        public async Task Create_Album_Should_Return_Created_Album()
        {
            var newAlbum = new Album
            {
                userId = 1,
                title = "Test Album"
            };

            var created = await _client.PostAsync<Album>("albums", newAlbum);

            created.Should().NotBeNull();
            created!.id.Should().BePositive();
            created.title.Should().Be(newAlbum.title);
        }

        //put test (update album)
        [Fact]
        public async Task Update_Album_Should_Return_Updated_Album()
        {
            var updatedAlbum = new Album
            {
                userId = 1,
                id = 1,
                title = "Updated Title"
            };

            var result = await _client.PutAsync<Album>("albums/1", updatedAlbum);

            result.Should().NotBeNull();
            result!.id.Should().Be(1);
            result.title.Should().Be("Updated Title");
        }

        // put test (update album with wrong fields)
        //JSONPlaceholder always accepts and echoes back any JSON so url is ignored, but we can still test the API's behavior
        // to ensure it doesn't throw an error when unexpected fields are present.
        [Fact]
        public async Task Update_Album_With_Wrong_Fields_Should_Ignore_Invalid_Properties()
        {
            var invalidAlbumPayload = new
            {
                userId = 1,
                id = 1,
                url = "https://not-a-valid-field.com"
            };

            using var response = await _client.PutWithResponseAsync<Album>("albums/1", invalidAlbumPayload);

            response.Should().NotBeNull();

            // HTTP-level checks
            response.IsSuccess.Should().BeTrue();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            // Body-level checks
            response.Body.Should().NotBeNull();
            response.Body!.id.Should().Be(1);
            response.Body.userId.Should().Be(1);

            // Title was omitted → null or empty
            response.Body.title.Should().BeNullOrEmpty();

            // If we want to prove JSONPlaceholder echoed the 'url', inspect Content (raw JSON)
            response.Content.Should().Contain("https://not-a-valid-field.com");
        }




        //delete test (delete album)
        [Fact]
        public async Task Delete_Album_Should_Return_True()
        {
            var success = await _client.DeleteAsync("albums/1");

            success.Should().BeTrue();
        }
    }
}
