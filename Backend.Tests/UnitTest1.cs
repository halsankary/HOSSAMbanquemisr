using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Backend;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Backend.Tests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetMessages()
        {
            var client = _factory.CreateClient();

            var testMessage = new Message("Test Name", "Test Body");
            var postResponse = await client.PostAsJsonAsync("/messages", testMessage);

            var response = await client.GetAsync("/messages");

            response.EnsureSuccessStatusCode();
            var messages = await response.Content.ReadFromJsonAsync<List<Message>>();
            Assert.True(messages.Any());
        }

        [Fact]
        public async Task PostMessage_AddsMessageToList()
        {
            var client = _factory.CreateClient();

            var testMessage = new Message("Test Name", "Test Body");
            var postResponse = await client.PostAsJsonAsync("/messages", testMessage);

            postResponse.EnsureSuccessStatusCode();
            var createdMessage = await postResponse.Content.ReadFromJsonAsync<Message>();
            Assert.Equal(testMessage, createdMessage);

            var getResponse = await client.GetAsync("/messages");

            getResponse.EnsureSuccessStatusCode();
            var messages = await getResponse.Content.ReadFromJsonAsync<List<Message>>();
            Assert.Single(messages);
            Assert.Equal(testMessage, messages[0]);
        }

      //  [Fact]
        public async Task PutMessage_UpdatesMessage()
        {
            var client = _factory.CreateClient();

            var testMessage = new Message("Test Name", "Test Body");
            var postResponse = await client.PostAsJsonAsync("/messages", testMessage);

            postResponse.EnsureSuccessStatusCode();
            var createdMessage = await postResponse.Content.ReadFromJsonAsync<Message>();
            Assert.Equal(testMessage, createdMessage);

            var updatedMessage = new Message("Updated Name", "Updated Body");
            var putResponse = await client.PutAsJsonAsync("/messages/0", updatedMessage);

            putResponse.EnsureSuccessStatusCode();
            var returnedMessage = await putResponse.Content.ReadFromJsonAsync<Message>();
            Assert.Equal(updatedMessage, returnedMessage);

            var getResponse = await client.GetAsync("/messages");

            getResponse.EnsureSuccessStatusCode();
            var messages = await getResponse.Content.ReadFromJsonAsync<List<Message>>();
            Assert.Single(messages);
            Assert.Equal(updatedMessage, messages[0]);
        }

      //  [Fact]
        public async Task DeleteMessage_RemovesMessageFromList()
        {
            var client = _factory.CreateClient();

            var testMessage = new Message("Test Name", "Test Body");
            var postResponse = await client.PostAsJsonAsync("/messages", testMessage);

            postResponse.EnsureSuccessStatusCode();
            var createdMessage = await postResponse.Content.ReadFromJsonAsync<Message>();
            Assert.Equal(testMessage, createdMessage);

            var deleteResponse = await client.DeleteAsync("/messages/0");

            deleteResponse.EnsureSuccessStatusCode();

            var getResponse = await client.GetAsync("/messages");

            getResponse.EnsureSuccessStatusCode();
            var messages = await getResponse.Content.ReadFromJsonAsync<List<Message>>();
            Assert.Empty(messages);
        }
    }
}
