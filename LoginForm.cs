using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class LoginForm : Form
{
    private TextBox txtRpcUrl;
    private TextBox txtRpcUser;
    private TextBox txtRpcPassword;
    private Button btnLogin;
    private Label lblStatus;

    public LoginForm()
    {
        this.Text = "Login - Bitcoin Messaging App";
        this.Size = new System.Drawing.Size(350, 300);

        txtRpcUrl = new TextBox { Top = 20, Left = 20, Width = 300, PlaceholderText = "RPC URL (e.g., http://127.0.0.1:8332)" };
        txtRpcUser = new TextBox { Top = 60, Left = 20, Width = 300, PlaceholderText = "Username" };
        txtRpcPassword = new TextBox { Top = 100, Left = 20, Width = 300, PlaceholderText = "Password", PasswordChar = '*' };
        btnLogin = new Button { Text = "Log In", Top = 140, Left = 20, Width = 300 };
        lblStatus = new Label { Top = 200, Left = 20, Width = 300, Height = 30 };

        btnLogin.Click += async (s, e) => await Login();

        Controls.Add(txtRpcUrl);
        Controls.Add(txtRpcUser);
        Controls.Add(txtRpcPassword);
        Controls.Add(btnLogin);
        Controls.Add(lblStatus);
    }

    private async Task Login()
    {
        string rpcUrl = txtRpcUrl.Text;
        string rpcUser = txtRpcUser.Text;
        string rpcPassword = txtRpcPassword.Text;

        lblStatus.Text = "Checking connection...";
        bool isConnected = await TestBitcoinCoreConnection(rpcUrl, rpcUser, rpcPassword);
        if (isConnected)
        {
            lblStatus.Text = "Successfully connected!";
            OpenMessageScreen(rpcUrl, rpcUser, rpcPassword);
        }
        else
        {
            lblStatus.Text = "Connection failed! Please verify your details.";
        }
    }

    private void OpenMessageScreen(string rpcUrl, string rpcUser, string rpcPassword)
    {
        MainForm mainForm = new MainForm(rpcUrl, rpcUser, rpcPassword);
        this.Hide();
        mainForm.Show();
    }

    private async Task<bool> TestBitcoinCoreConnection(string rpcUrl, string rpcUser, string rpcPassword)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{rpcUser}:{rpcPassword}")));

            try
            {
                var response = await client.GetAsync(rpcUrl);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}