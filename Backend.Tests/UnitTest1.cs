using System.Collections.Generic;  
using System.Linq;  
using System.Net;  
using System.Net.Http.Json;  
using System.Threading.Tasks;  
using Microsoft.AspNetCore.TestHost;  
using Microsoft.Extensions.Hosting;  
using Xunit;  
  
namespace Backend.Tests  
{  
    public class MessageApiTests  
    {  
        private IHost CreateHost()  
        {  
            return new HostBuilder()  
                .ConfigureWebHost(webHost =>  
                {  
                    webHost.UseTestServer();  
                    webHost.Configure((ctx, app) =>  
                    {  
                        var webApp = WebApplication.Create();  
                        Program.Configure(webApp);  
                        app.Use(next => webApp.Build().Invoke);  
                    });  
                })  
                .Build();  
        }  
  
        [Fact]  
        public async Task TestMessageApi_GetMessages_ReturnsMessages()  
        {  
            var host = CreateHost();  
            var client = host.GetTestClient();  
            var response = await client.GetAsync("/messages");  
  
            response.EnsureSuccessStatusCode();  
            var messages = await response.Content.ReadFromJsonAsync<List<Message>>();  
            Assert.NotNull(messages);  
        }  
  
        [Fact]  
        public async Task TestMessageApi_GetMessageById_ReturnsMessage()  
        {  
            var host = CreateHost();  
            var client = host.GetTestClient();  
            var message = new Message("Test", "Test body");  
            MessageStore.Messages.Add(message);  
            var response = await client.GetAsync($"/messages/{MessageStore.Messages.IndexOf(message)}");  
  
            response.EnsureSuccessStatusCode();  
            var retrievedMessage = await response.Content.ReadFromJsonAsync<Message>();  
            Assert.NotNull(retrievedMessage);  
            Assert.Equal(message, retrievedMessage);  
        }  
  
        [Fact]  
        public async Task TestMessageApi_CreateMessage_ReturnsCreatedMessage()  
        {  
            var host = CreateHost();  
            var client = host.GetTestClient();  
            var message = new Message("Test", "Test body");  
            var response = await client.PostAsJsonAsync("/messages", message);  
  
            response.EnsureSuccessStatusCode();  
            var createdMessage = await response.Content.ReadFromJsonAsync<Message>();  
            Assert.NotNull(createdMessage);  
            Assert.Equal(message, createdMessage);  
            Assert.Contains(createdMessage, MessageStore.Messages);  
        }  
  
        [Fact]  
        public async Task TestMessageApi_UpdateMessage_UpdatesAndReturnsMessage()  
        {  
            var host = CreateHost();  
            var client = host.GetTestClient();  
            var message = new Message("Test", "Test body");  
            MessageStore.Messages.Add(message);  
            var updatedMessage = new Message("Updated Test", "Updated Test body");  
            var response = await client.PutAsJsonAsync($"/messages/{MessageStore.Messages.IndexOf(message)}", updatedMessage);  
  
            response.EnsureSuccessStatusCode();  
            var retrievedMessage = await response.Content.ReadFromJsonAsync<Message>();  
            Assert.NotNull(retrievedMessage);  
            Assert.Equal(updatedMessage, retrievedMessage);  
            Assert.Equal(updatedMessage, MessageStore.Messages[MessageStore.Messages.IndexOf(message)]);  
        }  
  
        [Fact]  
        public async Task TestMessageApi_DeleteMessage_RemovesMessage()  
        {  
            var host = CreateHost();  
            var client = host.GetTestClient();  
            var message = new Message("Test", "Test body");  
            MessageStore.Messages.Add(message);  
            var response = await client.DeleteAsync($"/messages/{MessageStore.Messages.IndexOf(message)}");  
  
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);  
            Assert.DoesNotContain(message, MessageStore.Messages);  
        }  
    }  
}  
