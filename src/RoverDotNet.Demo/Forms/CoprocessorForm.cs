using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RoverDotNet.Core.Coprocessor;
using RoverDotNet.Coprocessor;

namespace RoverDotNet.Demo.Forms;

/// <summary>
/// Demo form that hosts an in-process <see cref="RoverDotNet.Coprocessor"/> instance and
/// displays the last request/response the router sent for each stage (and subgraph).
/// </summary>
public partial class CoprocessorForm : Form
{
    private static readonly JsonSerializerOptions DetailsJsonOptions = new()
    {
        WriteIndented = true,
    };

    private readonly ICoprocessorActivityLog _activityLog;
    private readonly Dictionary<ListViewItem, CoprocessorActivityEntry> _entriesByItem = new();

    private WebApplication? _app;

    public CoprocessorForm(ICoprocessorActivityLog activityLog)
    {
        InitializeComponent();

        _activityLog = activityLog;
        _activityLog.EntryRecorded += OnEntryRecorded;

        urlTextBox.Text = CoprocessorAppBuilder.DefaultUrl;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshActivityList();
    }

    private async void startButton_Click(object sender, EventArgs e)
    {
        if (_app is not null)
            return;

        try
        {
            var url = string.IsNullOrWhiteSpace(urlTextBox.Text)
                ? CoprocessorAppBuilder.DefaultUrl
                : urlTextBox.Text.Trim();

            _app = CoprocessorAppBuilder.Build(
                url: url,
                configureServices: services =>
                {
                    // Share this form's activity log so the demo can display live requests/responses.
                    services.AddSingleton(_activityLog);
                    services.AddCoprocessorMiddleware<RoverDotNet.Coprocessor.Middleware.AuthClaimsCoprocessorMiddleware>();
                });

            await _app.StartAsync();

            startButton.Enabled = false;
            stopButton.Enabled = true;
            urlTextBox.Enabled = false;
            statusLabel.Text = $"Listening on {url}.";
        }
        catch (Exception ex)
        {
            statusLabel.Text = $"Failed to start: {ex.Message}";
            _app = null;
        }
    }

    private async void stopButton_Click(object sender, EventArgs e)
    {
        await StopCoprocessorAsync();
    }

    private async void CoprocessorForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            // Keep the form (and the coprocessor host) alive so it can be reopened
            // from the main form without losing state.
            e.Cancel = true;
            Hide();
            return;
        }

        _activityLog.EntryRecorded -= OnEntryRecorded;
        await StopCoprocessorAsync();
    }

    private async Task StopCoprocessorAsync()
    {
        if (_app is null)
            return;

        try
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
        finally
        {
            _app = null;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            urlTextBox.Enabled = true;
            statusLabel.Text = "Stopped.";
        }
    }

    private void OnEntryRecorded(object? sender, CoprocessorActivityEntry entry)
    {
        if (InvokeRequired)
        {
            Invoke(() => OnEntryRecorded(sender, entry));
            return;
        }

        RefreshActivityList();
    }

    private void RefreshActivityList()
    {
        var selectedEntry = activityListView.SelectedItems.Count > 0
            ? _entriesByItem.GetValueOrDefault(activityListView.SelectedItems[0])
            : null;

        activityListView.BeginUpdate();
        activityListView.Items.Clear();
        _entriesByItem.Clear();

        ListViewItem? itemToReselect = null;

        foreach (var entry in _activityLog.GetLatestEntries())
        {
            var item = new ListViewItem(entry.Stage.ToString())
            {
                SubItems =
                {
                    entry.ServiceName ?? string.Empty,
                    entry.Timestamp.ToLocalTime().ToString("HH:mm:ss"),
                    entry.Response.Control.IsBreak ? $"break ({entry.Response.Control.BreakStatusCode})" : "continue",
                },
            };

            activityListView.Items.Add(item);
            _entriesByItem[item] = entry;

            if (selectedEntry is not null && entry.Stage == selectedEntry.Stage && entry.ServiceName == selectedEntry.ServiceName)
                itemToReselect = item;
        }

        activityListView.EndUpdate();

        if (itemToReselect is not null)
        {
            itemToReselect.Selected = true;
        }
        else
        {
            detailsTextBox.Text = string.Empty;
        }
    }

    private void activityListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (activityListView.SelectedItems.Count == 0)
        {
            detailsTextBox.Text = string.Empty;
            return;
        }

        var entry = _entriesByItem[activityListView.SelectedItems[0]];

        var requestJson = JsonSerializer.Serialize(entry.Request, DetailsJsonOptions);
        var responseJson = JsonSerializer.Serialize(entry.Response, DetailsJsonOptions);

        detailsTextBox.Text =
            $"--- Request from router ---{Environment.NewLine}{requestJson}{Environment.NewLine}{Environment.NewLine}" +
            $"--- Response to router ---{Environment.NewLine}{responseJson}";
    }
}
