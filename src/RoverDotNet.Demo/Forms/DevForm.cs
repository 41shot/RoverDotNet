using RoverDotNet.Core.Config;
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
    private string? _tempSupergraphConfigPath;

    public DevForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Set default values
        routerPortTextBox.Text = "4000";

        // Load default supergraph config
        LoadDefaultSupergraphConfig();
    }

    private void LoadDefaultSupergraphConfig()
    {
        // Try to load Supergraph.yaml from the application directory
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var supergraphConfigPath = Path.Combine(baseDirectory, "Supergraph.yaml");

        subgraphsTextBox.Text = @"# Apollo Supergraph Configuration" + Environment.NewLine;
        subgraphsTextBox.AppendText(
            "# This file defines the subgraphs that make up your federated GraphQL API." + Environment.NewLine);
        subgraphsTextBox.AppendText(
            "# For more information, see: https://www.apollographql.com/docs/rover/commands/supergraphs/" + Environment.NewLine);
        subgraphsTextBox.AppendText(Environment.NewLine);

        if (File.Exists(supergraphConfigPath))
        {
            try
            {
                var content = File.ReadAllText(supergraphConfigPath);
                subgraphsTextBox.AppendText(content);
                LogMessage($"Loaded default supergraph config from: {supergraphConfigPath}");
            }
            catch (Exception ex)
            {
                LogMessage($"Warning: Could not load Supergraph.yaml: {ex.Message}");
            }
        }
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
            routerPortTextBox.Enabled = false;
            acceptElv2CheckBox.Enabled = false;
            enableCoprocessorCheckBox.Enabled = false;
            apolloKeyTextBox.Enabled = false;
            apolloGraphRefTextBox.Enabled = false;
            saveConfigButton.Enabled = true;

            _cancellationTokenSource = new CancellationTokenSource();

            // Create and start session
            _session = new DevSession(configuration);
            _session.StateChanged += OnSessionStateChanged;
            _session.LicenceAcceptanceRequired += OnLicenceAcceptanceRequired;

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

    private async void saveConfigButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_tempSupergraphConfigPath))
            {
                LogMessage("No active session. Save button is only available during a session.");
                return;
            }

            if (!File.Exists(_tempSupergraphConfigPath))
            {
                LogMessage($"Error: Temp config file not found: {_tempSupergraphConfigPath}");
                return;
            }

            // Write the current text box content to the temp file
            var content = subgraphsTextBox.Text;
            await File.WriteAllTextAsync(_tempSupergraphConfigPath, content);

            LogMessage("Supergraph configuration updated. The session will detect changes and recompose...");
        }
        catch (Exception ex)
        {
            LogMessage($"ERROR saving config: {ex.Message}");
        }
    }

    private async void stopButton_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("Stopping dev session...");

            await StopDevSession();

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

    private async void DevForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            // Keep the form (and any running dev session) alive so it can be reopened
            // from the main form without losing state.
            e.Cancel = true;
            Hide();
            return;
        }

        await StopDevSession();
    }

    private async Task StopDevSession()
    {
        _cancellationTokenSource?.Cancel();

        if (_session != null)
        {
            await _session.StopAsync();
            _session.Dispose();
            _session = null;
        }

        // Clean up temp config file
        if (_tempSupergraphConfigPath != null && File.Exists(_tempSupergraphConfigPath))
        {
            try
            {
                File.Delete(_tempSupergraphConfigPath);
            }
            catch
            {
                // Ignore cleanup errors
            }
            _tempSupergraphConfigPath = null;
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

    private Task<bool> OnLicenceAcceptanceRequired()
    {
        // Ensure we're on the UI thread
        if (InvokeRequired)
        {
            return Task.FromResult((bool)Invoke(() => OnLicenceAcceptanceRequired().Result));
        }

        var result = MessageBox.Show(
            "The Apollo Router requires the Elastic License v2.0 (ELv2).\n\n" +
            "By installing this plugin, you accept the terms and conditions outlined by this licence.\n\n" +
            "More information on the ELv2 licence can be found here:\n" +
            "https://go.apollo.dev/elv2\n\n" +
            "Do you accept the terms and conditions of the ELv2 licence?",
            "Licence Acceptance Required",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2); // Default to No for safety

        var accepted = result == DialogResult.Yes;

        if (accepted)
        {
            LogMessage("ELv2 licence accepted by user.");
        }
        else
        {
            LogMessage("ELv2 licence declined by user.");
        }

        return Task.FromResult(accepted);
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

        // Validate YAML content
        var yamlContent = subgraphsTextBox.Text;
        if (string.IsNullOrWhiteSpace(yamlContent))
        {
            MessageBox.Show(
                "Please provide an Apollo supergraph configuration in YAML format.",
                "No Configuration",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return null;
        }

        // Write content to a temporary file and track it
        _tempSupergraphConfigPath = Path.Combine(Path.GetTempPath(), $"supergraph-config-{Guid.NewGuid()}.yaml");

        try
        {
            File.WriteAllText(_tempSupergraphConfigPath, yamlContent);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to write temporary config file: {ex.Message}",
                "File Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return null;
        }

        // Configure ELv2 licence acceptance
        var elv2Licence = acceptElv2CheckBox.Checked ? "accept" : null;

        var coprocessorEnabled = enableCoprocessorCheckBox.Checked;
        var routerConfigPath = coprocessorEnabled ? "Router-with-coprocessor.yaml" : "Router.yaml";

        if (coprocessorEnabled)
        {
            var apolloKey = apolloKeyTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(apolloKey))
            {
                if (!apolloKey.StartsWith("service:", StringComparison.Ordinal))
                {
                    var result = MessageBox.Show(
                        "The Apollo Key should be a graph (service) token, starting with \"service:\", not a personal \"user:\" token. " +
                        "Graph tokens are found in GraphOS Studio and must be licenced for coprocessor use. Continue anyway?",
                        "Check Apollo Key",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.OK)
                    {
                        return null;
                    }
                }

                EnvironmentVariableHelper.SetProcessValue("APOLLO_KEY", apolloKey);
            }

            var apolloGraphRef = apolloGraphRefTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(apolloGraphRef))
            {
                EnvironmentVariableHelper.SetProcessValue("APOLLO_GRAPH_REF", apolloGraphRef);
            }
        }

        return new DevConfiguration(
            SupergraphConfigPath: _tempSupergraphConfigPath,
            RouterPort: port,
            RouterConfigPath: routerConfigPath,
            Elv2Licence: elv2Licence);
    }

    private void enableCoprocessorCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        var enabled = enableCoprocessorCheckBox.Checked;
        apolloKeyTextBox.Enabled = enabled;
        apolloGraphRefTextBox.Enabled = enabled;
    }

    private void ResetControls()
    {
        startButton.Enabled = true;
        stopButton.Enabled = false;
        subgraphsTextBox.Enabled = true;
        routerPortTextBox.Enabled = true;
        acceptElv2CheckBox.Enabled = true;
        enableCoprocessorCheckBox.Enabled = true;
        apolloKeyTextBox.Enabled = enableCoprocessorCheckBox.Checked;
        apolloGraphRefTextBox.Enabled = enableCoprocessorCheckBox.Checked;
        saveConfigButton.Enabled = false;
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
