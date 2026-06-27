namespace RoverDotNet.Demo.Forms
{
    partial class ConfigAuthForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            resultTextBox = new TextBox();
            closeButton = new Button();
            titleLabel = new Label();
            SuspendLayout();
            // 
            // resultTextBox
            // 
            resultTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            resultTextBox.Font = new Font("Consolas", 9F);
            resultTextBox.Location = new Point(12, 52);
            resultTextBox.Multiline = true;
            resultTextBox.Name = "resultTextBox";
            resultTextBox.ReadOnly = true;
            resultTextBox.ScrollBars = ScrollBars.Vertical;
            resultTextBox.Size = new Size(576, 308);
            resultTextBox.TabIndex = 0;
            // 
            // closeButton
            // 
            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeButton.Location = new Point(513, 366);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(75, 23);
            closeButton.TabIndex = 1;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += closeButton_Click;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            titleLabel.Location = new Point(12, 18);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(147, 21);
            titleLabel.TabIndex = 2;
            titleLabel.Text = "Config Auth Result";
            // 
            // ConfigAuthForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 400);
            Controls.Add(titleLabel);
            Controls.Add(closeButton);
            Controls.Add(resultTextBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConfigAuthForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Config Auth";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox resultTextBox;
        private Button closeButton;
        private Label titleLabel;
    }
}
