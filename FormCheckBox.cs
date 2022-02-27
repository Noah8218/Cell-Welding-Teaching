using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace IntelligentFactory
{
    public partial class FormCheckBox : System.Windows.Forms.Control
    {

        private bool _check = false;

        public bool Check
        {
            get
            {
                return _check;
            }
            set
            {
                _check = value;
                Invalidate();
            }
        }
        public FormCheckBox()
        {
            InitializeComponent();
        }

        public FormCheckBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;


            g.FillRectangle(new SolidBrush(Color.Transparent), this.ClientRectangle);



            ControlPaint.DrawCheckBox(g, 1, 1, this.ClientRectangle.Height - 2, this.ClientRectangle.Height - 2, _check ? ButtonState.Checked : ButtonState.Normal);



            g.DrawString(this.Text, this.Font, new SolidBrush(Color.Black), this.ClientRectangle.Height + 2, (this.Height - g.MeasureString(this.Text, this.Font).Height) / 2);
        }

        public void FormCheckBox_Click(object sender, System.EventArgs e)
        {
            _check = !_check;
            Invalidate();
        }


    }
}
