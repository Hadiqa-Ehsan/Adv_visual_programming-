using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace AIChatbotApp
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private const string apiKey = "AIzaSyDW7MfNZLm75_m-YKQadp84gLAso4T9QbU";

        public Form1()
        {
            InitializeComponent();
            listBox1.Items.Add("Bot: Hello! I am your AI Assistant. Ask me anything!");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string userMessage = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(userMessage))
            {
                label2.Text = "Please type a message";
                return;
            }

            listBox1.Items.Add($"You: {userMessage}");
            textBox1.Clear();
            label2.Text = "Bot is thinking...";

            string reply = await GetGeminiResponse(userMessage);

            listBox1.Items.Add($"Bot: {reply}");
            label2.Text = "Ready";
        }

        private async Task<string> GetGeminiResponse(string userInput)
        {
            try
            {
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = userInput }
                            }
                        }
                    }
                };

                string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                var json = JObject.Parse(responseBody);

                if (json["error"] != null)
                {
                    string errorMsg = json["error"]["message"].ToString();
                    return $"Error: {errorMsg}";
                }

                if (json["candidates"] != null && json["candidates"].HasValues)
                {
                    string reply = json["candidates"][0]["content"]["parts"][0]["text"].ToString();
                    return reply;
                }
                else
                {
                    return "Sorry, I couldn't process that. Please try again.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}