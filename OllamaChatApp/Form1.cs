using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OllamaChatApp
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();

        private TextBox txtInput;
        private Button btnSend;
        private TextBox txtResponse;

        public Form1()
        {
            InitializeComponent();
            CriarInterface();
        }

        private void CriarInterface()
        {
            this.Text = "Chat com Ollama";
            this.Width = 600;
            this.Height = 400;

            txtInput = new TextBox() { Left = 10, Top = 10, Width = 450 };
            btnSend = new Button() { Left = 470, Top = 10, Width = 100, Text = "Enviar" };
            txtResponse = new TextBox()
            {
                Left = 10,
                Top = 50,
                Width = 560,
                Height = 300,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnSend.Click += async (sender, e) =>
            {
                string pergunta = txtInput.Text;
                if (string.IsNullOrWhiteSpace(pergunta)) return;

                txtResponse.AppendText("Você: " + pergunta + Environment.NewLine);
                string resposta = await EnviarParaOllama(pergunta);
                txtResponse.AppendText("Ollama: " + resposta + Environment.NewLine + Environment.NewLine);
                txtInput.Clear();
            };

            this.Controls.Add(txtInput);
            this.Controls.Add(btnSend);
            this.Controls.Add(txtResponse);
        }

        private async Task<string> EnviarParaOllama(string prompt)
        {
            var requestData = new
            {
                model = "llama3", // altere conforme o modelo disponível
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:11434/api/generate", content);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("response").GetString();
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }
    }
}
