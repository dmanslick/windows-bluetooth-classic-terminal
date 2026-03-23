namespace Bluetooth_Classic_Terminal
{
    partial class Form1
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
            cmbDevices = new ComboBox();
            btnRefresh = new Button();
            btnConnect = new Button();
            btnDisconnect = new Button();
            lblStatus = new Label();
            txtOutput = new TextBox();
            txtInput = new TextBox();
            btnSend = new Button();
            SuspendLayout();
            // 
            // cmbDevices
            // 
            cmbDevices.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbDevices.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDevices.FormattingEnabled = true;
            cmbDevices.Location = new Point(12, 12);
            cmbDevices.Name = "cmbDevices";
            cmbDevices.Size = new Size(500, 23);
            cmbDevices.TabIndex = 0;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Location = new Point(518, 11);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(80, 25);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnConnect
            // 
            btnConnect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConnect.Location = new Point(604, 11);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(80, 25);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(690, 11);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(90, 25);
            btnDisconnect.TabIndex = 3;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 44);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(113, 15);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Status: Disconnected";
            // 
            // txtOutput
            // 
            txtOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtOutput.Location = new Point(12, 62);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ReadOnly = true;
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Size = new Size(768, 337);
            txtOutput.TabIndex = 5;
            // 
            // txtInput
            // 
            txtInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtInput.Enabled = false;
            txtInput.Location = new Point(12, 409);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(676, 23);
            txtInput.TabIndex = 6;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Enabled = false;
            btnSend.Location = new Point(694, 408);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(86, 25);
            btnSend.TabIndex = 7;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(792, 444);
            Controls.Add(btnSend);
            Controls.Add(txtInput);
            Controls.Add(txtOutput);
            Controls.Add(lblStatus);
            Controls.Add(btnDisconnect);
            Controls.Add(btnConnect);
            Controls.Add(btnRefresh);
            Controls.Add(cmbDevices);
            MinimumSize = new Size(700, 400);
            Name = "Form1";
            Text = "Bluetooth Classic Terminal";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbDevices;
        private Button btnRefresh;
        private Button btnConnect;
        private Button btnDisconnect;
        private Label lblStatus;
        private TextBox txtOutput;
        private TextBox txtInput;
        private Button btnSend;
    }
}
