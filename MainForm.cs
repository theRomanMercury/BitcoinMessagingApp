using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

public class MainForm : Form
{
    private string rpcUrl;
    private string rpcUser;
    private string rpcPassword;

    private TextBox txtTo;
    private TextBox txtMessage;
    private Button btnSend;
    private Label lblResult;
    private Label lblInbox;

    private System.Windows.Forms.Timer timer;

    public MainForm(string rpcUrl, string rpcUser, string rpcPassword)
    {
        this.rpcUrl = rpcUrl;
        this.rpcUser = rpcUser;
        this.rpcPassword = rpcPassword;

        this.Text = "Bitcoin Messaging App";
        this.Size = new System.Drawing.Size(400, 300);

        txtTo = new TextBox { Top = 20, Left = 20, Width = 350, PlaceholderText = "To: Recipient Address" };
        txtMessage = new TextBox { Top = 60, Left = 20, Width = 350, PlaceholderText = "Write your message here..." };
        btnSend = new Button { Text = "Send", Top = 100, Left = 20, Width = 350 };
        lblResult = new Label { Top = 140, Left = 20, Width = 350, Height = 30 };
        lblInbox = new Label { Top = 180, Left = 20, Width = 350, Height = 30 };

        btnSend.Click += async (s, e) => await SendMessage();

        Controls.Add(txtTo);
        Controls.Add(txtMessage);
        Controls.Add(btnSend);
        Controls.Add(lblResult);
        Controls.Add(lblInbox);

        timer = new System.Windows.Forms.Timer { Interval = 10000 };
        timer.Tick += async (s, e) => await CheckInbox();
        timer.Start();
    }

    private async Task SendMessage()
    {
        string toAddress = txtTo.Text;
        string message = txtMessage.Text;

        lblResult.Text = "Sending message to the blockchain...";
        try
        {
            string transactionId = await SendToBlockchain(toAddress, message);
            lblResult.Text = $"Message sent successfully! Transaction ID: {transactionId}";
        }
        catch (Exception ex)
        {
            lblResult.Text = $"Failed to send message: {ex.Message}";
        }
    }

    private async Task<string> SendToBlockchain(string toAddress, string message)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{rpcUser}:{rpcPassword}")));

            var rpcRequest = new
            {
                method = "createrawtransaction",
                @params = new object[]
                {
                    new object[] { },
                    new object[] { new { data = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{toAddress}:{message}")) } }
                }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, rpcUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(rpcRequest), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    private async Task CheckInbox()
    {
        lblInbox.Text = "Checking inbox for messages...";
        try
        {
            int inboxCount = await GetInboxMessageCount();
            lblInbox.Text = $"Inbox: {inboxCount} messages";
        }
        catch (Exception ex)
        {
            lblInbox.Text = $"Failed to check inbox: {ex.Message}";
        }
    }

    private async Task<int> GetInboxMessageCount()
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{rpcUser}:{rpcPassword}")));

            var rpcRequest = new { method = "listtransactions", @params = new object[] { "*" } };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, rpcUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(rpcRequest), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();

            dynamic? data = JsonConvert.DeserializeObject(responseData);
            if (data?.result == null)
            {
                throw new Exception("Received invalid or empty JSON data.");
            }

            int messageCount = 0;
            foreach (var tx in data.result)
            {
                if (tx.details?.scriptPubKey?.asm?.Contains("OP_RETURN") == true)
                {
                    messageCount++;
                }
            }
            return messageCount;
        }
    }
}