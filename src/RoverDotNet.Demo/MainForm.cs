using RoverDotNet.Demo.Forms;

namespace RoverDotNet.Demo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void configWhoAmIButton_Click(object sender, EventArgs e)
        {
            var form = new ConfigWhoAmIForm
            {
                ProfileName = string.IsNullOrWhiteSpace(profileTextBox.Text)
                    ? null
                    : profileTextBox.Text.Trim()
            };

            form.ShowDialog(this);
        }
    }
}
