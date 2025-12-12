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
            btnAdd.Location = new Point(801, 67);
            btnAdd.Margin = new Padding(4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(133, 51);
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
            lstWords.Location = new Point(13, 169);
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
            txtSearch.Location = new Point(290, 80);
            txtSearch.Margin = new Padding(4);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(477, 25);
            txtSearch.TabIndex = 2;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // txtResult
            // 
            txtResult.BackColor = Color.White;
            txtResult.BorderStyle = BorderStyle.None;
            txtResult.Location = new Point(280, 215);
            txtResult.Margin = new Padding(4);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.Size = new Size(654, 280);
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
            panel1.Controls.Add(btnAdd);
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
            label2.Location = new Point(113, 80);
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
            label1.Location = new Point(349, 20);
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
            label3.Location = new Point(280, 169);
            label3.Name = "label3";
            label3.Size = new Size(126, 25);
            label3.TabIndex = 7;
            label3.Text = "Nghĩa của từ";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.AliceBlue;
            ClientSize = new Size(1000, 562);
            Controls.Add(label3);
            Controls.Add(lstWords);
            Controls.Add(lblStatus);
            Controls.Add(panel1);
            Controls.Add(txtResult);
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
    }
}
