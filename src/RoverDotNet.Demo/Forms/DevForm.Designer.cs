namespace RoverDotNet.Demo.Forms
{
    partial class DevForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            titleLabel = new Label();
            subgraphsLabel = new Label();
            subgraphsTextBox = new TextBox();
            routerPortLabel = new Label();
            routerPortTextBox = new TextBox();
            startButton = new Button();
            stopButton = new Button();
            outputLabel = new Label();
            outputTextBox = new TextBox();
            statusLabel = new Label();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            titleLabel.Location = new Point(12, 9);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(174, 25);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Rover Dev Session";
            // 
            // subgraphsLabel
            // 
            subgraphsLabel.AutoSize = true;
            subgraphsLabel.Location = new Point(12, 50);
            subgraphsLabel.Name = "subgraphsLabel";
            subgraphsLabel.Size = new Size(239, 15);
            subgraphsLabel.TabIndex = 1;
            subgraphsLabel.Text = "Subgraphs (format: name|url|schema_path):";
            // 
            // subgraphsTextBox
            // 
            subgraphsTextBox.Font = new Font("Consolas", 9F);
            subgraphsTextBox.Location = new Point(12, 68);
            subgraphsTextBox.Multiline = true;
            subgraphsTextBox.Name = "subgraphsTextBox";
            subgraphsTextBox.ScrollBars = ScrollBars.Vertical;
            subgraphsTextBox.Size = new Size(760, 120);
            subgraphsTextBox.TabIndex = 2;
            // 
            // routerPortLabel
            // 
            routerPortLabel.AutoSize = true;
            routerPortLabel.Location = new Point(12, 200);
            routerPortLabel.Name = "routerPortLabel";
            routerPortLabel.Size = new Size(70, 15);
            routerPortLabel.TabIndex = 3;
            routerPortLabel.Text = "Router Port:";
            // 
            // routerPortTextBox
            // 
            routerPortTextBox.Location = new Point(90, 197);
            routerPortTextBox.Name = "routerPortTextBox";
            routerPortTextBox.Size = new Size(100, 23);
            routerPortTextBox.TabIndex = 4;
            // 
            // startButton
            // 
            startButton.BackColor = Color.FromArgb(0, 120, 215);
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.ForeColor = Color.White;
            startButton.Location = new Point(12, 236);
            startButton.Name = "startButton";
            startButton.Size = new Size(120, 35);
            startButton.TabIndex = 5;
            startButton.Text = "Start Session";
            startButton.UseVisualStyleBackColor = false;
            startButton.Click += startButton_Click;
            // 
            // stopButton
            // 
            stopButton.BackColor = Color.FromArgb(232, 17, 35);
            stopButton.Enabled = false;
            stopButton.FlatStyle = FlatStyle.Flat;
            stopButton.ForeColor = Color.White;
            stopButton.Location = new Point(138, 236);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(120, 35);
            stopButton.TabIndex = 6;
            stopButton.Text = "Stop Session";
            stopButton.UseVisualStyleBackColor = false;
            stopButton.Click += stopButton_Click;
            // 
            // outputLabel
            // 
            outputLabel.AutoSize = true;
            outputLabel.Location = new Point(12, 284);
            outputLabel.Name = "outputLabel";
            outputLabel.Size = new Size(48, 15);
            outputLabel.TabIndex = 7;
            outputLabel.Text = "Output:";
            // 
            // outputTextBox
            // 
            outputTextBox.BackColor = Color.Black;
            outputTextBox.Font = new Font("Consolas", 9F);
            outputTextBox.ForeColor = Color.LimeGreen;
            outputTextBox.Location = new Point(12, 302);
            outputTextBox.Multiline = true;
            outputTextBox.Name = "outputTextBox";
            outputTextBox.ReadOnly = true;
            outputTextBox.ScrollBars = ScrollBars.Both;
            outputTextBox.Size = new Size(760, 200);
            outputTextBox.TabIndex = 8;
            outputTextBox.WordWrap = false;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            statusLabel.Location = new Point(264, 245);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(69, 15);
            statusLabel.TabIndex = 9;
            statusLabel.Text = "Status: Idle";
            // 
            // DevForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 514);
            Controls.Add(statusLabel);
            Controls.Add(outputTextBox);
            Controls.Add(outputLabel);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(routerPortTextBox);
            Controls.Add(routerPortLabel);
            Controls.Add(subgraphsTextBox);
            Controls.Add(subgraphsLabel);
            Controls.Add(titleLabel);
            Name = "DevForm";
            Text = "Rover Dev Session";
            FormClosing += DevForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label titleLabel;
        private Label subgraphsLabel;
        private TextBox subgraphsTextBox;
        private Label routerPortLabel;
        private TextBox routerPortTextBox;
        private Button startButton;
        private Button stopButton;
        private Label outputLabel;
        private TextBox outputTextBox;
        private Label statusLabel;
    }
}
