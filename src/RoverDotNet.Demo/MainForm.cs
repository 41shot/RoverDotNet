using RoverDotNet.Demo.Forms;

namespace RoverDotNet.Demo
{
    public partial class MainForm : Form
    {
        private readonly ConfigWhoAmIForm _configWhoAmIForm;
        private readonly ConfigAuthForm _configAuthForm;
        private readonly DevForm _devForm;

        public MainForm(ConfigWhoAmIForm configWhoAmIForm, ConfigAuthForm configAuthForm, DevForm devForm)
        {
            InitializeComponent();
            _configWhoAmIForm = configWhoAmIForm;
            _configAuthForm = configAuthForm;
            _devForm = devForm;
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

        private void devButton_Click(object sender, EventArgs e)
        {
            _devForm.ShowDialog(this);
        }
    }
}
