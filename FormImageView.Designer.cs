
namespace IntelligentFactory
{
    partial class FormImageView
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            ImageGlass.DefaultGifAnimator defaultGifAnimator1 = new ImageGlass.DefaultGifAnimator();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.btnRst1ToolLoad = new System.Windows.Forms.ToolStripButton();
            this.btnRst1ToolSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFit = new System.Windows.Forms.ToolStripButton();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton9 = new System.Windows.Forms.ToolStripButton();
            this.ibSource = new ImageGlass.ImageBoxEx();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackgroundImage = global::IntelligentFactory.Properties.Resources.ButtonBackGround;
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(3);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(35, 30);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.toolStripButton3,
            this.toolStripButton4,
            this.btnRst1ToolLoad,
            this.btnRst1ToolSave,
            this.toolStripSeparator3,
            this.btnFit,
            this.btnZoomIn,
            this.btnZoomOut,
            this.toolStripButton7,
            this.toolStripButton9});
            this.toolStrip1.Location = new System.Drawing.Point(0, 739);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1014, 28);
            this.toolStrip1.TabIndex = 823;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.AutoSize = false;
            this.toolStripButton2.Checked = true;
            this.toolStripButton2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(125, 50);
            this.toolStripButton2.Text = "Cursor";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.AutoToolTip = false;
            this.toolStripButton3.Checked = true;
            this.toolStripButton3.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Font = new System.Drawing.Font("Arial", 12F);
            this.toolStripButton3.ForeColor = System.Drawing.Color.White;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Margin = new System.Windows.Forms.Padding(1);
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(46, 26);
            this.toolStripButton3.Text = "LIVE";
            this.toolStripButton3.ToolTipText = "Expansion";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Font = new System.Drawing.Font("Arial", 12F);
            this.toolStripButton4.ForeColor = System.Drawing.Color.White;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(57, 25);
            this.toolStripButton4.Text = "GRAB";
            // 
            // btnRst1ToolLoad
            // 
            this.btnRst1ToolLoad.Font = new System.Drawing.Font("Arial", 12F);
            this.btnRst1ToolLoad.ForeColor = System.Drawing.Color.White;
            this.btnRst1ToolLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRst1ToolLoad.Name = "btnRst1ToolLoad";
            this.btnRst1ToolLoad.Size = new System.Drawing.Size(56, 25);
            this.btnRst1ToolLoad.Text = "LOAD";
            // 
            // btnRst1ToolSave
            // 
            this.btnRst1ToolSave.Font = new System.Drawing.Font("Arial", 12F);
            this.btnRst1ToolSave.ForeColor = System.Drawing.Color.White;
            this.btnRst1ToolSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRst1ToolSave.Name = "btnRst1ToolSave";
            this.btnRst1ToolSave.Size = new System.Drawing.Size(55, 25);
            this.btnRst1ToolSave.Text = "SAVE";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // btnFit
            // 
            this.btnFit.Font = new System.Drawing.Font("Arial", 12F);
            this.btnFit.ForeColor = System.Drawing.Color.White;
            this.btnFit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFit.Name = "btnFit";
            this.btnFit.Size = new System.Drawing.Size(30, 25);
            this.btnFit.Text = "Fit";
            this.btnFit.Click += new System.EventHandler(this.btnFit_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Font = new System.Drawing.Font("Arial", 12F);
            this.btnZoomIn.ForeColor = System.Drawing.Color.White;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(67, 25);
            this.btnZoomIn.Text = "Zoom In";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Font = new System.Drawing.Font("Arial", 12F);
            this.btnZoomOut.ForeColor = System.Drawing.Color.White;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(80, 25);
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.Font = new System.Drawing.Font("Arial", 12F);
            this.toolStripButton7.ForeColor = System.Drawing.Color.White;
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(69, 25);
            this.toolStripButton7.Text = "CROSS";
            // 
            // toolStripButton9
            // 
            this.toolStripButton9.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButton9.ForeColor = System.Drawing.Color.White;
            this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton9.Name = "toolStripButton9";
            this.toolStripButton9.Size = new System.Drawing.Size(38, 25);
            this.toolStripButton9.Text = "ROI";
            // 
            // ibSource
            // 
            this.ibSource.Animator = defaultGifAnimator1;
            this.ibSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibSource.Location = new System.Drawing.Point(20, 60);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(974, 687);
            this.ibSource.TabIndex = 824;
            this.ibSource.Zoom = 100D;
            // 
            // FormImageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 767);
            this.Controls.Add(this.ibSource);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormImageView";
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Text = "Viewer";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormImageView_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        internal System.Windows.Forms.ToolStrip toolStrip1;
        internal System.Windows.Forms.ToolStripButton toolStripButton2;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton btnRst1ToolLoad;
        private System.Windows.Forms.ToolStripButton btnRst1ToolSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton toolStripButton9;
        private System.Windows.Forms.ToolStripButton btnFit;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private ImageGlass.ImageBoxEx ibSource;
    }
}
