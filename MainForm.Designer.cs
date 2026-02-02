namespace OVRefiner
{
    partial class MainForm
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
            grpType = new GroupBox();
            rbTypeOvpn = new RadioButton();
            rbTypeV2ray = new RadioButton();
            grpInput = new GroupBox();
            rbInputSub = new RadioButton();
            rbInputFile = new RadioButton();
            lblPath = new Label();
            txtInputPath = new TextBox();
            btnBrowse = new Button();
            grpDns = new GroupBox();
            chkDnsBase64 = new CheckBox();
            txtCustomDoh = new TextBox();
            lblCustom = new Label();
            cmbDoh = new ComboBox();
            lblDoh = new Label();
            chkIncludeOriginal = new CheckBox();
            btnStart = new Button();
            rtbLog = new RichTextBox();
            progressBar = new ProgressBar();
            lnkGithub = new LinkLabel();
            grpType.SuspendLayout();
            grpInput.SuspendLayout();
            grpDns.SuspendLayout();
            SuspendLayout();
            // 
            // grpType
            // 
            grpType.Controls.Add(rbTypeOvpn);
            grpType.Controls.Add(rbTypeV2ray);
            grpType.ForeColor = Color.LightGray;
            grpType.Location = new Point(12, 12);
            grpType.Name = "grpType";
            grpType.Size = new Size(300, 60);
            grpType.TabIndex = 0;
            grpType.TabStop = false;
            grpType.Text = "1. Select Config Type";
            // 
            // rbTypeOvpn
            // 
            rbTypeOvpn.AutoSize = true;
            rbTypeOvpn.Location = new Point(160, 25);
            rbTypeOvpn.Name = "rbTypeOvpn";
            rbTypeOvpn.Size = new Size(118, 19);
            rbTypeOvpn.TabIndex = 1;
            rbTypeOvpn.Text = "OpenVPN (.ovpn)";
            rbTypeOvpn.UseVisualStyleBackColor = true;
            rbTypeOvpn.CheckedChanged += RbTypeOvpn_CheckedChanged;
            // 
            // rbTypeV2ray
            // 
            rbTypeV2ray.AutoSize = true;
            rbTypeV2ray.Checked = true;
            rbTypeV2ray.Location = new Point(10, 25);
            rbTypeV2ray.Name = "rbTypeV2ray";
            rbTypeV2ray.Size = new Size(132, 19);
            rbTypeV2ray.TabIndex = 0;
            rbTypeV2ray.TabStop = true;
            rbTypeV2ray.Text = "V2Ray (vless, vmess)";
            rbTypeV2ray.UseVisualStyleBackColor = true;
            // 
            // grpInput
            // 
            grpInput.Controls.Add(rbInputSub);
            grpInput.Controls.Add(rbInputFile);
            grpInput.ForeColor = Color.LightGray;
            grpInput.Location = new Point(320, 12);
            grpInput.Name = "grpInput";
            grpInput.Size = new Size(450, 60);
            grpInput.TabIndex = 1;
            grpInput.TabStop = false;
            grpInput.Text = "2. Input Method";
            // 
            // rbInputSub
            // 
            rbInputSub.AutoSize = true;
            rbInputSub.Location = new Point(200, 25);
            rbInputSub.Name = "rbInputSub";
            rbInputSub.Size = new Size(138, 19);
            rbInputSub.TabIndex = 1;
            rbInputSub.Text = "Subscription (Base64)";
            rbInputSub.UseVisualStyleBackColor = true;
            // 
            // rbInputFile
            // 
            rbInputFile.AutoSize = true;
            rbInputFile.Checked = true;
            rbInputFile.Location = new Point(10, 25);
            rbInputFile.Name = "rbInputFile";
            rbInputFile.Size = new Size(146, 19);
            rbInputFile.TabIndex = 0;
            rbInputFile.TabStop = true;
            rbInputFile.Text = "Text File / OVPN Folder";
            rbInputFile.UseVisualStyleBackColor = true;
            // 
            // lblPath
            // 
            lblPath.AutoSize = true;
            lblPath.Location = new Point(12, 85);
            lblPath.Name = "lblPath";
            lblPath.Size = new Size(34, 15);
            lblPath.TabIndex = 2;
            lblPath.Text = "Path:";
            // 
            // txtInputPath
            // 
            txtInputPath.BackColor = Color.FromArgb(50, 50, 50);
            txtInputPath.BorderStyle = BorderStyle.FixedSingle;
            txtInputPath.ForeColor = Color.White;
            txtInputPath.Location = new Point(52, 82);
            txtInputPath.Name = "txtInputPath";
            txtInputPath.Size = new Size(600, 23);
            txtInputPath.TabIndex = 3;
            // 
            // btnBrowse
            // 
            btnBrowse.BackColor = Color.FromArgb(60, 60, 60);
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.Location = new Point(660, 80);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(110, 27);
            btnBrowse.TabIndex = 4;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = false;
            btnBrowse.Click += BtnBrowse_Click;
            // 
            // grpDns
            // 
            grpDns.Controls.Add(chkDnsBase64);
            grpDns.Controls.Add(txtCustomDoh);
            grpDns.Controls.Add(lblCustom);
            grpDns.Controls.Add(cmbDoh);
            grpDns.Controls.Add(lblDoh);
            grpDns.ForeColor = Color.LightGray;
            grpDns.Location = new Point(12, 120);
            grpDns.Name = "grpDns";
            grpDns.Size = new Size(758, 80);
            grpDns.TabIndex = 5;
            grpDns.TabStop = false;
            grpDns.Text = "3. DNS / DoH Settings";
            // 
            // chkDnsBase64
            // 
            chkDnsBase64.AutoSize = true;
            chkDnsBase64.Enabled = false;
            chkDnsBase64.Location = new Point(320, 50);
            chkDnsBase64.Name = "chkDnsBase64";
            chkDnsBase64.Size = new Size(250, 19);
            chkDnsBase64.TabIndex = 4;
            chkDnsBase64.Text = "Use Base64 URL Encoding (Binary Request)";
            chkDnsBase64.UseVisualStyleBackColor = true;
            // 
            // txtCustomDoh
            // 
            txtCustomDoh.BackColor = Color.FromArgb(50, 50, 50);
            txtCustomDoh.BorderStyle = BorderStyle.FixedSingle;
            txtCustomDoh.Enabled = false;
            txtCustomDoh.ForeColor = Color.White;
            txtCustomDoh.Location = new Point(320, 22);
            txtCustomDoh.Name = "txtCustomDoh";
            txtCustomDoh.Size = new Size(400, 23);
            txtCustomDoh.TabIndex = 3;
            // 
            // lblCustom
            // 
            lblCustom.AutoSize = true;
            lblCustom.Location = new Point(240, 25);
            lblCustom.Name = "lblCustom";
            lblCustom.Size = new Size(76, 15);
            lblCustom.TabIndex = 2;
            lblCustom.Text = "Custom URL:";
            // 
            // cmbDoh
            // 
            cmbDoh.BackColor = Color.White;
            cmbDoh.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDoh.FormattingEnabled = true;
            cmbDoh.Items.AddRange(new object[] { "Google DNS", "Cloudflare", "Custom" });
            cmbDoh.Location = new Point(70, 22);
            cmbDoh.Name = "cmbDoh";
            cmbDoh.Size = new Size(150, 23);
            cmbDoh.TabIndex = 1;
            cmbDoh.SelectedIndexChanged += CmbDoh_SelectedIndexChanged;
            // 
            // lblDoh
            // 
            lblDoh.AutoSize = true;
            lblDoh.Location = new Point(10, 25);
            lblDoh.Name = "lblDoh";
            lblDoh.Size = new Size(54, 15);
            lblDoh.TabIndex = 0;
            lblDoh.Text = "Provider:";
            // 
            // chkIncludeOriginal
            // 
            chkIncludeOriginal.AutoSize = true;
            chkIncludeOriginal.Location = new Point(20, 213);
            chkIncludeOriginal.Name = "chkIncludeOriginal";
            chkIncludeOriginal.Size = new Size(279, 19);
            chkIncludeOriginal.TabIndex = 6;
            chkIncludeOriginal.Text = "Include Original Configs in Output (V2Ray Only)";
            chkIncludeOriginal.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.SlateBlue;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnStart.ForeColor = Color.White;
            btnStart.Location = new Point(560, 205);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(210, 35);
            btnStart.TabIndex = 7;
            btnStart.Text = "START PROCESS";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += BtnStart_Click;
            // 
            // rtbLog
            // 
            rtbLog.BackColor = Color.Black;
            rtbLog.BorderStyle = BorderStyle.None;
            rtbLog.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            rtbLog.ForeColor = Color.White;
            rtbLog.Location = new Point(12, 250);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new Size(758, 300);
            rtbLog.TabIndex = 8;
            rtbLog.Text = "";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 560);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(758, 10);
            progressBar.TabIndex = 9;
            // 
            // lnkGithub
            // 
            lnkGithub.ActiveLinkColor = Color.White;
            lnkGithub.AutoSize = true;
            lnkGithub.LinkColor = Color.CornflowerBlue;
            lnkGithub.Location = new Point(368, 215);
            lnkGithub.Name = "lnkGithub";
            lnkGithub.Size = new Size(176, 15);
            lnkGithub.TabIndex = 10;
            lnkGithub.TabStop = true;
            lnkGithub.Text = "Developed by SinorDev | GitHub";
            lnkGithub.TextAlign = ContentAlignment.TopRight;
            lnkGithub.VisitedLinkColor = Color.CornflowerBlue;
            lnkGithub.LinkClicked += LnkGithub_LinkClicked;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(784, 580);
            Controls.Add(lnkGithub);
            Controls.Add(progressBar);
            Controls.Add(rtbLog);
            Controls.Add(btnStart);
            Controls.Add(chkIncludeOriginal);
            Controls.Add(grpDns);
            Controls.Add(btnBrowse);
            Controls.Add(txtInputPath);
            Controls.Add(lblPath);
            Controls.Add(grpInput);
            Controls.Add(grpType);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ForeColor = Color.WhiteSmoke;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Config Splitter Pro";
            grpType.ResumeLayout(false);
            grpType.PerformLayout();
            grpInput.ResumeLayout(false);
            grpInput.PerformLayout();
            grpDns.ResumeLayout(false);
            grpDns.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpType;
        private System.Windows.Forms.RadioButton rbTypeOvpn;
        private System.Windows.Forms.RadioButton rbTypeV2ray;
        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.RadioButton rbInputSub;
        private System.Windows.Forms.RadioButton rbInputFile;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox txtInputPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox grpDns;
        private System.Windows.Forms.TextBox txtCustomDoh;
        private System.Windows.Forms.Label lblCustom;
        private System.Windows.Forms.ComboBox cmbDoh;
        private System.Windows.Forms.Label lblDoh;
        private System.Windows.Forms.CheckBox chkDnsBase64;
        private System.Windows.Forms.CheckBox chkIncludeOriginal;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.LinkLabel lnkGithub;
    }
}