using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentFactory
{
    public partial class IntelligentImageView : Component
    {
        public float m_fImageScale { get; set; } = 5;
        public float m_fImgW { get; set; } = 0;
        public float m_fImgH { get; set; } = 0;

        public float m_dPenX { get; set; } = 0;
        public float m_dPenY { get; set; } = 0;

        public float m_fMinX { get; private set; } = 1;
        public float m_fMinY { get; private set; } = 1;

        public float m_fMaxX { get; private set; } = 0;
        public float m_fMaxY { get; private set; } = 0;


        bool m_bCenter = true;

        public bool bUseControlMoveCell = false;

        public bool bUseShift = false;

        public bool bUseAlt = false;

        public IntelligentImageView(bool bCenter = true)
        {
            InitializeComponent();


            m_bCenter = bCenter;

        }

        public ImageGlass.ImageBox ibSource = new ImageGlass.ImageBox();

        public IntelligentImageView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void LoadImageBox(ImageGlass.ImageBox ImageBox)
        {
            ibSource = ImageBox;

            //ibSource.ZoomToFit();

            for (int i = 0; i < 10; i++)
            {
                ibSource.ZoomOut();
            }

            if (m_bCenter)
            {
                ibSource.ImageChanged += IbSource_ImageChanged;
            }

            ibSource.MouseWheel += new MouseEventHandler(MouseWheelEvent);

            ibSource.AllowClickZoom = false;

            Color color = Color.FromArgb(20, 20, 20);

            ibSource.GridColor = color;
            ibSource.GridColorAlternate = color;
        }

        public void CenterToImage()
        {
            for (int i = 0; i < 10; i++)
            {
                ibSource.ZoomOut();
            }
            ibSource.CenterToImage();
        }

        private void IbSource_ImageChanged(object sender, EventArgs e)
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    ibSource.ZoomOut();
            //}
            //ibSource.CenterToImage();
        }

        #region ImageBox
        private void MouseWheelEvent(object sender, MouseEventArgs e)
        {
            if (IGlobal.Instance.iSystem.Mode == ISystem.MODE.AUTO) return;
            if (bUseControlMoveCell) { return; }

            if ((e.Delta / 120) > 0)
            {
                // up
                if (m_fImageScale > 1)
                    m_fImageScale--;



                ZoomInImage();
            }
            else
            {
                // down
                m_fImageScale++;

                ZoomOutImage();
            }
        }

        #region Display
        private void ZoomInImage()
        {
            ibSource.ZoomIn();
        }

        private void ZoomOutImage()
        {
            ibSource.ZoomOut();
        }

        private void ZoomFitImage()
        {
            ibSource.ZoomToFit();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            ZoomOutImage();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            ZoomInImage();
        }

        private void btnFit_Click(object sender, EventArgs e)
        {
            ZoomFitImage();
        }
        #endregion

        #endregion
    }
}
