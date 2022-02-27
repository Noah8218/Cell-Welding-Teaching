﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace IntelligentFactory
{
    class RichTextBoxEx : RichTextBox
    {
        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);

        public RichTextBoxEx()
        {
            this.MouseDoubleClick += RichTextBoxEx_MouseDoubleClick;
        }

        private void RichTextBoxEx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IGlobal.Instance.iSystem.EventChangeSize != null)
            {
                IGlobal.Instance.iSystem.EventChangeSize(null, null);
            }
        }

        protected override void WndProc(ref Message m)
        {
            //WM_LBUTTONDOWN
            if (m.Msg == 0x201)
                HideCaret(m.HWnd);
            else
                base.WndProc(ref m);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RichTextBoxEx
            // 
            this.Font = new System.Drawing.Font("돋움체", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ResumeLayout(false);

        }
    }
}