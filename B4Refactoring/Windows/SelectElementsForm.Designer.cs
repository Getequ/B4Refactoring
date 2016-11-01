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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.showDiff = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSvc = new System.Windows.Forms.Button();
            this.btnVM = new System.Windows.Forms.Button();
            this.clbViewModel = new System.Windows.Forms.CheckedListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnDeselect = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.clbService = new System.Windows.Forms.CheckedListBox();
            this.clearPanel = new System.Windows.Forms.Panel();
            this.codePanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.35294F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.64706F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.clearPanel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.codePanel, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(898, 626);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.showDiff);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnSvc);
            this.panel1.Controls.Add(this.btnVM);
            this.panel1.Controls.Add(this.clbViewModel);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(446, 214);
            this.panel1.TabIndex = 12;
            // 
            // showDiff
            // 
            this.showDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.showDiff.AutoSize = true;
            this.showDiff.Location = new System.Drawing.Point(368, 189);
            this.showDiff.Name = "showDiff";
            this.showDiff.Size = new System.Drawing.Size(40, 17);
            this.showDiff.TabIndex = 10;
            this.showDiff.Text = "diff";
            this.showDiff.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "ViewModel";
            // 
            // btnSvc
            // 
            this.btnSvc.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSvc.Location = new System.Drawing.Point(368, 121);
            this.btnSvc.Name = "btnSvc";
            this.btnSvc.Size = new System.Drawing.Size(75, 23);
            this.btnSvc.TabIndex = 8;
            this.btnSvc.Text = ">";
            this.btnSvc.UseVisualStyleBackColor = true;
            this.btnSvc.Click += new System.EventHandler(this.btnSvc_Click);
            // 
            // btnVM
            // 
            this.btnVM.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnVM.Location = new System.Drawing.Point(368, 92);
            this.btnVM.Name = "btnVM";
            this.btnVM.Size = new System.Drawing.Size(75, 23);
            this.btnVM.TabIndex = 7;
            this.btnVM.Text = "<";
            this.btnVM.UseVisualStyleBackColor = true;
            this.btnVM.Click += new System.EventHandler(this.btnVM_Click);
            // 
            // clbViewModel
            // 
            this.clbViewModel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbViewModel.FormattingEnabled = true;
            this.clbViewModel.Location = new System.Drawing.Point(3, 24);
            this.clbViewModel.Name = "clbViewModel";
            this.clbViewModel.Size = new System.Drawing.Size(353, 184);
            this.clbViewModel.TabIndex = 6;
            this.clbViewModel.SelectedIndexChanged += new System.EventHandler(this.clbViewModel_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.btnDeselect);
            this.panel2.Controls.Add(this.btnSelect);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnOk);
            this.panel2.Controls.Add(this.clbService);
            this.panel2.Location = new System.Drawing.Point(455, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(440, 214);
            this.panel2.TabIndex = 14;
            // 
            // btnDeselect
            // 
            this.btnDeselect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeselect.Location = new System.Drawing.Point(362, 76);
            this.btnDeselect.Name = "btnDeselect";
            this.btnDeselect.Size = new System.Drawing.Size(75, 45);
            this.btnDeselect.TabIndex = 14;
            this.btnDeselect.Text = "Deselect all";
            this.btnDeselect.UseVisualStyleBackColor = true;
            this.btnDeselect.Click += new System.EventHandler(this.btnDeselect_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(362, 27);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 46);
            this.btnSelect.TabIndex = 13;
            this.btnSelect.Text = "Select all";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Service";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(362, 185);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "Ок";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // clbService
            // 
            this.clbService.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbService.FormattingEnabled = true;
            this.clbService.Location = new System.Drawing.Point(3, 24);
            this.clbService.Name = "clbService";
            this.clbService.Size = new System.Drawing.Size(353, 184);
            this.clbService.TabIndex = 10;
            this.clbService.SelectedIndexChanged += new System.EventHandler(this.clbService_SelectedIndexChanged);
            // 
            // clearPanel
            // 
            this.clearPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clearPanel.Location = new System.Drawing.Point(455, 223);
            this.clearPanel.Name = "clearPanel";
            this.clearPanel.Size = new System.Drawing.Size(440, 400);
            this.clearPanel.TabIndex = 15;
            // 
            // codePanel
            // 
            this.codePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.codePanel.Location = new System.Drawing.Point(3, 223);
            this.codePanel.Name = "codePanel";
            this.codePanel.Size = new System.Drawing.Size(446, 400);
            this.codePanel.TabIndex = 13;
            // 
            // SelectElementsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 650);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimizeBox = false;
            this.Name = "SelectElementsForm";
            this.Text = "Select methods to refactor";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel codePanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSvc;
        private System.Windows.Forms.Button btnVM;
        private System.Windows.Forms.CheckedListBox clbViewModel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnDeselect;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckedListBox clbService;
        private System.Windows.Forms.Panel clearPanel;
        private System.Windows.Forms.CheckBox showDiff;

    }
}