namespace RoverDotNet.Demo.Forms
{
    partial class CoprocessorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            titleLabel = new Label();
            urlLabel = new Label();
            urlTextBox = new TextBox();
            startButton = new Button();
            stopButton = new Button();
            statusLabel = new Label();
            activityListView = new ListView();
            stageColumnHeader = new ColumnHeader();
            subgraphColumnHeader = new ColumnHeader();
            timeColumnHeader = new ColumnHeader();
            controlColumnHeader = new ColumnHeader();
            detailsLabel = new Label();
            detailsTextBox = new TextBox();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            titleLabel.Location = new Point(12, 9);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(155, 25);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Router Coprocessor";
            // 
            // urlLabel
            // 
            urlLabel.AutoSize = true;
            urlLabel.Location = new Point(12, 50);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new Size(63, 15);
            urlLabel.TabIndex = 1;
            urlLabel.Text = "Listen URL:";
            // 
            // urlTextBox
            // 
            urlTextBox.Location = new Point(81, 47);
            urlTextBox.Name = "urlTextBox";
            urlTextBox.Size = new Size(220, 23);
            urlTextBox.TabIndex = 2;
            // 
            // startButton
            // 
            startButton.BackColor = Color.FromArgb(0, 120, 215);
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.ForeColor = Color.White;
            startButton.Location = new Point(311, 46);
            startButton.Name = "startButton";
            startButton.Size = new Size(110, 25);
            startButton.TabIndex = 3;
            startButton.Text = "Start";
            startButton.UseVisualStyleBackColor = false;
            startButton.Click += startButton_Click;
            // 
            // stopButton
            // 
            stopButton.BackColor = Color.FromArgb(232, 17, 35);
            stopButton.Enabled = false;
            stopButton.FlatStyle = FlatStyle.Flat;
            stopButton.ForeColor = Color.White;
            stopButton.Location = new Point(427, 46);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(110, 25);
            stopButton.TabIndex = 4;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = false;
            stopButton.Click += stopButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.ForeColor = Color.DimGray;
            statusLabel.Location = new Point(12, 79);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(52, 15);
            statusLabel.TabIndex = 5;
            statusLabel.Text = "Stopped.";
            // 
            // activityListView
            // 
            activityListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            activityListView.Columns.AddRange(new ColumnHeader[] { stageColumnHeader, subgraphColumnHeader, timeColumnHeader, controlColumnHeader });
            activityListView.FullRowSelect = true;
            activityListView.GridLines = true;
            activityListView.HideSelection = false;
            activityListView.Location = new Point(12, 104);
            activityListView.MultiSelect = false;
            activityListView.Name = "activityListView";
            activityListView.Size = new Size(760, 150);
            activityListView.TabIndex = 6;
            activityListView.UseCompatibleStateImageBehavior = false;
            activityListView.View = View.Details;
            activityListView.SelectedIndexChanged += activityListView_SelectedIndexChanged;
            // 
            // stageColumnHeader
            // 
            stageColumnHeader.Text = "Stage";
            stageColumnHeader.Width = 140;
            // 
            // subgraphColumnHeader
            // 
            subgraphColumnHeader.Text = "Subgraph";
            subgraphColumnHeader.Width = 120;
            // 
            // timeColumnHeader
            // 
            timeColumnHeader.Text = "Time";
            timeColumnHeader.Width = 100;
            // 
            // controlColumnHeader
            // 
            controlColumnHeader.Text = "Control";
            controlColumnHeader.Width = 120;
            // 
            // detailsLabel
            // 
            detailsLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            detailsLabel.AutoSize = true;
            detailsLabel.Location = new Point(12, 262);
            detailsLabel.Name = "detailsLabel";
            detailsLabel.Size = new Size(133, 15);
            detailsLabel.TabIndex = 7;
            detailsLabel.Text = "Request / Response:";
            // 
            // detailsTextBox
            // 
            detailsTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            detailsTextBox.Font = new Font("Consolas", 9F);
            detailsTextBox.Location = new Point(12, 280);
            detailsTextBox.Multiline = true;
            detailsTextBox.Name = "detailsTextBox";
            detailsTextBox.ReadOnly = true;
            detailsTextBox.ScrollBars = ScrollBars.Vertical;
            detailsTextBox.Size = new Size(760, 200);
            detailsTextBox.TabIndex = 8;
            // 
            // CoprocessorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 495);
            Controls.Add(detailsTextBox);
            Controls.Add(detailsLabel);
            Controls.Add(activityListView);
            Controls.Add(statusLabel);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(urlTextBox);
            Controls.Add(urlLabel);
            Controls.Add(titleLabel);
            MinimumSize = new Size(600, 400);
            Name = "CoprocessorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Router Coprocessor";
            FormClosing += CoprocessorForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label titleLabel;
        private Label urlLabel;
        private TextBox urlTextBox;
        private Button startButton;
        private Button stopButton;
        private Label statusLabel;
        private ListView activityListView;
        private ColumnHeader stageColumnHeader;
        private ColumnHeader subgraphColumnHeader;
        private ColumnHeader timeColumnHeader;
        private ColumnHeader controlColumnHeader;
        private Label detailsLabel;
        private TextBox detailsTextBox;
    }
}
