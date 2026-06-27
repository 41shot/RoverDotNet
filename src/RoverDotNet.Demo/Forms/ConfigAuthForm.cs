using RoverDotNet.Config.Auth;
using RoverDotNet.Core.Config;
using RoverDotNet.Core.Exceptions;

namespace RoverDotNet.Demo.Forms;

/// <summary>
/// Form for executing and displaying the result of the Config Auth operation.
/// </summary>
public partial class ConfigAuthForm : RoverOperationFormBase
{
    private readonly ConfigAuth _configAuth;

    public ConfigAuthForm(ConfigAuth configAuth)
    {
        _configAuth = configAuth;
        InitializeComponent();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Disable the form while the operation is running
        Enabled = false;
        Cursor = Cursors.WaitCursor;

        try
        {
            await ExecuteConfigAuthAsync();
        }
        catch (Exception ex)
        {
            DisplayError(ex);
        }
        finally
        {
            Enabled = true;
            Cursor = Cursors.Default;
        }
    }

    private async Task ExecuteConfigAuthAsync()
    {
        try
        {
            // Execute the operation using the injected ConfigAuth
            var result = await _configAuth.ExecuteAsync(
                profileName: ProfileName ?? ProfileConfig.DefaultProfile,
                cancellationToken: default);

            DisplayResult(result);
        }
        catch (OperationCanceledException)
        {
            // User cancelled the API key input
            MessageBox.Show(
                "API key input was cancelled.",
                "Cancelled",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            Close();
        }
        catch (RoverException ex)
        {
            MessageBox.Show(
                ex.Message + "\n\nPlease try again with a valid API key.",
                "Authentication Failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            Close();
        }
    }

    private void DisplayResult(AuthResult result)
    {
        var text = $"Profile: {result.ProfileName}\n\n" +
                   $"{result.Message}";

        resultTextBox.Text = text;
    }

    private void DisplayError(Exception ex)
    {
        MessageBox.Show(
            $"An unexpected error occurred:\n\n{ex.Message}",
            "Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
        Close();
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
        Close();
    }
}
