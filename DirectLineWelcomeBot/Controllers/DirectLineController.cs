using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DirectLineWelcomeBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectLineController : ControllerBase
    {
        string _directLineUrl = "";
        string _directLineSecret = "";


        public DirectLineController(IConfiguration configuration)
        {
            _directLineSecret = configuration["DirectLineSecret"];
            _directLineUrl = configuration["DirectLineUrl"];
        }
        // POST api/<controller>
        [HttpPost]
        public async Task<ChatConfig> Token(string value = null)
        {
            string token = String.Empty;
            ChatConfig chatConfig = null;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_directLineUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "v3/directline/tokens/generate");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _directLineSecret);
            var userId = $"dl_{Guid.NewGuid()}";
            request.Content = new StringContent(
            JsonConvert.SerializeObject(
                new { User = new { Id = userId } }),
                Encoding.UTF8,
                "application/json");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<DirectLineToken>(body).token;
                chatConfig = new ChatConfig()
                {
                    Token = token,
                    UserId = userId
                };
            }


            return chatConfig;
        }


    }
}
public class DirectLineToken
{
    public string conversationId { get; set; }
    public string token { get; set; }
    public int expires_in { get; set; }
}
public class ChatConfig
{
    public string Token { get; set; }
    public string UserId { get; set; }
}
