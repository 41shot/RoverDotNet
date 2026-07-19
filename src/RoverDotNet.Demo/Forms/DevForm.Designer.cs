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
            acceptElv2CheckBox = new CheckBox();
            enableCoprocessorCheckBox = new CheckBox();
            coprocessorInfoLabel = new Label();
            apolloKeyLabel = new Label();
            apolloKeyTextBox = new TextBox();
            apolloGraphRefLabel = new Label();
            apolloGraphRefTextBox = new TextBox();
            startButton = new Button();
            stopButton = new Button();
            saveConfigButton = new Button();
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
            subgraphsLabel.Size = new Size(190, 15);
            subgraphsLabel.TabIndex = 1;
            subgraphsLabel.Text = "Supergraph Configuration (YAML):";
            // 
            // subgraphsTextBox
            // 
            subgraphsTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            subgraphsTextBox.Font = new Font("Consolas", 9F);
            subgraphsTextBox.Location = new Point(12, 68);
            subgraphsTextBox.Multiline = true;
            subgraphsTextBox.Name = "subgraphsTextBox";
            subgraphsTextBox.ScrollBars = ScrollBars.Vertical;
            subgraphsTextBox.Size = new Size(760, 207);
            subgraphsTextBox.TabIndex = 2;
            // 
            // routerPortLabel
            // 
            routerPortLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            routerPortLabel.AutoSize = true;
            routerPortLabel.Location = new Point(12, 287);
            routerPortLabel.Name = "routerPortLabel";
            routerPortLabel.Size = new Size(70, 15);
            routerPortLabel.TabIndex = 3;
            routerPortLabel.Text = "Router Port:";
            // 
            // routerPortTextBox
            // 
            routerPortTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            routerPortTextBox.Location = new Point(90, 284);
            routerPortTextBox.Name = "routerPortTextBox";
            routerPortTextBox.Size = new Size(100, 23);
            routerPortTextBox.TabIndex = 4;
            // 
            // acceptElv2CheckBox
            // 
            acceptElv2CheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            acceptElv2CheckBox.AutoSize = true;
            acceptElv2CheckBox.Location = new Point(200, 286);
            acceptElv2CheckBox.Name = "acceptElv2CheckBox";
            acceptElv2CheckBox.Size = new Size(129, 19);
            acceptElv2CheckBox.TabIndex = 10;
            acceptElv2CheckBox.Text = "Accept ELv2 licence";
            acceptElv2CheckBox.UseVisualStyleBackColor = true;
            // 
            // enableCoprocessorCheckBox
            // 
            enableCoprocessorCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            enableCoprocessorCheckBox.AutoSize = true;
            enableCoprocessorCheckBox.Location = new Point(12, 315);
            enableCoprocessorCheckBox.Name = "enableCoprocessorCheckBox";
            enableCoprocessorCheckBox.Size = new Size(140, 19);
            enableCoprocessorCheckBox.TabIndex = 12;
            enableCoprocessorCheckBox.Text = "Enable coprocessor";
            enableCoprocessorCheckBox.UseVisualStyleBackColor = true;
            enableCoprocessorCheckBox.CheckedChanged += enableCoprocessorCheckBox_CheckedChanged;
            // 
            // coprocessorInfoLabel
            // 
            coprocessorInfoLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            coprocessorInfoLabel.AutoSize = false;
            coprocessorInfoLabel.ForeColor = Color.DimGray;
            coprocessorInfoLabel.Location = new Point(12, 338);
            coprocessorInfoLabel.Name = "coprocessorInfoLabel";
            coprocessorInfoLabel.Size = new Size(760, 45);
            coprocessorInfoLabel.TabIndex = 13;
            coprocessorInfoLabel.Text = "Requires a GraphOS graph token (service:...) licenced for coprocessor use. Provide an Apollo Key and Graph Ref below to set them for this session, or leave blank to use values already set as environment variables.";
            // 
            // apolloKeyLabel
            // 
            apolloKeyLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            apolloKeyLabel.AutoSize = true;
            apolloKeyLabel.Location = new Point(12, 388);
            apolloKeyLabel.Name = "apolloKeyLabel";
            apolloKeyLabel.Size = new Size(66, 15);
            apolloKeyLabel.TabIndex = 14;
            apolloKeyLabel.Text = "Apollo Key:";
            // 
            // apolloKeyTextBox
            // 
            apolloKeyTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            apolloKeyTextBox.Enabled = false;
            apolloKeyTextBox.Location = new Point(100, 385);
            apolloKeyTextBox.Name = "apolloKeyTextBox";
            apolloKeyTextBox.PasswordChar = '*';
            apolloKeyTextBox.Size = new Size(300, 23);
            apolloKeyTextBox.TabIndex = 15;
            // 
            // apolloGraphRefLabel
            // 
            apolloGraphRefLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            apolloGraphRefLabel.AutoSize = true;
            apolloGraphRefLabel.Location = new Point(420, 388);
            apolloGraphRefLabel.Name = "apolloGraphRefLabel";
            apolloGraphRefLabel.Size = new Size(70, 15);
            apolloGraphRefLabel.TabIndex = 16;
            apolloGraphRefLabel.Text = "Graph Ref:";
            // 
            // apolloGraphRefTextBox
            // 
            apolloGraphRefTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            apolloGraphRefTextBox.Enabled = false;
            apolloGraphRefTextBox.Location = new Point(500, 385);
            apolloGraphRefTextBox.Name = "apolloGraphRefTextBox";
            apolloGraphRefTextBox.Size = new Size(272, 23);
            apolloGraphRefTextBox.TabIndex = 17;
            // 
            // startButton
            // 
            startButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            startButton.BackColor = Color.FromArgb(0, 120, 215);
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.ForeColor = Color.White;
            startButton.Location = new Point(12, 445);
            startButton.Name = "startButton";
            startButton.Size = new Size(120, 35);
            startButton.TabIndex = 5;
            startButton.Text = "Start Session";
            startButton.UseVisualStyleBackColor = false;
            startButton.Click += startButton_Click;
            // 
            // stopButton
            // 
            stopButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            stopButton.BackColor = Color.FromArgb(232, 17, 35);
            stopButton.Enabled = false;
            stopButton.FlatStyle = FlatStyle.Flat;
            stopButton.ForeColor = Color.White;
            stopButton.Location = new Point(138, 445);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(120, 35);
            stopButton.TabIndex = 6;
            stopButton.Text = "Stop Session";
            stopButton.UseVisualStyleBackColor = false;
            stopButton.Click += stopButton_Click;
            // 
            // saveConfigButton
            // 
            saveConfigButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveConfigButton.Enabled = false;
            saveConfigButton.Location = new Point(678, 284);
            saveConfigButton.Name = "saveConfigButton";
            saveConfigButton.Size = new Size(94, 23);
            saveConfigButton.TabIndex = 11;
            saveConfigButton.Text = "Update Config";
            saveConfigButton.UseVisualStyleBackColor = true;
            saveConfigButton.Click += saveConfigButton_Click;
            // 
            // outputLabel
            // 
            outputLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            outputLabel.AutoSize = true;
            outputLabel.Location = new Point(12, 493);
            outputLabel.Name = "outputLabel";
            outputLabel.Size = new Size(48, 15);
            outputLabel.TabIndex = 7;
            outputLabel.Text = "Output:";
            // 
            // outputTextBox
            // 
            outputTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            outputTextBox.BackColor = Color.Black;
            outputTextBox.Font = new Font("Consolas", 9F);
            outputTextBox.ForeColor = Color.LimeGreen;
            outputTextBox.Location = new Point(12, 511);
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
            statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            statusLabel.Location = new Point(264, 454);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(69, 15);
            statusLabel.TabIndex = 9;
            statusLabel.Text = "Status: Idle";
            // 
            // DevForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 723);
            Controls.Add(apolloGraphRefTextBox);
            Controls.Add(apolloGraphRefLabel);
            Controls.Add(apolloKeyTextBox);
            Controls.Add(apolloKeyLabel);
            Controls.Add(coprocessorInfoLabel);
            Controls.Add(enableCoprocessorCheckBox);
            Controls.Add(saveConfigButton);
            Controls.Add(acceptElv2CheckBox);
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
        private CheckBox acceptElv2CheckBox;
        private CheckBox enableCoprocessorCheckBox;
        private Label coprocessorInfoLabel;
        private Label apolloKeyLabel;
        private TextBox apolloKeyTextBox;
        private Label apolloGraphRefLabel;
        private TextBox apolloGraphRefTextBox;
        private Button startButton;
        private Button stopButton;
        private Label outputLabel;
        private TextBox outputTextBox;
        private Label statusLabel;
        private Button saveConfigButton;
    }
}
