namespace EnglishVietnameseDictionary
{
    partial class Form1
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
            btnAdd = new Button();
            lstWords = new ListBox();
            txtSearch = new TextBox();
            txtResult = new TextBox();
            lblStatus = new Label();
            panel1 = new Panel();
            label2 = new Label();
            label1 = new Label();
            label3 = new Label();
            btnImport = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnReset = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.White;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnAdd.ForeColor = Color.Black;
            btnAdd.Location = new Point(845, 210);
            btnAdd.Margin = new Padding(4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(125, 42);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "THÊM TỪ";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // 
            // lstWords
            // 
            lstWords.BackColor = Color.White;
            lstWords.BorderStyle = BorderStyle.None;
            lstWords.FormattingEnabled = true;
            lstWords.Location = new Point(30, 169);
            lstWords.Margin = new Padding(4);
            lstWords.Name = "lstWords";
            lstWords.Size = new Size(208, 350);
            lstWords.TabIndex = 1;
            lstWords.SelectedIndexChanged += lstWords_SelectedIndexChanged;
            // 
            // txtSearch
            // 
            txtSearch.BackColor = SystemColors.Window;
            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.Location = new Point(299, 78);
            txtSearch.Margin = new Padding(4);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(549, 25);
            txtSearch.TabIndex = 2;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // txtResult
            // 
            txtResult.BackColor = Color.White;
            txtResult.BorderStyle = BorderStyle.None;
            txtResult.Location = new Point(299, 210);
            txtResult.Margin = new Padding(4);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.Size = new Size(469, 309);
            txtResult.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.BackColor = Color.White;
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Location = new Point(0, 537);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(88, 25);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Sẵn sàng";
            // 
            // panel1
            // 
            panel1.BackColor = Color.DodgerBlue;
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(txtSearch);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1000, 150);
            panel1.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.DodgerBlue;
            label2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label2.ForeColor = Color.White;
            label2.Location = new Point(113, 78);
            label2.Name = "label2";
            label2.Size = new Size(160, 25);
            label2.TabIndex = 6;
            label2.Text = "Nhập từ cần tìm:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.DodgerBlue;
            label1.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(363, 21);
            label1.Name = "label1";
            label1.Size = new Size(275, 37);
            label1.TabIndex = 0;
            label1.Text = "TỪ ĐIỂN ANH - VIỆT";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.White;
            label3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label3.ForeColor = Color.Black;
            label3.Location = new Point(299, 169);
            label3.Name = "label3";
            label3.Size = new Size(126, 25);
            label3.TabIndex = 7;
            label3.Text = "Nghĩa của từ";
            // 
            // btnImport
            // 
            btnImport.BackColor = Color.White;
            btnImport.FlatAppearance.BorderSize = 0;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnImport.ForeColor = Color.Black;
            btnImport.Location = new Point(845, 277);
            btnImport.Margin = new Padding(4);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(125, 42);
            btnImport.TabIndex = 7;
            btnImport.Text = "NẠP TỪ";
            btnImport.UseVisualStyleBackColor = false;
            btnImport.Click += btnImport_Click;
            // 
            // btnEdit
            // 
            btnEdit.BackColor = Color.White;
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnEdit.ForeColor = Color.Black;
            btnEdit.Location = new Point(845, 342);
            btnEdit.Margin = new Padding(4);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(125, 42);
            btnEdit.TabIndex = 8;
            btnEdit.Text = "SỬA TỪ";
            btnEdit.UseVisualStyleBackColor = false;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.White;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnDelete.ForeColor = Color.Black;
            btnDelete.Location = new Point(845, 409);
            btnDelete.Margin = new Padding(4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(125, 42);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "XÓA TỪ";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnReset
            // 
            btnReset.BackColor = Color.White;
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnReset.ForeColor = Color.Black;
            btnReset.Location = new Point(845, 477);
            btnReset.Margin = new Padding(4);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(125, 42);
            btnReset.TabIndex = 10;
            btnReset.Text = "RESET";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += btnReset_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.AliceBlue;
            ClientSize = new Size(1000, 562);
            Controls.Add(btnReset);
            Controls.Add(btnDelete);
            Controls.Add(label3);
            Controls.Add(btnEdit);
            Controls.Add(lstWords);
            Controls.Add(btnImport);
            Controls.Add(lblStatus);
            Controls.Add(panel1);
            Controls.Add(txtResult);
            Controls.Add(btnAdd);
            Font = new Font("Segoe UI", 11F);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Từ điển Anh - Việt v1.0";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnAdd;
        private ListBox lstWords;
        private TextBox txtSearch;
        private TextBox txtResult;
        private Label lblStatus;
        private Panel panel1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btnImport;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnReset;
    }
}
