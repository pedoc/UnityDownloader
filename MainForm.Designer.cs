namespace UnityDownloader
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            groupControl1 = new DevExpress.XtraEditors.GroupControl();
            txtSelector = new DevExpress.XtraEditors.TextEdit();
            labelControl5 = new DevExpress.XtraEditors.LabelControl();
            txtSaveDirectory = new DevExpress.XtraEditors.TextEdit();
            labelControl4 = new DevExpress.XtraEditors.LabelControl();
            btnDownloadEditor = new DevExpress.XtraEditors.SimpleButton();
            btnDownloadEditorJson = new DevExpress.XtraEditors.SimpleButton();
            txtDownloadCount = new DevExpress.XtraEditors.TextEdit();
            txtEditorJson = new DevExpress.XtraEditors.TextEdit();
            txtProxy = new DevExpress.XtraEditors.TextEdit();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
            gridControl = new DevExpress.XtraGrid.GridControl();
            gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            memTxt = new DevExpress.XtraEditors.MemoEdit();
            btnOpenDirectory = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)groupControl1).BeginInit();
            groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtSelector.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSaveDirectory.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtDownloadCount.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtEditorJson.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtProxy.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel1).BeginInit();
            splitContainerControl2.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel2).BeginInit();
            splitContainerControl2.Panel2.SuspendLayout();
            splitContainerControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)memTxt.Properties).BeginInit();
            SuspendLayout();
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = DockStyle.Fill;
            splitContainerControl1.Horizontal = false;
            splitContainerControl1.Location = new Point(0, 0);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(groupControl1);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(splitContainerControl2);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new Size(1364, 968);
            splitContainerControl1.SplitterPosition = 183;
            splitContainerControl1.TabIndex = 0;
            // 
            // groupControl1
            // 
            groupControl1.Controls.Add(btnOpenDirectory);
            groupControl1.Controls.Add(txtSelector);
            groupControl1.Controls.Add(labelControl5);
            groupControl1.Controls.Add(txtSaveDirectory);
            groupControl1.Controls.Add(labelControl4);
            groupControl1.Controls.Add(btnDownloadEditor);
            groupControl1.Controls.Add(btnDownloadEditorJson);
            groupControl1.Controls.Add(txtDownloadCount);
            groupControl1.Controls.Add(txtEditorJson);
            groupControl1.Controls.Add(txtProxy);
            groupControl1.Controls.Add(labelControl3);
            groupControl1.Controls.Add(labelControl2);
            groupControl1.Controls.Add(labelControl1);
            groupControl1.Dock = DockStyle.Fill;
            groupControl1.Location = new Point(0, 0);
            groupControl1.Name = "groupControl1";
            groupControl1.Size = new Size(1364, 183);
            groupControl1.TabIndex = 0;
            groupControl1.Text = "选项";
            // 
            // txtSelector
            // 
            txtSelector.EditValue = "body > div.flex.min-h-screen.flex-col > main > div.relative.flex.flex-wrap.justify-center.gap-2.p-2 > button";
            txtSelector.Location = new Point(727, 81);
            txtSelector.Name = "txtSelector";
            txtSelector.Size = new Size(495, 20);
            txtSelector.TabIndex = 11;
            // 
            // labelControl5
            // 
            labelControl5.Location = new Point(652, 84);
            labelControl5.Name = "labelControl5";
            labelControl5.Size = new Size(36, 14);
            labelControl5.TabIndex = 10;
            labelControl5.Text = "选择器";
            // 
            // txtSaveDirectory
            // 
            txtSaveDirectory.EditValue = "K:\\Unity";
            txtSaveDirectory.Location = new Point(121, 113);
            txtSaveDirectory.Name = "txtSaveDirectory";
            txtSaveDirectory.Size = new Size(495, 20);
            txtSaveDirectory.TabIndex = 9;
            // 
            // labelControl4
            // 
            labelControl4.Location = new Point(55, 116);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new Size(48, 14);
            labelControl4.TabIndex = 8;
            labelControl4.Text = "存储目录";
            // 
            // btnDownloadEditor
            // 
            btnDownloadEditor.Location = new Point(242, 143);
            btnDownloadEditor.Name = "btnDownloadEditor";
            btnDownloadEditor.Size = new Size(75, 23);
            btnDownloadEditor.TabIndex = 7;
            btnDownloadEditor.Text = "下载选中";
            btnDownloadEditor.Click += btnDownloadEditor_Click;
            // 
            // btnDownloadEditorJson
            // 
            btnDownloadEditorJson.Location = new Point(121, 143);
            btnDownloadEditorJson.Name = "btnDownloadEditorJson";
            btnDownloadEditorJson.Size = new Size(75, 23);
            btnDownloadEditorJson.TabIndex = 6;
            btnDownloadEditorJson.Text = "更新资源";
            btnDownloadEditorJson.Click += btnDownloadEditorJson_Click;
            // 
            // txtDownloadCount
            // 
            txtDownloadCount.EditValue = "4";
            txtDownloadCount.Location = new Point(403, 48);
            txtDownloadCount.Name = "txtDownloadCount";
            txtDownloadCount.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            txtDownloadCount.Properties.MaskSettings.Set("mask", "d");
            txtDownloadCount.Size = new Size(214, 20);
            txtDownloadCount.TabIndex = 5;
            // 
            // txtEditorJson
            // 
            txtEditorJson.EditValue = "https://unity3d.com/get-unity/download/archive";
            txtEditorJson.Location = new Point(122, 81);
            txtEditorJson.Name = "txtEditorJson";
            txtEditorJson.Size = new Size(495, 20);
            txtEditorJson.TabIndex = 4;
            // 
            // txtProxy
            // 
            txtProxy.EditValue = "socks5://127.0.0.1:10808";
            txtProxy.Location = new Point(121, 48);
            txtProxy.Name = "txtProxy";
            txtProxy.Size = new Size(196, 20);
            txtProxy.TabIndex = 3;
            // 
            // labelControl3
            // 
            labelControl3.Location = new Point(349, 51);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new Size(48, 14);
            labelControl3.TabIndex = 2;
            labelControl3.Text = "下载线程";
            // 
            // labelControl2
            // 
            labelControl2.Location = new Point(56, 84);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new Size(60, 14);
            labelControl2.TabIndex = 1;
            labelControl2.Text = "编辑器资源";
            // 
            // labelControl1
            // 
            labelControl1.Location = new Point(57, 51);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new Size(48, 14);
            labelControl1.TabIndex = 0;
            labelControl1.Text = "代理地址";
            // 
            // splitContainerControl2
            // 
            splitContainerControl2.Dock = DockStyle.Fill;
            splitContainerControl2.Location = new Point(0, 0);
            splitContainerControl2.Name = "splitContainerControl2";
            // 
            // splitContainerControl2.Panel1
            // 
            splitContainerControl2.Panel1.Controls.Add(gridControl);
            splitContainerControl2.Panel1.Text = "Panel1";
            // 
            // splitContainerControl2.Panel2
            // 
            splitContainerControl2.Panel2.Controls.Add(memTxt);
            splitContainerControl2.Panel2.Text = "Panel2";
            splitContainerControl2.Size = new Size(1364, 775);
            splitContainerControl2.SplitterPosition = 1121;
            splitContainerControl2.TabIndex = 0;
            // 
            // gridControl
            // 
            gridControl.Dock = DockStyle.Fill;
            gridControl.Location = new Point(0, 0);
            gridControl.MainView = gridView;
            gridControl.Name = "gridControl";
            gridControl.Size = new Size(1121, 775);
            gridControl.TabIndex = 0;
            gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });
            // 
            // gridView
            // 
            gridView.GridControl = gridControl;
            gridView.Name = "gridView";
            // 
            // memTxt
            // 
            memTxt.Dock = DockStyle.Fill;
            memTxt.Location = new Point(0, 0);
            memTxt.Name = "memTxt";
            memTxt.Size = new Size(233, 775);
            memTxt.TabIndex = 0;
            // 
            // btnOpenDirectory
            // 
            btnOpenDirectory.Location = new Point(365, 143);
            btnOpenDirectory.Name = "btnOpenDirectory";
            btnOpenDirectory.Size = new Size(75, 23);
            btnOpenDirectory.TabIndex = 12;
            btnOpenDirectory.Text = "打开目录";
            btnOpenDirectory.Click += btnOpenDirectory_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1364, 968);
            Controls.Add(splitContainerControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Icon = (Icon)resources.GetObject("MainForm.IconOptions.Icon");
            Name = "MainForm";
            Text = "Unity资源下载";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)groupControl1).EndInit();
            groupControl1.ResumeLayout(false);
            groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtSelector.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSaveDirectory.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtDownloadCount.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtEditorJson.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtProxy.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel1).EndInit();
            splitContainerControl2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2.Panel2).EndInit();
            splitContainerControl2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl2).EndInit();
            splitContainerControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)memTxt.Properties).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl2;
        private DevExpress.XtraEditors.MemoEdit memTxt;
        private DevExpress.XtraEditors.TextEdit txtDownloadCount;
        private DevExpress.XtraEditors.TextEdit txtEditorJson;
        private DevExpress.XtraEditors.TextEdit txtProxy;
        private DevExpress.XtraEditors.SimpleButton btnDownloadEditor;
        private DevExpress.XtraEditors.SimpleButton btnDownloadEditorJson;
        private DevExpress.XtraEditors.TextEdit txtSaveDirectory;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraEditors.TextEdit txtSelector;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.SimpleButton btnOpenDirectory;
    }
}