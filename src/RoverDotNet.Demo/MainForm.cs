using RoverDotNet.Demo.Forms;

namespace RoverDotNet.Demo
{
    public partial class MainForm : Form
    {
        private readonly ConfigWhoAmIForm _configWhoAmIForm;
        private readonly ConfigAuthForm _configAuthForm;

        public MainForm(ConfigWhoAmIForm configWhoAmIForm, ConfigAuthForm configAuthForm)
        {
            InitializeComponent();
            _configWhoAmIForm = configWhoAmIForm;
            _configAuthForm = configAuthForm;
        }

        private void configWhoAmIButton_Click(object sender, EventArgs e)
        {
            _configWhoAmIForm.ProfileName = string.IsNullOrWhiteSpace(profileTextBox.Text)
                ? null
                : profileTextBox.Text.Trim();

            _configWhoAmIForm.ShowDialog(this);
        }

        private void configAuthButton_Click(object sender, EventArgs e)
        {
            _configAuthForm.ProfileName = string.IsNullOrWhiteSpace(profileTextBox.Text)
                ? null
                : profileTextBox.Text.Trim();

            _configAuthForm.ShowDialog(this);
        }
    }
}
