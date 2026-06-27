namespace RoverDotNet.Demo
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            profileLabel = new Label();
            profileTextBox = new TextBox();
            configWhoAmIButton = new Button();
            configAuthButton = new Button();
            devButton = new Button();
            operationsGroupBox = new GroupBox();
            titleLabel = new Label();
            operationsGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // profileLabel
            // 
            profileLabel.AutoSize = true;
            profileLabel.Location = new Point(12, 67);
            profileLabel.Name = "profileLabel";
            profileLabel.Size = new Size(143, 15);
            profileLabel.TabIndex = 0;
            profileLabel.Text = "Profile Name (optional):";
            // 
            // profileTextBox
            // 
            profileTextBox.Location = new Point(161, 64);
            profileTextBox.Name = "profileTextBox";
            profileTextBox.PlaceholderText = "default";
            profileTextBox.Size = new Size(200, 23);
            profileTextBox.TabIndex = 1;
            // 
            // configWhoAmIButton
            // 
            configWhoAmIButton.Location = new Point(15, 32);
            configWhoAmIButton.Name = "configWhoAmIButton";
            configWhoAmIButton.Size = new Size(200, 40);
            configWhoAmIButton.TabIndex = 2;
            configWhoAmIButton.Text = "Config WhoAmI";
            configWhoAmIButton.UseVisualStyleBackColor = true;
            configWhoAmIButton.Click += configWhoAmIButton_Click;
            // 
            // configAuthButton
            // 
            configAuthButton.Location = new Point(15, 88);
            configAuthButton.Name = "configAuthButton";
            configAuthButton.Size = new Size(200, 40);
            configAuthButton.TabIndex = 3;
            configAuthButton.Text = "Config Auth";
            configAuthButton.UseVisualStyleBackColor = true;
            configAuthButton.Click += configAuthButton_Click;
            // 
            // devButton
            // 
            devButton.Location = new Point(15, 144);
            devButton.Name = "devButton";
            devButton.Size = new Size(200, 40);
            devButton.TabIndex = 4;
            devButton.Text = "Dev";
            devButton.UseVisualStyleBackColor = true;
            devButton.Click += devButton_Click;
            // 
            // operationsGroupBox
            // 
            operationsGroupBox.Controls.Add(configWhoAmIButton);
            operationsGroupBox.Controls.Add(devButton);
            operationsGroupBox.Controls.Add(configAuthButton);
            operationsGroupBox.Location = new Point(12, 110);
            operationsGroupBox.Name = "operationsGroupBox";
            operationsGroupBox.Size = new Size(349, 200);
            operationsGroupBox.TabIndex = 5;
            operationsGroupBox.TabStop = false;
            operationsGroupBox.Text = "Operations";
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            titleLabel.Location = new Point(12, 18);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(184, 30);
            titleLabel.TabIndex = 6;
            titleLabel.Text = "RoverDotNet Demo";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(373, 330);
            Controls.Add(titleLabel);
            Controls.Add(operationsGroupBox);
            Controls.Add(profileTextBox);
            Controls.Add(profileLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "RoverDotNet Demo";
            operationsGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label profileLabel;
        private TextBox profileTextBox;
        private Button configWhoAmIButton;
        private Button configAuthButton;
        private Button devButton;
        private GroupBox operationsGroupBox;
        private Label titleLabel;
    }
}
