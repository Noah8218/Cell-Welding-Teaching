namespace IntelligentFactory
{
    partial class FormRecipe
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRecipe));
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.dgvRecipeList = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnRecipeModify = new System.Windows.Forms.Panel();
            this.btnModifyRecipeCancel = new MetroFramework.Controls.MetroButton();
            this.btnModifyRecipeCreate = new MetroFramework.Controls.MetroButton();
            this.metroTile7 = new MetroFramework.Controls.MetroTile();
            this.cbRecipeRefModifyList = new MetroFramework.Controls.MetroComboBox();
            this.metroTile8 = new MetroFramework.Controls.MetroTile();
            this.metroTile9 = new MetroFramework.Controls.MetroTile();
            this.tbRecipeModifyName = new MetroFramework.Controls.MetroTextBox();
            this.btnRecipeModify = new MetroFramework.Controls.MetroButton();
            this.lbSelectedModel = new MetroFramework.Controls.MetroLabel();
            this.metroTile6 = new MetroFramework.Controls.MetroTile();
            this.btnRecipeSelect = new MetroFramework.Controls.MetroButton();
            this.btnRecipeDel = new MetroFramework.Controls.MetroButton();
            this.btnRecipeNew = new MetroFramework.Controls.MetroButton();
            this.lbLastUpdateTime = new MetroFramework.Controls.MetroLabel();
            this.metroTile2 = new MetroFramework.Controls.MetroTile();
            this.lbLotIdName = new MetroFramework.Controls.MetroLabel();
            this.metroTile1 = new MetroFramework.Controls.MetroTile();
            this.lbModelName = new MetroFramework.Controls.MetroLabel();
            this.metroTile11 = new MetroFramework.Controls.MetroTile();
            this.pnRecipeNew = new System.Windows.Forms.Panel();
            this.btnNewRecipeCancel = new MetroFramework.Controls.MetroButton();
            this.btnNewRecipeCreate = new MetroFramework.Controls.MetroButton();
            this.metroTile5 = new MetroFramework.Controls.MetroTile();
            this.cbRecipeRefList = new MetroFramework.Controls.MetroComboBox();
            this.metroTile4 = new MetroFramework.Controls.MetroTile();
            this.metroTile3 = new MetroFramework.Controls.MetroTile();
            this.tbRecipeNewName = new MetroFramework.Controls.MetroTextBox();
            this.pnRecipeBackupView = new System.Windows.Forms.Panel();
            this.btnBackupPath = new System.Windows.Forms.Button();
            this.btnOriginalPath = new System.Windows.Forms.Button();
            this.tbBackupPath = new System.Windows.Forms.TextBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.tbRecipePath = new System.Windows.Forms.TextBox();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.metroTile10 = new MetroFramework.Controls.MetroTile();
            this.tbRecipeModifyNo = new MetroFramework.Controls.MetroTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecipeList)).BeginInit();
            this.pnRecipeModify.SuspendLayout();
            this.pnRecipeNew.SuspendLayout();
            this.pnRecipeBackupView.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(20, 60);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.dgvRecipeList);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.pnRecipeModify);
            this.splitContainerMain.Panel2.Controls.Add(this.btnRecipeModify);
            this.splitContainerMain.Panel2.Controls.Add(this.lbSelectedModel);
            this.splitContainerMain.Panel2.Controls.Add(this.metroTile6);
            this.splitContainerMain.Panel2.Controls.Add(this.btnRecipeSelect);
            this.splitContainerMain.Panel2.Controls.Add(this.btnRecipeDel);
            this.splitContainerMain.Panel2.Controls.Add(this.btnRecipeNew);
            this.splitContainerMain.Panel2.Controls.Add(this.lbLastUpdateTime);
            this.splitContainerMain.Panel2.Controls.Add(this.metroTile2);
            this.splitContainerMain.Panel2.Controls.Add(this.lbLotIdName);
            this.splitContainerMain.Panel2.Controls.Add(this.metroTile1);
            this.splitContainerMain.Panel2.Controls.Add(this.lbModelName);
            this.splitContainerMain.Panel2.Controls.Add(this.metroTile11);
            this.splitContainerMain.Panel2.Controls.Add(this.pnRecipeNew);
            this.splitContainerMain.Panel2.Controls.Add(this.pnRecipeBackupView);
            this.splitContainerMain.Size = new System.Drawing.Size(984, 820);
            this.splitContainerMain.SplitterDistance = 474;
            this.splitContainerMain.SplitterWidth = 1;
            this.splitContainerMain.TabIndex = 1;
            // 
            // dgvRecipeList
            // 
            this.dgvRecipeList.AllowUserToAddRows = false;
            this.dgvRecipeList.AllowUserToDeleteRows = false;
            this.dgvRecipeList.AllowUserToResizeColumns = false;
            this.dgvRecipeList.AllowUserToResizeRows = false;
            this.dgvRecipeList.BackgroundColor = System.Drawing.Color.White;
            this.dgvRecipeList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Arial", 16F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.DeepSkyBlue;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRecipeList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvRecipeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecipeList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.Column4});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Arial", 16F);
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRecipeList.DefaultCellStyle = dataGridViewCellStyle11;
            this.dgvRecipeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecipeList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvRecipeList.EnableHeadersVisualStyles = false;
            this.dgvRecipeList.GridColor = System.Drawing.Color.Black;
            this.dgvRecipeList.Location = new System.Drawing.Point(0, 0);
            this.dgvRecipeList.MultiSelect = false;
            this.dgvRecipeList.Name = "dgvRecipeList";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Arial", 12F);
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRecipeList.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvRecipeList.RowHeadersVisible = false;
            this.dgvRecipeList.RowHeadersWidth = 62;
            this.dgvRecipeList.RowTemplate.Height = 30;
            this.dgvRecipeList.RowTemplate.ReadOnly = true;
            this.dgvRecipeList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvRecipeList.Size = new System.Drawing.Size(474, 820);
            this.dgvRecipeList.TabIndex = 103;
            this.dgvRecipeList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRecipeList_CellClick);
            this.dgvRecipeList.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvRecipeList_SortCompare_1);
            // 
            // ColumnName
            // 
            this.ColumnName.HeaderText = "모델";
            this.ColumnName.MinimumWidth = 8;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.Width = 250;
            // 
            // Column4
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle10;
            this.Column4.HeaderText = "LOT_ID";
            this.Column4.MinimumWidth = 8;
            this.Column4.Name = "Column4";
            this.Column4.Width = 225;
            // 
            // pnRecipeModify
            // 
            this.pnRecipeModify.BackgroundImage = global::IntelligentFactory.Properties.Resources.Background;
            this.pnRecipeModify.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnRecipeModify.Controls.Add(this.metroTile10);
            this.pnRecipeModify.Controls.Add(this.tbRecipeModifyNo);
            this.pnRecipeModify.Controls.Add(this.btnModifyRecipeCancel);
            this.pnRecipeModify.Controls.Add(this.btnModifyRecipeCreate);
            this.pnRecipeModify.Controls.Add(this.metroTile7);
            this.pnRecipeModify.Controls.Add(this.cbRecipeRefModifyList);
            this.pnRecipeModify.Controls.Add(this.metroTile8);
            this.pnRecipeModify.Controls.Add(this.metroTile9);
            this.pnRecipeModify.Controls.Add(this.tbRecipeModifyName);
            this.pnRecipeModify.Location = new System.Drawing.Point(3, 363);
            this.pnRecipeModify.Name = "pnRecipeModify";
            this.pnRecipeModify.Size = new System.Drawing.Size(505, 221);
            this.pnRecipeModify.TabIndex = 1028;
            this.pnRecipeModify.Visible = false;
            // 
            // btnModifyRecipeCancel
            // 
            this.btnModifyRecipeCancel.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnModifyRecipeCancel.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnModifyRecipeCancel.Highlight = true;
            this.btnModifyRecipeCancel.Location = new System.Drawing.Point(336, 180);
            this.btnModifyRecipeCancel.Name = "btnModifyRecipeCancel";
            this.btnModifyRecipeCancel.Size = new System.Drawing.Size(167, 35);
            this.btnModifyRecipeCancel.TabIndex = 1025;
            this.btnModifyRecipeCancel.Text = "취소";
            this.btnModifyRecipeCancel.UseSelectable = true;
            this.btnModifyRecipeCancel.Click += new System.EventHandler(this.btnModifyRecipeCancel_Click);
            // 
            // btnModifyRecipeCreate
            // 
            this.btnModifyRecipeCreate.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnModifyRecipeCreate.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnModifyRecipeCreate.Highlight = true;
            this.btnModifyRecipeCreate.Location = new System.Drawing.Point(167, 180);
            this.btnModifyRecipeCreate.Name = "btnModifyRecipeCreate";
            this.btnModifyRecipeCreate.Size = new System.Drawing.Size(168, 35);
            this.btnModifyRecipeCreate.TabIndex = 1024;
            this.btnModifyRecipeCreate.Text = "변경";
            this.btnModifyRecipeCreate.UseSelectable = true;
            this.btnModifyRecipeCreate.Click += new System.EventHandler(this.btnModifyRecipeCreate_Click);
            // 
            // metroTile7
            // 
            this.metroTile7.ActiveControl = null;
            this.metroTile7.BackColor = System.Drawing.Color.Transparent;
            this.metroTile7.Location = new System.Drawing.Point(-1, 3);
            this.metroTile7.Name = "metroTile7";
            this.metroTile7.Size = new System.Drawing.Size(505, 36);
            this.metroTile7.TabIndex = 1013;
            this.metroTile7.Text = "이름 변경";
            this.metroTile7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile7.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile7.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile7.UseSelectable = true;
            this.metroTile7.UseTileImage = true;
            // 
            // cbRecipeRefModifyList
            // 
            this.cbRecipeRefModifyList.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbRecipeRefModifyList.FormattingEnabled = true;
            this.cbRecipeRefModifyList.ItemHeight = 29;
            this.cbRecipeRefModifyList.Location = new System.Drawing.Point(181, 114);
            this.cbRecipeRefModifyList.Name = "cbRecipeRefModifyList";
            this.cbRecipeRefModifyList.Size = new System.Drawing.Size(324, 35);
            this.cbRecipeRefModifyList.TabIndex = 1012;
            this.cbRecipeRefModifyList.UseSelectable = true;
            // 
            // metroTile8
            // 
            this.metroTile8.ActiveControl = null;
            this.metroTile8.BackColor = System.Drawing.Color.Transparent;
            this.metroTile8.Location = new System.Drawing.Point(0, 114);
            this.metroTile8.Name = "metroTile8";
            this.metroTile8.Size = new System.Drawing.Size(180, 36);
            this.metroTile8.TabIndex = 1011;
            this.metroTile8.Text = "복사 대상";
            this.metroTile8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile8.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile8.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile8.UseSelectable = true;
            this.metroTile8.UseTileImage = true;
            // 
            // metroTile9
            // 
            this.metroTile9.ActiveControl = null;
            this.metroTile9.BackColor = System.Drawing.Color.Transparent;
            this.metroTile9.Location = new System.Drawing.Point(0, 77);
            this.metroTile9.Name = "metroTile9";
            this.metroTile9.Size = new System.Drawing.Size(180, 36);
            this.metroTile9.TabIndex = 1010;
            this.metroTile9.Text = "LOT_ID";
            this.metroTile9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile9.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile9.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile9.UseSelectable = true;
            this.metroTile9.UseTileImage = true;
            // 
            // tbRecipeModifyName
            // 
            // 
            // 
            // 
            this.tbRecipeModifyName.CustomButton.Image = null;
            this.tbRecipeModifyName.CustomButton.Location = new System.Drawing.Point(290, 2);
            this.tbRecipeModifyName.CustomButton.Name = "";
            this.tbRecipeModifyName.CustomButton.Size = new System.Drawing.Size(31, 31);
            this.tbRecipeModifyName.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbRecipeModifyName.CustomButton.TabIndex = 1;
            this.tbRecipeModifyName.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbRecipeModifyName.CustomButton.UseSelectable = true;
            this.tbRecipeModifyName.DisplayIcon = true;
            this.tbRecipeModifyName.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbRecipeModifyName.Lines = new string[0];
            this.tbRecipeModifyName.Location = new System.Drawing.Point(181, 77);
            this.tbRecipeModifyName.MaxLength = 32767;
            this.tbRecipeModifyName.Name = "tbRecipeModifyName";
            this.tbRecipeModifyName.PasswordChar = '\0';
            this.tbRecipeModifyName.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbRecipeModifyName.SelectedText = "";
            this.tbRecipeModifyName.SelectionLength = 0;
            this.tbRecipeModifyName.SelectionStart = 0;
            this.tbRecipeModifyName.ShortcutsEnabled = true;
            this.tbRecipeModifyName.ShowButton = true;
            this.tbRecipeModifyName.ShowClearButton = true;
            this.tbRecipeModifyName.Size = new System.Drawing.Size(324, 36);
            this.tbRecipeModifyName.TabIndex = 1009;
            this.tbRecipeModifyName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRecipeModifyName.UseSelectable = true;
            this.tbRecipeModifyName.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbRecipeModifyName.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // btnRecipeModify
            // 
            this.btnRecipeModify.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRecipeModify.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnRecipeModify.Highlight = true;
            this.btnRecipeModify.Location = new System.Drawing.Point(385, 168);
            this.btnRecipeModify.Name = "btnRecipeModify";
            this.btnRecipeModify.Size = new System.Drawing.Size(117, 35);
            this.btnRecipeModify.TabIndex = 1027;
            this.btnRecipeModify.Text = "이름 변경";
            this.btnRecipeModify.UseSelectable = true;
            this.btnRecipeModify.Click += new System.EventHandler(this.btnRecipeModify_Click);
            // 
            // lbSelectedModel
            // 
            this.lbSelectedModel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbSelectedModel.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbSelectedModel.Location = new System.Drawing.Point(180, 126);
            this.lbSelectedModel.Name = "lbSelectedModel";
            this.lbSelectedModel.Size = new System.Drawing.Size(324, 41);
            this.lbSelectedModel.TabIndex = 1026;
            this.lbSelectedModel.Text = "-";
            this.lbSelectedModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbSelectedModel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbSelectedModel.UseStyleColors = true;
            // 
            // metroTile6
            // 
            this.metroTile6.ActiveControl = null;
            this.metroTile6.BackColor = System.Drawing.Color.Transparent;
            this.metroTile6.Location = new System.Drawing.Point(-1, 126);
            this.metroTile6.Name = "metroTile6";
            this.metroTile6.Size = new System.Drawing.Size(180, 41);
            this.metroTile6.TabIndex = 1025;
            this.metroTile6.Text = "선택 모델";
            this.metroTile6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile6.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile6.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile6.UseSelectable = true;
            this.metroTile6.UseTileImage = true;
            // 
            // btnRecipeSelect
            // 
            this.btnRecipeSelect.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRecipeSelect.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnRecipeSelect.Highlight = true;
            this.btnRecipeSelect.Location = new System.Drawing.Point(250, 168);
            this.btnRecipeSelect.Name = "btnRecipeSelect";
            this.btnRecipeSelect.Size = new System.Drawing.Size(131, 35);
            this.btnRecipeSelect.TabIndex = 1024;
            this.btnRecipeSelect.Text = "선택";
            this.btnRecipeSelect.UseSelectable = true;
            this.btnRecipeSelect.Click += new System.EventHandler(this.btnRecipeSelect_Click);
            // 
            // btnRecipeDel
            // 
            this.btnRecipeDel.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRecipeDel.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnRecipeDel.Highlight = true;
            this.btnRecipeDel.Location = new System.Drawing.Point(124, 168);
            this.btnRecipeDel.Name = "btnRecipeDel";
            this.btnRecipeDel.Size = new System.Drawing.Size(124, 35);
            this.btnRecipeDel.TabIndex = 1023;
            this.btnRecipeDel.Text = "삭제";
            this.btnRecipeDel.UseSelectable = true;
            this.btnRecipeDel.Click += new System.EventHandler(this.btnRecipeDel_Click);
            // 
            // btnRecipeNew
            // 
            this.btnRecipeNew.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnRecipeNew.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnRecipeNew.Highlight = true;
            this.btnRecipeNew.Location = new System.Drawing.Point(-1, 168);
            this.btnRecipeNew.Name = "btnRecipeNew";
            this.btnRecipeNew.Size = new System.Drawing.Size(124, 35);
            this.btnRecipeNew.TabIndex = 1022;
            this.btnRecipeNew.Text = "추가";
            this.btnRecipeNew.UseSelectable = true;
            this.btnRecipeNew.Click += new System.EventHandler(this.btnRecipeNew_Click);
            // 
            // lbLastUpdateTime
            // 
            this.lbLastUpdateTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbLastUpdateTime.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbLastUpdateTime.Location = new System.Drawing.Point(180, 84);
            this.lbLastUpdateTime.Name = "lbLastUpdateTime";
            this.lbLastUpdateTime.Size = new System.Drawing.Size(324, 41);
            this.lbLastUpdateTime.TabIndex = 897;
            this.lbLastUpdateTime.Text = "-";
            this.lbLastUpdateTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbLastUpdateTime.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbLastUpdateTime.UseStyleColors = true;
            // 
            // metroTile2
            // 
            this.metroTile2.ActiveControl = null;
            this.metroTile2.BackColor = System.Drawing.Color.Transparent;
            this.metroTile2.Location = new System.Drawing.Point(-1, 84);
            this.metroTile2.Name = "metroTile2";
            this.metroTile2.Size = new System.Drawing.Size(180, 41);
            this.metroTile2.TabIndex = 896;
            this.metroTile2.Text = "최중 수정일시";
            this.metroTile2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile2.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile2.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile2.UseSelectable = true;
            this.metroTile2.UseTileImage = true;
            // 
            // lbLotIdName
            // 
            this.lbLotIdName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbLotIdName.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbLotIdName.Location = new System.Drawing.Point(180, 42);
            this.lbLotIdName.Name = "lbLotIdName";
            this.lbLotIdName.Size = new System.Drawing.Size(324, 41);
            this.lbLotIdName.TabIndex = 895;
            this.lbLotIdName.Text = "-";
            this.lbLotIdName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbLotIdName.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbLotIdName.UseStyleColors = true;
            // 
            // metroTile1
            // 
            this.metroTile1.ActiveControl = null;
            this.metroTile1.BackColor = System.Drawing.Color.Transparent;
            this.metroTile1.Location = new System.Drawing.Point(-1, 42);
            this.metroTile1.Name = "metroTile1";
            this.metroTile1.Size = new System.Drawing.Size(180, 41);
            this.metroTile1.TabIndex = 894;
            this.metroTile1.Text = "LOT ID";
            this.metroTile1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile1.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile1.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile1.UseSelectable = true;
            this.metroTile1.UseTileImage = true;
            // 
            // lbModelName
            // 
            this.lbModelName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbModelName.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.lbModelName.Location = new System.Drawing.Point(180, 0);
            this.lbModelName.Name = "lbModelName";
            this.lbModelName.Size = new System.Drawing.Size(324, 41);
            this.lbModelName.TabIndex = 893;
            this.lbModelName.Text = "-";
            this.lbModelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbModelName.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbModelName.UseStyleColors = true;
            // 
            // metroTile11
            // 
            this.metroTile11.ActiveControl = null;
            this.metroTile11.BackColor = System.Drawing.Color.Transparent;
            this.metroTile11.Location = new System.Drawing.Point(-1, 0);
            this.metroTile11.Name = "metroTile11";
            this.metroTile11.Size = new System.Drawing.Size(180, 41);
            this.metroTile11.TabIndex = 892;
            this.metroTile11.Text = "모델";
            this.metroTile11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile11.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile11.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile11.UseSelectable = true;
            this.metroTile11.UseTileImage = true;
            // 
            // pnRecipeNew
            // 
            this.pnRecipeNew.BackgroundImage = global::IntelligentFactory.Properties.Resources.Background;
            this.pnRecipeNew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnRecipeNew.Controls.Add(this.btnNewRecipeCancel);
            this.pnRecipeNew.Controls.Add(this.btnNewRecipeCreate);
            this.pnRecipeNew.Controls.Add(this.metroTile5);
            this.pnRecipeNew.Controls.Add(this.cbRecipeRefList);
            this.pnRecipeNew.Controls.Add(this.metroTile4);
            this.pnRecipeNew.Controls.Add(this.metroTile3);
            this.pnRecipeNew.Controls.Add(this.tbRecipeNewName);
            this.pnRecipeNew.Location = new System.Drawing.Point(-1, 206);
            this.pnRecipeNew.Name = "pnRecipeNew";
            this.pnRecipeNew.Size = new System.Drawing.Size(505, 155);
            this.pnRecipeNew.TabIndex = 0;
            this.pnRecipeNew.Visible = false;
            // 
            // btnNewRecipeCancel
            // 
            this.btnNewRecipeCancel.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnNewRecipeCancel.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnNewRecipeCancel.Highlight = true;
            this.btnNewRecipeCancel.Location = new System.Drawing.Point(337, 116);
            this.btnNewRecipeCancel.Name = "btnNewRecipeCancel";
            this.btnNewRecipeCancel.Size = new System.Drawing.Size(167, 35);
            this.btnNewRecipeCancel.TabIndex = 1025;
            this.btnNewRecipeCancel.Text = "취소";
            this.btnNewRecipeCancel.UseSelectable = true;
            this.btnNewRecipeCancel.Click += new System.EventHandler(this.btnNewRecipeCancel_Click);
            // 
            // btnNewRecipeCreate
            // 
            this.btnNewRecipeCreate.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnNewRecipeCreate.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.btnNewRecipeCreate.Highlight = true;
            this.btnNewRecipeCreate.Location = new System.Drawing.Point(168, 116);
            this.btnNewRecipeCreate.Name = "btnNewRecipeCreate";
            this.btnNewRecipeCreate.Size = new System.Drawing.Size(168, 35);
            this.btnNewRecipeCreate.TabIndex = 1024;
            this.btnNewRecipeCreate.Text = "추가";
            this.btnNewRecipeCreate.UseSelectable = true;
            this.btnNewRecipeCreate.Click += new System.EventHandler(this.btnNewRecipeCreate_Click);
            // 
            // metroTile5
            // 
            this.metroTile5.ActiveControl = null;
            this.metroTile5.BackColor = System.Drawing.Color.Transparent;
            this.metroTile5.Location = new System.Drawing.Point(-1, 3);
            this.metroTile5.Name = "metroTile5";
            this.metroTile5.Size = new System.Drawing.Size(505, 36);
            this.metroTile5.TabIndex = 1013;
            this.metroTile5.Text = "모델 추가";
            this.metroTile5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile5.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile5.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Bold;
            this.metroTile5.UseSelectable = true;
            this.metroTile5.UseTileImage = true;
            // 
            // cbRecipeRefList
            // 
            this.cbRecipeRefList.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.cbRecipeRefList.FormattingEnabled = true;
            this.cbRecipeRefList.ItemHeight = 29;
            this.cbRecipeRefList.Location = new System.Drawing.Point(180, 79);
            this.cbRecipeRefList.Name = "cbRecipeRefList";
            this.cbRecipeRefList.Size = new System.Drawing.Size(324, 35);
            this.cbRecipeRefList.TabIndex = 1012;
            this.cbRecipeRefList.UseSelectable = true;
            // 
            // metroTile4
            // 
            this.metroTile4.ActiveControl = null;
            this.metroTile4.BackColor = System.Drawing.Color.Transparent;
            this.metroTile4.Location = new System.Drawing.Point(-1, 78);
            this.metroTile4.Name = "metroTile4";
            this.metroTile4.Size = new System.Drawing.Size(180, 36);
            this.metroTile4.TabIndex = 1011;
            this.metroTile4.Text = "복사 대상";
            this.metroTile4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile4.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile4.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile4.UseSelectable = true;
            this.metroTile4.UseTileImage = true;
            // 
            // metroTile3
            // 
            this.metroTile3.ActiveControl = null;
            this.metroTile3.BackColor = System.Drawing.Color.Transparent;
            this.metroTile3.Location = new System.Drawing.Point(-1, 41);
            this.metroTile3.Name = "metroTile3";
            this.metroTile3.Size = new System.Drawing.Size(180, 36);
            this.metroTile3.TabIndex = 1010;
            this.metroTile3.Text = "LOT_ID";
            this.metroTile3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile3.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile3.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile3.UseSelectable = true;
            this.metroTile3.UseTileImage = true;
            // 
            // tbRecipeNewName
            // 
            // 
            // 
            // 
            this.tbRecipeNewName.CustomButton.Image = null;
            this.tbRecipeNewName.CustomButton.Location = new System.Drawing.Point(290, 2);
            this.tbRecipeNewName.CustomButton.Name = "";
            this.tbRecipeNewName.CustomButton.Size = new System.Drawing.Size(31, 31);
            this.tbRecipeNewName.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbRecipeNewName.CustomButton.TabIndex = 1;
            this.tbRecipeNewName.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbRecipeNewName.CustomButton.UseSelectable = true;
            this.tbRecipeNewName.DisplayIcon = true;
            this.tbRecipeNewName.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbRecipeNewName.Lines = new string[0];
            this.tbRecipeNewName.Location = new System.Drawing.Point(180, 41);
            this.tbRecipeNewName.MaxLength = 32767;
            this.tbRecipeNewName.Name = "tbRecipeNewName";
            this.tbRecipeNewName.PasswordChar = '\0';
            this.tbRecipeNewName.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbRecipeNewName.SelectedText = "";
            this.tbRecipeNewName.SelectionLength = 0;
            this.tbRecipeNewName.SelectionStart = 0;
            this.tbRecipeNewName.ShortcutsEnabled = true;
            this.tbRecipeNewName.ShowButton = true;
            this.tbRecipeNewName.ShowClearButton = true;
            this.tbRecipeNewName.Size = new System.Drawing.Size(324, 36);
            this.tbRecipeNewName.TabIndex = 1009;
            this.tbRecipeNewName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRecipeNewName.UseSelectable = true;
            this.tbRecipeNewName.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbRecipeNewName.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbRecipeNewName.Click += new System.EventHandler(this.tbRecipeNewName_Click);
            // 
            // pnRecipeBackupView
            // 
            this.pnRecipeBackupView.BackgroundImage = global::IntelligentFactory.Properties.Resources.Background;
            this.pnRecipeBackupView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnRecipeBackupView.Controls.Add(this.btnBackupPath);
            this.pnRecipeBackupView.Controls.Add(this.btnOriginalPath);
            this.pnRecipeBackupView.Controls.Add(this.tbBackupPath);
            this.pnRecipeBackupView.Controls.Add(this.panel7);
            this.pnRecipeBackupView.Controls.Add(this.tbRecipePath);
            this.pnRecipeBackupView.Controls.Add(this.panel9);
            this.pnRecipeBackupView.Controls.Add(this.panel10);
            this.pnRecipeBackupView.Location = new System.Drawing.Point(276, 843);
            this.pnRecipeBackupView.Name = "pnRecipeBackupView";
            this.pnRecipeBackupView.Size = new System.Drawing.Size(691, 143);
            this.pnRecipeBackupView.TabIndex = 1;
            this.pnRecipeBackupView.Visible = false;
            // 
            // btnBackupPath
            // 
            this.btnBackupPath.BackColor = System.Drawing.Color.Black;
            this.btnBackupPath.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBackupPath.BackgroundImage")));
            this.btnBackupPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBackupPath.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnBackupPath.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Aquamarine;
            this.btnBackupPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackupPath.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackupPath.ForeColor = System.Drawing.Color.White;
            this.btnBackupPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBackupPath.Location = new System.Drawing.Point(596, 88);
            this.btnBackupPath.Name = "btnBackupPath";
            this.btnBackupPath.Size = new System.Drawing.Size(85, 44);
            this.btnBackupPath.TabIndex = 28;
            this.btnBackupPath.Text = "Backup Path";
            this.btnBackupPath.UseVisualStyleBackColor = false;
            this.btnBackupPath.Click += new System.EventHandler(this.btnBackupPath_Click);
            // 
            // btnOriginalPath
            // 
            this.btnOriginalPath.BackColor = System.Drawing.Color.Black;
            this.btnOriginalPath.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnOriginalPath.BackgroundImage")));
            this.btnOriginalPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOriginalPath.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnOriginalPath.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Aquamarine;
            this.btnOriginalPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOriginalPath.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOriginalPath.ForeColor = System.Drawing.Color.White;
            this.btnOriginalPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOriginalPath.Location = new System.Drawing.Point(596, 42);
            this.btnOriginalPath.Name = "btnOriginalPath";
            this.btnOriginalPath.Size = new System.Drawing.Size(85, 44);
            this.btnOriginalPath.TabIndex = 27;
            this.btnOriginalPath.Text = "Original Path";
            this.btnOriginalPath.UseVisualStyleBackColor = false;
            this.btnOriginalPath.Click += new System.EventHandler(this.btnOriginalPath_Click);
            // 
            // tbBackupPath
            // 
            this.tbBackupPath.BackColor = System.Drawing.Color.Black;
            this.tbBackupPath.Font = new System.Drawing.Font("Arial", 24F);
            this.tbBackupPath.ForeColor = System.Drawing.Color.Aquamarine;
            this.tbBackupPath.Location = new System.Drawing.Point(141, 88);
            this.tbBackupPath.Name = "tbBackupPath";
            this.tbBackupPath.Size = new System.Drawing.Size(453, 44);
            this.tbBackupPath.TabIndex = 25;
            this.tbBackupPath.Text = "D:\\Recipe_Backup";
            // 
            // panel7
            // 
            this.panel7.BackgroundImage = global::IntelligentFactory.Properties.Resources.DefaultBackground;
            this.panel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel7.Controls.Add(this.label5);
            this.panel7.Location = new System.Drawing.Point(4, 88);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(135, 45);
            this.panel7.TabIndex = 24;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 45);
            this.label5.TabIndex = 0;
            this.label5.Text = "Backup Path";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbRecipePath
            // 
            this.tbRecipePath.BackColor = System.Drawing.Color.Black;
            this.tbRecipePath.Font = new System.Drawing.Font("Arial", 24F);
            this.tbRecipePath.ForeColor = System.Drawing.Color.Aquamarine;
            this.tbRecipePath.Location = new System.Drawing.Point(141, 42);
            this.tbRecipePath.Name = "tbRecipePath";
            this.tbRecipePath.Size = new System.Drawing.Size(453, 44);
            this.tbRecipePath.TabIndex = 8;
            this.tbRecipePath.Text = "C:\\IntelligentAlign\\Recipe";
            // 
            // panel9
            // 
            this.panel9.BackgroundImage = global::IntelligentFactory.Properties.Resources.DefaultBackground;
            this.panel9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel9.Controls.Add(this.label7);
            this.panel9.Location = new System.Drawing.Point(4, 41);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(135, 45);
            this.panel9.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 45);
            this.label7.TabIndex = 0;
            this.label7.Text = "Recipe Path";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel10
            // 
            this.panel10.BackgroundImage = global::IntelligentFactory.Properties.Resources.DefaultBackground;
            this.panel10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel10.Controls.Add(this.label8);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(689, 35);
            this.panel10.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.Aquamarine;
            this.label8.Location = new System.Drawing.Point(0, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(689, 35);
            this.label8.TabIndex = 1;
            this.label8.Text = "Recipe Backup";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroTile10
            // 
            this.metroTile10.ActiveControl = null;
            this.metroTile10.BackColor = System.Drawing.Color.Transparent;
            this.metroTile10.Location = new System.Drawing.Point(0, 40);
            this.metroTile10.Name = "metroTile10";
            this.metroTile10.Size = new System.Drawing.Size(180, 36);
            this.metroTile10.TabIndex = 1027;
            this.metroTile10.Text = "모델 No";
            this.metroTile10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroTile10.TileImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.metroTile10.TileTextFontWeight = MetroFramework.MetroTileTextWeight.Regular;
            this.metroTile10.UseSelectable = true;
            this.metroTile10.UseTileImage = true;
            // 
            // tbRecipeModifyNo
            // 
            // 
            // 
            // 
            this.tbRecipeModifyNo.CustomButton.Image = null;
            this.tbRecipeModifyNo.CustomButton.Location = new System.Drawing.Point(290, 2);
            this.tbRecipeModifyNo.CustomButton.Name = "";
            this.tbRecipeModifyNo.CustomButton.Size = new System.Drawing.Size(31, 31);
            this.tbRecipeModifyNo.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbRecipeModifyNo.CustomButton.TabIndex = 1;
            this.tbRecipeModifyNo.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbRecipeModifyNo.CustomButton.UseSelectable = true;
            this.tbRecipeModifyNo.DisplayIcon = true;
            this.tbRecipeModifyNo.FontSize = MetroFramework.MetroTextBoxSize.Tall;
            this.tbRecipeModifyNo.Lines = new string[0];
            this.tbRecipeModifyNo.Location = new System.Drawing.Point(181, 40);
            this.tbRecipeModifyNo.MaxLength = 2;
            this.tbRecipeModifyNo.Name = "tbRecipeModifyNo";
            this.tbRecipeModifyNo.PasswordChar = '\0';
            this.tbRecipeModifyNo.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbRecipeModifyNo.SelectedText = "";
            this.tbRecipeModifyNo.SelectionLength = 0;
            this.tbRecipeModifyNo.SelectionStart = 0;
            this.tbRecipeModifyNo.ShortcutsEnabled = true;
            this.tbRecipeModifyNo.ShowButton = true;
            this.tbRecipeModifyNo.ShowClearButton = true;
            this.tbRecipeModifyNo.Size = new System.Drawing.Size(324, 36);
            this.tbRecipeModifyNo.TabIndex = 1026;
            this.tbRecipeModifyNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbRecipeModifyNo.UseSelectable = true;
            this.tbRecipeModifyNo.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbRecipeModifyNo.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbRecipeModifyNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // FormRecipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 900);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "FormRecipe";
            this.Resizable = false;
            this.Text = "모델";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.FormRecipe_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecipeList)).EndInit();
            this.pnRecipeModify.ResumeLayout(false);
            this.pnRecipeNew.ResumeLayout(false);
            this.pnRecipeBackupView.ResumeLayout(false);
            this.pnRecipeBackupView.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel pnRecipeNew;
        private System.Windows.Forms.Panel pnRecipeBackupView;
        private System.Windows.Forms.TextBox tbBackupPath;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbRecipePath;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnBackupPath;
        private System.Windows.Forms.Button btnOriginalPath;
        private MetroFramework.Controls.MetroLabel lbLastUpdateTime;
        private MetroFramework.Controls.MetroTile metroTile2;
        private MetroFramework.Controls.MetroLabel lbLotIdName;
        private MetroFramework.Controls.MetroTile metroTile1;
        private MetroFramework.Controls.MetroLabel lbModelName;
        private MetroFramework.Controls.MetroTile metroTile11;
        private MetroFramework.Controls.MetroButton btnRecipeSelect;
        private MetroFramework.Controls.MetroButton btnRecipeDel;
        private MetroFramework.Controls.MetroButton btnRecipeNew;
        private MetroFramework.Controls.MetroTile metroTile4;
        private MetroFramework.Controls.MetroTile metroTile3;
        private MetroFramework.Controls.MetroTextBox tbRecipeNewName;
        private System.Windows.Forms.DataGridView dgvRecipeList;
        private MetroFramework.Controls.MetroButton btnNewRecipeCancel;
        private MetroFramework.Controls.MetroButton btnNewRecipeCreate;
        private MetroFramework.Controls.MetroTile metroTile5;
        private MetroFramework.Controls.MetroComboBox cbRecipeRefList;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private MetroFramework.Controls.MetroLabel lbSelectedModel;
        private MetroFramework.Controls.MetroTile metroTile6;
        private MetroFramework.Controls.MetroButton btnRecipeModify;
        private System.Windows.Forms.Panel pnRecipeModify;
        private MetroFramework.Controls.MetroButton btnModifyRecipeCancel;
        private MetroFramework.Controls.MetroButton btnModifyRecipeCreate;
        private MetroFramework.Controls.MetroTile metroTile7;
        private MetroFramework.Controls.MetroComboBox cbRecipeRefModifyList;
        private MetroFramework.Controls.MetroTile metroTile8;
        private MetroFramework.Controls.MetroTile metroTile9;
        private MetroFramework.Controls.MetroTextBox tbRecipeModifyName;
        private MetroFramework.Controls.MetroTile metroTile10;
        private MetroFramework.Controls.MetroTextBox tbRecipeModifyNo;
    }
}