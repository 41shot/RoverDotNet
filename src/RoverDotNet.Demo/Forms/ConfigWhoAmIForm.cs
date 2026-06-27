using RoverDotNet.Client.Operations.WhoAmI;
using RoverDotNet.Config.WhoAmI;
using RoverDotNet.Core.Config;
using RoverDotNet.Core.Exceptions;

namespace RoverDotNet.Demo.Forms;

/// <summary>
/// Form for executing and displaying the result of the Config WhoAmI operation.
/// </summary>
public partial class ConfigWhoAmIForm : RoverOperationFormBase
{
    public ConfigWhoAmIForm()
    {
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
            await ExecuteWhoAmIAsync();
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

    private async Task ExecuteWhoAmIAsync()
    {
        try
        {
            // Create the required dependencies
            var profileConfig = new ProfileConfig();
            using var httpClient = new HttpClient();
            var studioClient = new Client.Http.StudioHttpClient(
                httpClient,
                clientVersion: "demo-1.0.0");
            var operation = new WhoAmIOperation(studioClient);
            var configWhoAmI = new ConfigWhoAmI(profileConfig, operation);

            // Determine the profile name to use
            var profileName = string.IsNullOrWhiteSpace(ProfileName)
                ? ProfileConfig.DefaultProfile
                : ProfileName;

            // Execute the operation
            var result = await configWhoAmI.ExecuteAsync(
                profileName: profileName,
                unmaskKey: false,
                cancellationToken: default);

            DisplayResult(result);
        }
        catch (NoProfilesException ex)
        {
            MessageBox.Show(
                ex.Message + "\n\nPlease run 'config auth' first to set up a profile.",
                "No Profiles Found",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            Close();
        }
        catch (ProfileNotFoundException ex)
        {
            MessageBox.Show(
                ex.Message + $"\n\nAvailable profiles can be found in your config file.",
                "Profile Not Found",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            Close();
        }
        catch (InvalidKeyException ex)
        {
            MessageBox.Show(
                ex.Message + "\n\nPlease verify your API key is valid.",
                "Invalid API Key",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            Close();
        }
    }

    private void DisplayResult(WhoAmIResult result)
    {
        var text = $"API Key: {result.ApiKey}\n" +
                   $"Actor Type: {result.ActorType}\n" +
                   $"Origin: {result.Origin}\n";

        if (result.ActorType == Actor.User && result.UserId != null)
        {
            text += $"User ID: {result.UserId}\n";
        }
        else if (result.ActorType == Actor.Graph)
        {
            text += $"Graph ID: {result.GraphId}\n";
            if (!string.IsNullOrEmpty(result.GraphTitle))
            {
                text += $"Graph Title: {result.GraphTitle}\n";
            }
        }

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
