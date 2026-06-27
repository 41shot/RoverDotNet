using RoverDotNet.Demo.Forms;

namespace RoverDotNet.Demo
{
    public partial class MainForm : Form
    {
        private readonly ConfigWhoAmIForm _configWhoAmIForm;

        public MainForm(ConfigWhoAmIForm configWhoAmIForm)
        {
            InitializeComponent();
            _configWhoAmIForm = configWhoAmIForm;
        }

        private void configWhoAmIButton_Click(object sender, EventArgs e)
        {
            _configWhoAmIForm.ProfileName = string.IsNullOrWhiteSpace(profileTextBox.Text)
                ? null
                : profileTextBox.Text.Trim();

            _configWhoAmIForm.ShowDialog(this);
        }
    }
}
