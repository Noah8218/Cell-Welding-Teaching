#if MATROX
namespace IntelligentFactory
{
    partial class FormSettings_CVEdge
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
            ImageGlass.DefaultGifAnimator defaultGifAnimator1 = new ImageGlass.DefaultGifAnimator();
            ImageGlass.DefaultGifAnimator defaultGifAnimator2 = new ImageGlass.DefaultGifAnimator();
            this.ibMaskingTemplate = new ImageGlass.ImageBoxEx();
            this.metroLabel32 = new MetroFramework.Controls.MetroLabel();
            this.btnTopEdgeROI = new MetroFramework.Controls.MetroButton();
            this.btnTubingEdgeROI = new MetroFramework.Controls.MetroButton();
            this.metroLabel11 = new MetroFramework.Controls.MetroLabel();
            this.btnMaskingTrain = new MetroFramework.Controls.MetroButton();
            this.metroLabel15 = new MetroFramework.Controls.MetroLabel();
            this.lbParameter = new MetroFramework.Controls.MetroLabel();
            this.metroLabel29 = new MetroFramework.Controls.MetroLabel();
            this.propertygrid_Parameter = new System.Windows.Forms.PropertyGrid();
            this.ibSource = new ImageGlass.ImageBoxEx();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnRun = new MetroFramework.Controls.MetroButton();
            this.btnImageLoad = new MetroFramework.Controls.MetroButton();
            this.btnMaskingROI = new MetroFramework.Controls.MetroButton();
            this.lbThreshold = new MetroFramework.Controls.MetroLabel();
            this.trbThreshold = new MetroFramework.Controls.MetroTrackBar();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // ibMaskingTemplate
            // 
            this.ibMaskingTemplate.Animator = defaultGifAnimator1;
            this.ibMaskingTemplate.Location = new System.Drawing.Point(170, 523);
            this.ibMaskingTemplate.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.ibMaskingTemplate.Name = "ibMaskingTemplate";
            this.ibMaskingTemplate.Size = new System.Drawing.Size(129, 94);
            this.ibMaskingTemplate.TabIndex = 1119;
            this.ibMaskingTemplate.Zoom = 100D;
            // 
            // metroLabel32
            // 
            this.metroLabel32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel32.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel32.ForeColor = System.Drawing.Color.White;
            this.metroLabel32.Location = new System.Drawing.Point(20, 406);
            this.metroLabel32.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.metroLabel32.Name = "metroLabel32";
            this.metroLabel32.Size = new System.Drawing.Size(149, 38);
            this.metroLabel32.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel32.TabIndex = 1118;
            this.metroLabel32.Text = "1. TOP EDGE";
            this.metroLabel32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.metroLabel32.UseCustomBackColor = true;
            this.metroLabel32.UseCustomForeColor = true;
            this.metroLabel32.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClickParameter);
            // 
            // btnTopEdgeROI
            // 
            this.btnTopEdgeROI.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnTopEdgeROI.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnTopEdgeROI.Highlight = true;
            this.btnTopEdgeROI.Location = new System.Drawing.Point(170, 406);
            this.btnTopEdgeROI.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnTopEdgeROI.Name = "btnTopEdgeROI";
            this.btnTopEdgeROI.Size = new System.Drawing.Size(128, 38);
            this.btnTopEdgeROI.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnTopEdgeROI.TabIndex = 1117;
            this.btnTopEdgeROI.Text = "ROI";
            this.btnTopEdgeROI.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnTopEdgeROI.UseSelectable = true;
            this.btnTopEdgeROI.Click += new System.EventHandler(this.btnTopEdgeROI_Click);
            // 
            // btnTubingEdgeROI
            // 
            this.btnTubingEdgeROI.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnTubingEdgeROI.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnTubingEdgeROI.Highlight = true;
            this.btnTubingEdgeROI.Location = new System.Drawing.Point(170, 445);
            this.btnTubingEdgeROI.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnTubingEdgeROI.Name = "btnTubingEdgeROI";
            this.btnTubingEdgeROI.Size = new System.Drawing.Size(128, 38);
            this.btnTubingEdgeROI.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnTubingEdgeROI.TabIndex = 1113;
            this.btnTubingEdgeROI.Text = "ROI";
            this.btnTubingEdgeROI.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnTubingEdgeROI.UseSelectable = true;
            this.btnTubingEdgeROI.Click += new System.EventHandler(this.btnTubingEdgeROI_Click);
            // 
            // metroLabel11
            // 
            this.metroLabel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel11.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel11.ForeColor = System.Drawing.Color.White;
            this.metroLabel11.Location = new System.Drawing.Point(20, 445);
            this.metroLabel11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.metroLabel11.Name = "metroLabel11";
            this.metroLabel11.Size = new System.Drawing.Size(149, 38);
            this.metroLabel11.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel11.TabIndex = 1114;
            this.metroLabel11.Text = "2. TUBING EDGE";
            this.metroLabel11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.metroLabel11.UseCustomBackColor = true;
            this.metroLabel11.UseCustomForeColor = true;
            this.metroLabel11.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClickParameter);
            // 
            // btnMaskingTrain
            // 
            this.btnMaskingTrain.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnMaskingTrain.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnMaskingTrain.Highlight = true;
            this.btnMaskingTrain.Location = new System.Drawing.Point(170, 484);
            this.btnMaskingTrain.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnMaskingTrain.Name = "btnMaskingTrain";
            this.btnMaskingTrain.Size = new System.Drawing.Size(128, 38);
            this.btnMaskingTrain.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnMaskingTrain.TabIndex = 1115;
            this.btnMaskingTrain.Text = "Train";
            this.btnMaskingTrain.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnMaskingTrain.UseSelectable = true;
            this.btnMaskingTrain.Click += new System.EventHandler(this.btnMaskingTrain_Click);
            // 
            // metroLabel15
            // 
            this.metroLabel15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel15.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel15.ForeColor = System.Drawing.Color.White;
            this.metroLabel15.Location = new System.Drawing.Point(20, 484);
            this.metroLabel15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.metroLabel15.Name = "metroLabel15";
            this.metroLabel15.Size = new System.Drawing.Size(149, 93);
            this.metroLabel15.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel15.TabIndex = 1116;
            this.metroLabel15.Text = "3. MASKING";
            this.metroLabel15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.metroLabel15.UseCustomBackColor = true;
            this.metroLabel15.UseCustomForeColor = true;
            this.metroLabel15.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClickParameter);
            // 
            // lbParameter
            // 
            this.lbParameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbParameter.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbParameter.ForeColor = System.Drawing.Color.White;
            this.lbParameter.Location = new System.Drawing.Point(449, 41);
            this.lbParameter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbParameter.Name = "lbParameter";
            this.lbParameter.Size = new System.Drawing.Size(301, 38);
            this.lbParameter.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbParameter.TabIndex = 1122;
            this.lbParameter.Text = "-";
            this.lbParameter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbParameter.UseCustomBackColor = true;
            this.lbParameter.UseCustomForeColor = true;
            // 
            // metroLabel29
            // 
            this.metroLabel29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel29.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel29.ForeColor = System.Drawing.Color.White;
            this.metroLabel29.Location = new System.Drawing.Point(299, 41);
            this.metroLabel29.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.metroLabel29.Name = "metroLabel29";
            this.metroLabel29.Size = new System.Drawing.Size(149, 38);
            this.metroLabel29.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel29.TabIndex = 1121;
            this.metroLabel29.Text = "Parameter";
            this.metroLabel29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel29.UseCustomBackColor = true;
            this.metroLabel29.UseCustomForeColor = true;
            // 
            // propertygrid_Parameter
            // 
            this.propertygrid_Parameter.BackColor = System.Drawing.Color.Black;
            this.propertygrid_Parameter.CategoryForeColor = System.Drawing.Color.White;
            this.propertygrid_Parameter.CategorySplitterColor = System.Drawing.Color.Black;
            this.propertygrid_Parameter.CommandsBorderColor = System.Drawing.Color.Black;
            this.propertygrid_Parameter.CommandsDisabledLinkColor = System.Drawing.Color.White;
            this.propertygrid_Parameter.CommandsForeColor = System.Drawing.Color.White;
            this.propertygrid_Parameter.DisabledItemForeColor = System.Drawing.Color.White;
            this.propertygrid_Parameter.HelpBackColor = System.Drawing.Color.Black;
            this.propertygrid_Parameter.HelpBorderColor = System.Drawing.Color.Teal;
            this.propertygrid_Parameter.HelpForeColor = System.Drawing.Color.White;
            this.propertygrid_Parameter.LineColor = System.Drawing.Color.Teal;
            this.propertygrid_Parameter.Location = new System.Drawing.Point(299, 80);
            this.propertygrid_Parameter.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.propertygrid_Parameter.Name = "propertygrid_Parameter";
            this.propertygrid_Parameter.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertygrid_Parameter.SelectedItemWithFocusBackColor = System.Drawing.Color.Black;
            this.propertygrid_Parameter.SelectedItemWithFocusForeColor = System.Drawing.Color.Teal;
            this.propertygrid_Parameter.Size = new System.Drawing.Size(451, 536);
            this.propertygrid_Parameter.TabIndex = 1120;
            this.propertygrid_Parameter.ToolbarVisible = false;
            this.propertygrid_Parameter.ViewBackColor = System.Drawing.Color.Black;
            this.propertygrid_Parameter.ViewBorderColor = System.Drawing.Color.Teal;
            this.propertygrid_Parameter.ViewForeColor = System.Drawing.Color.White;
            // 
            // ibSource
            // 
            this.ibSource.Animator = defaultGifAnimator2;
            this.ibSource.Location = new System.Drawing.Point(20, 80);
            this.ibSource.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.ibSource.Name = "ibSource";
            this.ibSource.Size = new System.Drawing.Size(278, 285);
            this.ibSource.TabIndex = 1123;
            this.ibSource.Zoom = 100D;
            this.ibSource.DoubleClick += new System.EventHandler(this.ibSource_DoubleClick);
            // 
            // metroLabel1
            // 
            this.metroLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel1.ForeColor = System.Drawing.Color.White;
            this.metroLabel1.Location = new System.Drawing.Point(20, 41);
            this.metroLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(149, 38);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroLabel1.TabIndex = 1124;
            this.metroLabel1.Text = "Image";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel1.UseCustomBackColor = true;
            this.metroLabel1.UseCustomForeColor = true;
            // 
            // btnSave
            // 
            this.btnSave.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnSave.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnSave.Highlight = true;
            this.btnSave.Location = new System.Drawing.Point(526, 619);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(224, 38);
            this.btnSave.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSave.TabIndex = 1125;
            this.btnSave.Text = "Save";
            this.btnSave.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRun
            // 
            this.btnRun.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRun.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnRun.Highlight = true;
            this.btnRun.Location = new System.Drawing.Point(299, 619);
            this.btnRun.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(224, 38);
            this.btnRun.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnRun.TabIndex = 1126;
            this.btnRun.Text = "Run";
            this.btnRun.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnRun.UseSelectable = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnImageLoad
            // 
            this.btnImageLoad.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnImageLoad.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnImageLoad.Highlight = true;
            this.btnImageLoad.Location = new System.Drawing.Point(170, 41);
            this.btnImageLoad.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnImageLoad.Name = "btnImageLoad";
            this.btnImageLoad.Size = new System.Drawing.Size(128, 38);
            this.btnImageLoad.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnImageLoad.TabIndex = 1127;
            this.btnImageLoad.Text = "LOAD";
            this.btnImageLoad.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnImageLoad.UseSelectable = true;
            this.btnImageLoad.Click += new System.EventHandler(this.btnImageLoad_Click);
            // 
            // btnMaskingROI
            // 
            this.btnMaskingROI.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnMaskingROI.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnMaskingROI.Highlight = true;
            this.btnMaskingROI.Location = new System.Drawing.Point(170, 619);
            this.btnMaskingROI.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnMaskingROI.Name = "btnMaskingROI";
            this.btnMaskingROI.Size = new System.Drawing.Size(128, 38);
            this.btnMaskingROI.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnMaskingROI.TabIndex = 1128;
            this.btnMaskingROI.Text = "ROI";
            this.btnMaskingROI.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnMaskingROI.UseSelectable = true;
            this.btnMaskingROI.Click += new System.EventHandler(this.btnMaskingROI_Click);
            // 
            // lbThreshold
            // 
            this.lbThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbThreshold.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbThreshold.ForeColor = System.Drawing.Color.White;
            this.lbThreshold.Location = new System.Drawing.Point(229, 366);
            this.lbThreshold.Name = "lbThreshold";
            this.lbThreshold.Size = new System.Drawing.Size(69, 38);
            this.lbThreshold.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbThreshold.TabIndex = 1130;
            this.lbThreshold.Text = "000";
            this.lbThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbThreshold.UseCustomBackColor = true;
            this.lbThreshold.UseCustomForeColor = true;
            // 
            // trbThreshold
            // 
            this.trbThreshold.BackColor = System.Drawing.Color.Transparent;
            this.trbThreshold.Location = new System.Drawing.Point(20, 365);
            this.trbThreshold.Maximum = 254;
            this.trbThreshold.Minimum = 1;
            this.trbThreshold.Name = "trbThreshold";
            this.trbThreshold.Size = new System.Drawing.Size(206, 38);
            this.trbThreshold.TabIndex = 1129;
            this.trbThreshold.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.trbThreshold.Scroll += new System.Windows.Forms.ScrollEventHandler(this.trbThreshold_Scroll);
            // 
            // metroButton1
            // 
            this.metroButton1.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.metroButton1.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.metroButton1.Highlight = true;
            this.metroButton1.Location = new System.Drawing.Point(20, 619);
            this.metroButton1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(149, 38);
            this.metroButton1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroButton1.TabIndex = 1131;
            this.metroButton1.Text = "ADAPTIVE FILTER";
            this.metroButton1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroButton1.UseSelectable = true;
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // FormSettings_CVEdge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 666);
            this.Controls.Add(this.metroButton1);
            this.Controls.Add(this.lbThreshold);
            this.Controls.Add(this.trbThreshold);
            this.Controls.Add(this.btnMaskingROI);
            this.Controls.Add(this.btnImageLoad);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.ibSource);
            this.Controls.Add(this.lbParameter);
            this.Controls.Add(this.metroLabel29);
            this.Controls.Add(this.propertygrid_Parameter);
            this.Controls.Add(this.ibMaskingTemplate);
            this.Controls.Add(this.metroLabel32);
            this.Controls.Add(this.btnTopEdgeROI);
            this.Controls.Add(this.btnTubingEdgeROI);
            this.Controls.Add(this.metroLabel11);
            this.Controls.Add(this.btnMaskingTrain);
            this.Controls.Add(this.metroLabel15);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "FormSettings_CVEdge";
            this.Padding = new System.Windows.Forms.Padding(17, 65, 17, 22);
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.FormSettings_CVEdge_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageGlass.ImageBoxEx ibMaskingTemplate;
        private MetroFramework.Controls.MetroLabel metroLabel32;
        private MetroFramework.Controls.MetroButton btnTopEdgeROI;
        private MetroFramework.Controls.MetroButton btnTubingEdgeROI;
        private MetroFramework.Controls.MetroLabel metroLabel11;
        private MetroFramework.Controls.MetroButton btnMaskingTrain;
        private MetroFramework.Controls.MetroLabel metroLabel15;
        private MetroFramework.Controls.MetroLabel lbParameter;
        private MetroFramework.Controls.MetroLabel metroLabel29;
        private System.Windows.Forms.PropertyGrid propertygrid_Parameter;
        private ImageGlass.ImageBoxEx ibSource;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnRun;
        private MetroFramework.Controls.MetroButton btnImageLoad;
        private MetroFramework.Controls.MetroButton btnMaskingROI;
        private MetroFramework.Controls.MetroLabel lbThreshold;
        private MetroFramework.Controls.MetroTrackBar trbThreshold;
        private MetroFramework.Controls.MetroButton metroButton1;
    }
} 
#endif