namespace IntelligentFactory
{
    partial class FormPermission
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
            this.components = new System.ComponentModel.Container();
            this.metroTile4 = new MetroFramework.Controls.MetroTile();
            this.cbPermission = new MetroFramework.Controls.MetroComboBox();
            this.metroTile3 = new MetroFramework.Controls.MetroTile();
            this.tbPassword = new MetroFramework.Controls.MetroTextBox();
            this.metroStyleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.btnLogin = new MetroFramework.Controls.MetroButton();
            this.lbNotice = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).BeginInit();
            this.SuspendLayout();
            // 
            // metroTile4
            // 
            this.metroTile4.ActiveControl = null;
            this.metroTile4.BackColor = System.Drawing.Color.Transparent;
            this.metroTile4.Location = new System.Drawing.Point(12, 73);
            this.metroTile4.Name = "metroTile4";
            this.metroTile4.Size = new System.Drawing.Size(139, 35);
            this.metroTile4.TabIndex = 961;
            this.metroTile4.Text = "Permission";
            this.metroTile4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile4.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile4.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile4.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile4.UseSelectable = true;
            // 
            // cbPermission
            // 
            this.cbPermission.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbPermission.FormattingEnabled = true;
            this.cbPermission.ItemHeight = 29;
            this.cbPermission.Items.AddRange(new object[] {
            "Operator",
            "Engineer",
            "Administrator"});
            this.cbPermission.Location = new System.Drawing.Point(152, 73);
            this.cbPermission.Name = "cbPermission";
            this.cbPermission.Size = new System.Drawing.Size(200, 35);
            this.cbPermission.TabIndex = 960;
            this.cbPermission.Theme = MetroFramework.MetroThemeStyle.Light;
            this.cbPermission.UseSelectable = true;
            // 
            // metroTile3
            // 
            this.metroTile3.ActiveControl = null;
            this.metroTile3.BackColor = System.Drawing.Color.Transparent;
            this.metroTile3.Location = new System.Drawing.Point(12, 110);
            this.metroTile3.Name = "metroTile3";
            this.metroTile3.Size = new System.Drawing.Size(139, 35);
            this.metroTile3.TabIndex = 1018;
            this.metroTile3.Text = "Password";
            this.metroTile3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile3.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile3.TileTextFontSize = MetroFramework.MetroTileTextSize.Small;
            this.metroTile3.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile3.UseSelectable = true;
            // 
            // tbPassword
            // 
            // 
            // 
            // 
            this.tbPassword.CustomButton.Image = null;
            this.tbPassword.CustomButton.Location = new System.Drawing.Point(166, 1);
            this.tbPassword.CustomButton.Name = "";
            this.tbPassword.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.tbPassword.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbPassword.CustomButton.TabIndex = 1;
            this.tbPassword.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbPassword.CustomButton.UseSelectable = true;
            this.tbPassword.DisplayIcon = true;
            this.tbPassword.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbPassword.Lines = new string[0];
            this.tbPassword.Location = new System.Drawing.Point(152, 110);
            this.tbPassword.MaxLength = 32767;
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbPassword.SelectedText = "";
            this.tbPassword.SelectionLength = 0;
            this.tbPassword.SelectionStart = 0;
            this.tbPassword.ShortcutsEnabled = true;
            this.tbPassword.ShowButton = true;
            this.tbPassword.ShowClearButton = true;
            this.tbPassword.Size = new System.Drawing.Size(200, 35);
            this.tbPassword.TabIndex = 1017;
            this.tbPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPassword.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbPassword.UseSelectable = true;
            this.tbPassword.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbPassword.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroStyleManager
            // 
            this.metroStyleManager.Owner = this;
            this.metroStyleManager.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroStyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // btnLogin
            // 
            this.btnLogin.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnLogin.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnLogin.Highlight = true;
            this.btnLogin.Location = new System.Drawing.Point(12, 172);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(340, 35);
            this.btnLogin.TabIndex = 1019;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseSelectable = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lbNotice
            // 
            this.lbNotice.AutoSize = true;
            this.lbNotice.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.lbNotice.Location = new System.Drawing.Point(100, 150);
            this.lbNotice.Name = "lbNotice";
            this.lbNotice.Size = new System.Drawing.Size(183, 19);
            this.lbNotice.Style = MetroFramework.MetroColorStyle.Silver;
            this.lbNotice.TabIndex = 1020;
            this.lbNotice.Text = "Please Enter the Password";
            this.lbNotice.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // FormPermission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 230);
            this.Controls.Add(this.lbNotice);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.metroTile3);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.metroTile4);
            this.Controls.Add(this.cbPermission);
            this.Name = "FormPermission";
            this.Resizable = false;
            this.Text = "Permission";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.FormTeachingSelect_Load);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTile metroTile4;
        private MetroFramework.Controls.MetroComboBox cbPermission;
        private MetroFramework.Controls.MetroTile metroTile3;
        private MetroFramework.Controls.MetroTextBox tbPassword;
        private MetroFramework.Components.MetroStyleManager metroStyleManager;
        private MetroFramework.Controls.MetroButton btnLogin;
        private MetroFramework.Controls.MetroLabel lbNotice;
    }
}