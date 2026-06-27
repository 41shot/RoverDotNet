using RoverDotNet.Dev;
using RoverDotNet.Dev.Exceptions;
using RoverDotNet.Dev.Models;

namespace RoverDotNet.Demo.Forms;

/// <summary>
/// Form for configuring and running a Dev session.
/// </summary>
public partial class DevForm : RoverOperationFormBase
{
    private DevSession? _session;
    private CancellationTokenSource? _cancellationTokenSource;

    public DevForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Set default values
        routerPortTextBox.Text = "4000";

        // Add sample subgraph
        AddSampleSubgraphs();
    }

    private void AddSampleSubgraphs()
    {
        // Add example subgraph entries
        var exampleText = "# Example subgraph configuration:\r\n"
            + "# users|http://localhost:4001|path/to/users.graphql\r\n"
            + "# products|http://localhost:4002|path/to/products.graphql\r\n";

        subgraphsTextBox.Text = exampleText;
    }

    private async void startButton_Click(object sender, EventArgs e)
    {
        try
        {
            var configuration = BuildConfiguration();
            if (configuration == null)
                return;

            // Disable controls
            startButton.Enabled = false;
            stopButton.Enabled = true;
            subgraphsTextBox.Enabled = false;
            routerPortTextBox.Enabled = false;

            _cancellationTokenSource = new CancellationTokenSource();

            // Create and start session
            _session = new DevSession(configuration);
            _session.StateChanged += OnSessionStateChanged;

            LogMessage("Starting dev session...");

            await _session.StartAsync(_cancellationTokenSource.Token);
        }
        catch (DevException ex)
        {
            LogMessage($"ERROR: {ex.Message}");
            if (ex is CompositionFailedException compositionEx)
            {
                foreach (var error in compositionEx.CompositionErrors)
                {
                    LogMessage($"  - {error}");
                }
            }
            ResetControls();
        }
        catch (Exception ex)
        {
            LogMessage($"UNEXPECTED ERROR: {ex.Message}");
            ResetControls();
        }
    }

    private async void stopButton_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("Stopping dev session...");

            _cancellationTokenSource?.Cancel();

            if (_session != null)
            {
                await _session.StopAsync();
                _session.Dispose();
                _session = null;
            }

            LogMessage("Dev session stopped.");
        }
        catch (Exception ex)
        {
            LogMessage($"ERROR stopping session: {ex.Message}");
        }
        finally
        {
            ResetControls();
        }
    }

    private void OnSessionStateChanged(object? sender, DevSessionEvent e)
    {
        // Ensure we're on the UI thread
        if (InvokeRequired)
        {
            Invoke(() => OnSessionStateChanged(sender, e));
            return;
        }

        var timestamp = e.Timestamp.ToString("HH:mm:ss");
        var message = $"[{timestamp}] [{e.State}] {e.Message}";

        LogMessage(message);

        if (e.Exception != null)
        {
            LogMessage($"  Exception: {e.Exception.Message}");
        }

        // Update status label
        statusLabel.Text = $"Status: {e.State}";
    }

    private void LogMessage(string message)
    {
        outputTextBox.AppendText(message + Environment.NewLine);
        outputTextBox.SelectionStart = outputTextBox.Text.Length;
        outputTextBox.ScrollToCaret();
    }

    private DevConfiguration? BuildConfiguration()
    {
        // Parse router port
        if (!int.TryParse(routerPortTextBox.Text, out var port) || port < 1 || port > 65535)
        {
            MessageBox.Show(
                "Please enter a valid port number (1-65535).",
                "Invalid Port",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return null;
        }

        // Parse subgraphs
        var subgraphs = new List<SubgraphDefinition>();
        var lines = subgraphsTextBox.Lines
            .Where(line => !string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith("#"))
            .ToArray();

        if (lines.Length == 0)
        {
            MessageBox.Show(
                "Please add at least one subgraph.\n\nFormat: name|url|schema_path",
                "No Subgraphs",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return null;
        }

        foreach (var line in lines)
        {
            var parts = line.Split('|');
            if (parts.Length != 3)
            {
                MessageBox.Show(
                    $"Invalid subgraph format: {line}\n\nExpected format: name|url|schema_path",
                    "Invalid Subgraph",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return null;
            }

            var name = parts[0].Trim();
            var url = parts[1].Trim();
            var schemaPath = parts[2].Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(schemaPath))
            {
                MessageBox.Show(
                    $"Invalid subgraph (empty fields): {line}",
                    "Invalid Subgraph",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return null;
            }

            if (!File.Exists(schemaPath))
            {
                MessageBox.Show(
                    $"Schema file not found: {schemaPath}\n\nSubgraph: {name}",
                    "File Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return null;
            }

            subgraphs.Add(new SubgraphDefinition(name, url, schemaPath));
        }

        return new DevConfiguration(subgraphs, RouterPort: port);
    }

    private void ResetControls()
    {
        startButton.Enabled = true;
        stopButton.Enabled = false;
        subgraphsTextBox.Enabled = true;
        routerPortTextBox.Enabled = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _session?.Dispose();
            _cancellationTokenSource?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
