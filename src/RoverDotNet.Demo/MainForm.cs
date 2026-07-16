using RoverDotNet.Demo.Forms;

namespace RoverDotNet.Demo
{
    public partial class MainForm : Form
    {
        private readonly ConfigWhoAmIForm _configWhoAmIForm;
        private readonly ConfigAuthForm _configAuthForm;
        private readonly DevForm _devForm;
        private readonly CoprocessorForm _coprocessorForm;

        public MainForm(ConfigWhoAmIForm configWhoAmIForm, ConfigAuthForm configAuthForm, DevForm devForm, CoprocessorForm coprocessorForm)
        {
            InitializeComponent();
            _configWhoAmIForm = configWhoAmIForm;
            _configAuthForm = configAuthForm;
            _devForm = devForm;
            _coprocessorForm = coprocessorForm;
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
            ShowNonModal(_devForm);
        }

        private void coprocessorButton_Click(object sender, EventArgs e)
        {
            ShowNonModal(_coprocessorForm);
        }

        private void ShowNonModal(Form form)
        {
            if (!form.Visible)
            {
                form.Show(this);
            }

            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
            }

            form.Activate();
        }
    }
}
