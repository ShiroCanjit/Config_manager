using System;
using System.Windows.Forms;
using CefSharp;

namespace VKR_Zhitenev2
{
    public partial class WebBrowser : Form
    {
        
        public WebBrowser()
        {
            
            InitializeComponent();
        }
        public string url_str;

        public void WebBrowser_Load(object sender, EventArgs e)
        {
            chromiumWebBrowser1.LoadUrl(url_str);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            chromiumWebBrowser1.Back();
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            chromiumWebBrowser1.Forward();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }
        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                chromiumWebBrowser1.Load(url);
            }
        }

        private void urlTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }        
    }
}
