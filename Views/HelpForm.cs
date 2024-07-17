// Interfaces/HelpForm.cs

namespace DicomModifier
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            InitializeWebView2();
        }

        private async void InitializeWebView2()
        {
            string helpFilePath = Path.Combine(Application.StartupPath, "help.html");
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.Navigate(helpFilePath);
        }
    }
}