using System;
using System.Reflection;
using System.Windows.Forms;
using DicomModifier.Services;

namespace DicomModifier.Views
{
    public partial class LoginForm : Form
    {
        private AuthenticationService authService;

        public LoginForm(AuthenticationService authService)
        {
            InitializeComponent();
            this.authService = authService;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version != null ? version.ToString() : "Versione non disponibile";
            labelVersion.Text = $"v. {versionString}";

            // Inizializza gli eventi dei pulsanti
            buttonLogin.Click += buttonLogin_Click;
            buttonQuit.Click += buttonQuit_Click;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;
            if (authService.Authenticate(username, password))
            {
                MessageBox.Show("Login successful!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Login failed. Check your username and password.");
            }
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
