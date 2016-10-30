namespace Getequ.B4Refactoring.Windows
{
    partial class SelectElementsForm
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
            this.clbViewModel = new System.Windows.Forms.CheckedListBox();
            this.clbService = new System.Windows.Forms.CheckedListBox();
            this.btnVM = new System.Windows.Forms.Button();
            this.btnSvc = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.codePanel = new System.Windows.Forms.Panel();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnDeselect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // clbViewModel
            // 
            this.clbViewModel.FormattingEnabled = true;
            this.clbViewModel.Location = new System.Drawing.Point(12, 30);
            this.clbViewModel.Name = "clbViewModel";
            this.clbViewModel.Size = new System.Drawing.Size(339, 169);
            this.clbViewModel.TabIndex = 0;
            this.clbViewModel.SelectedIndexChanged += new System.EventHandler(this.clbViewModel_SelectedIndexChanged);
            // 
            // clbService
            // 
            this.clbService.FormattingEnabled = true;
            this.clbService.Location = new System.Drawing.Point(438, 30);
            this.clbService.Name = "clbService";
            this.clbService.Size = new System.Drawing.Size(343, 169);
            this.clbService.TabIndex = 1;
            this.clbService.SelectedIndexChanged += new System.EventHandler(this.clbService_SelectedIndexChanged);
            // 
            // btnVM
            // 
            this.btnVM.Location = new System.Drawing.Point(357, 75);
            this.btnVM.Name = "btnVM";
            this.btnVM.Size = new System.Drawing.Size(75, 23);
            this.btnVM.TabIndex = 2;
            this.btnVM.Text = "<";
            this.btnVM.UseVisualStyleBackColor = true;
            this.btnVM.Click += new System.EventHandler(this.btnVM_Click);
            // 
            // btnSvc
            // 
            this.btnSvc.Location = new System.Drawing.Point(357, 104);
            this.btnSvc.Name = "btnSvc";
            this.btnSvc.Size = new System.Drawing.Size(75, 23);
            this.btnSvc.TabIndex = 3;
            this.btnSvc.Text = ">";
            this.btnSvc.UseVisualStyleBackColor = true;
            this.btnSvc.Click += new System.EventHandler(this.btnSvc_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(787, 176);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ок";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "ViewModel";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(441, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Service";
            // 
            // codePanel
            // 
            this.codePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.codePanel.Location = new System.Drawing.Point(12, 205);
            this.codePanel.Name = "codePanel";
            this.codePanel.Size = new System.Drawing.Size(850, 433);
            this.codePanel.TabIndex = 7;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(788, 30);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 46);
            this.btnSelect.TabIndex = 8;
            this.btnSelect.Text = "Select all";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDeselect
            // 
            this.btnDeselect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeselect.Location = new System.Drawing.Point(787, 82);
            this.btnDeselect.Name = "btnDeselect";
            this.btnDeselect.Size = new System.Drawing.Size(75, 45);
            this.btnDeselect.TabIndex = 9;
            this.btnDeselect.Text = "Deselect all";
            this.btnDeselect.UseVisualStyleBackColor = true;
            this.btnDeselect.Click += new System.EventHandler(this.btnDeselect_Click);
            // 
            // SelectElementsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 650);
            this.Controls.Add(this.btnDeselect);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.codePanel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnSvc);
            this.Controls.Add(this.btnVM);
            this.Controls.Add(this.clbService);
            this.Controls.Add(this.clbViewModel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectElementsForm";
            this.Text = "Select methods to refactor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbViewModel;
        private System.Windows.Forms.CheckedListBox clbService;
        private System.Windows.Forms.Button btnVM;
        private System.Windows.Forms.Button btnSvc;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel codePanel;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnDeselect;
    }
}