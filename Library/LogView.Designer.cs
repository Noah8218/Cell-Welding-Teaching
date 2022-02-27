namespace IntelligentFactory
{
    partial class LogView
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerDisplayLog = new System.Windows.Forms.Timer(this.components);
            this.richTextBoxExLog = new IntelligentFactory.RichTextBoxEx();
            this.SuspendLayout();
            // 
            // timerDisplayLog
            // 
            this.timerDisplayLog.Interval = 1000;
            this.timerDisplayLog.Tick += new System.EventHandler(this.timerDisplayLog_Tick);
            // 
            // richTextBoxExLog
            // 
            this.richTextBoxExLog.BackColor = System.Drawing.Color.Black;
            this.richTextBoxExLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxExLog.Font = new System.Drawing.Font("D2Coding ligature", 8F);
            this.richTextBoxExLog.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxExLog.Name = "richTextBoxExLog";
            this.richTextBoxExLog.ReadOnly = true;
            this.richTextBoxExLog.ShortcutsEnabled = false;
            this.richTextBoxExLog.Size = new System.Drawing.Size(163, 128);
            this.richTextBoxExLog.TabIndex = 1;
            this.richTextBoxExLog.Text = "";
            this.richTextBoxExLog.WordWrap = false;
            // 
            // LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.richTextBoxExLog);
            this.DoubleBuffered = true;
            this.Name = "LogView";
            this.Size = new System.Drawing.Size(163, 128);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerDisplayLog;
        private RichTextBoxEx richTextBoxExLog;
    }
}
