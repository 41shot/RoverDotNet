using RoverDotNet.Core.Config;

namespace RoverDotNet.Demo.Forms;

/// <summary>
/// WinForms implementation of <see cref="IApiKeyPrompt"/> that displays a dialog
/// to prompt the user for an Apollo Studio API key.
/// </summary>
public sealed class WinFormsApiKeyPrompt : IApiKeyPrompt
{
    /// <summary>
    /// Displays a dialog prompting the user to enter an Apollo Studio API key.
    /// </summary>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <returns>The API key entered by the user.</returns>
    /// <exception cref="OperationCanceledException">The operation was cancelled.</exception>
    public Task<string> PromptAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var inputDialog = new ApiKeyInputDialog();
        var result = inputDialog.ShowDialog();

        if (result != DialogResult.OK || string.IsNullOrWhiteSpace(inputDialog.ApiKey))
        {
            throw new OperationCanceledException("API key input was cancelled or empty.");
        }

        return Task.FromResult(inputDialog.ApiKey);
    }

    /// <summary>
    /// Internal dialog form for API key input.
    /// </summary>
    private sealed class ApiKeyInputDialog : Form
    {
        private readonly TextBox _apiKeyTextBox;
        private readonly Button _okButton;
        private readonly Button _cancelButton;

        public string ApiKey => _apiKeyTextBox.Text.Trim();

        public ApiKeyInputDialog()
        {
            // Form properties
            Text = "Enter API Key";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(500, 220);
            AcceptButton = null; // Will be set to _okButton after validation

            // Instructions label
            var instructionsLabel = new Label
            {
                Text = "To authenticate, please paste an Apollo Studio API key.\n\n" +
                       "You can create a new personal API key at:\n" +
                       "https://studio.apollographql.com/user-settings/api-keys",
                Location = new Point(12, 12),
                Size = new Size(476, 80),
                AutoSize = false
            };

            // API Key label
            var apiKeyLabel = new Label
            {
                Text = "API Key:",
                Location = new Point(12, 102),
                Size = new Size(60, 15),
                AutoSize = true
            };

            // API Key text box
            _apiKeyTextBox = new TextBox
            {
                Location = new Point(12, 122),
                Size = new Size(476, 23),
                TabIndex = 0,
                UseSystemPasswordChar = true
            };
            _apiKeyTextBox.TextChanged += OnTextChanged;

            // OK button
            _okButton = new Button
            {
                Text = "OK",
                Location = new Point(332, 160),
                Size = new Size(75, 23),
                TabIndex = 1,
                DialogResult = DialogResult.OK,
                Enabled = false
            };

            // Cancel button
            _cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(413, 160),
                Size = new Size(75, 23),
                TabIndex = 2,
                DialogResult = DialogResult.Cancel
            };

            // Add controls
            Controls.Add(instructionsLabel);
            Controls.Add(apiKeyLabel);
            Controls.Add(_apiKeyTextBox);
            Controls.Add(_okButton);
            Controls.Add(_cancelButton);

            CancelButton = _cancelButton;
        }

        private void OnTextChanged(object? sender, EventArgs e)
        {
            var hasText = !string.IsNullOrWhiteSpace(_apiKeyTextBox.Text);
            _okButton.Enabled = hasText;
            AcceptButton = hasText ? _okButton : null;
        }
    }
}
