using System;
using System.Drawing;
using System.Windows.Forms;

namespace EnglishVietnameseDictionary
{
    public partial class Form1 : Form
    {
        private DictionaryManager _dictManager;

        public Form1()
        {
            InitializeComponent();
            _dictManager = new DictionaryManager();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _dictManager.LoadData();
                UpdateListBox(_dictManager.KeyList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }

            lblStatus.BringToFront();

            // Trong Form1_Load
            txtSearch.Text = "Nhập từ vựng...";
            txtSearch.ForeColor = Color.Gray;

            // Gắn sự kiện (Enter và Leave)
            txtSearch.Enter += (s, e) => {
                if (txtSearch.Text == "Nhập từ vựng...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };

            txtSearch.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Nhập từ vựng...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
        }

        private void UpdateListBox(System.Collections.Generic.List<string> source)
        {
            // Tắt sự kiện để ListBox không báo linh tinh khi đang nạp dữ liệu
            lstWords.SelectedIndexChanged -= lstWords_SelectedIndexChanged;

            // Nạp dữ liệu mới
            lstWords.DataSource = null;
            lstWords.DataSource = source;

            // Quan trọng - Bỏ chọn tất cả (để không tự chọn dòng đầu tiên)
            lstWords.SelectedIndex = -1;

            // Bật lại sự kiện
            lstWords.SelectedIndexChanged += lstWords_SelectedIndexChanged;
        }

        // Xử lý sự kiện khi gõ phím (Real-time Search)
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(keyword) || keyword == "Nhập từ vựng...")
            {
                UpdateListBox(_dictManager.KeyList);
                txtResult.Text = "";
                return;
            }

            // Gọi thuật toán Binary Prefix Search
            // Đảm bảo danh sách đã được sort trước khi chạy
            List<string> searchResult = _dictManager.FilterListAdvanced(keyword);

            UpdateListBox(searchResult);

            if (searchResult.Count > 0)
            {
                lblStatus.Text = $"Tìm thấy {searchResult.Count} từ";
                txtResult.Text = _dictManager.GetMeaning(searchResult[0]);
            }
            else
            {
                lblStatus.Text = "Không tìm thấy từ nào.";
                txtResult.Text = "";
            }
        }

        // Sự kiện khi click vào một từ trong ListBox
        private void lstWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstWords.SelectedItem != null)
            {
                string selectedWord = lstWords.SelectedItem.ToString();

                // Sử dụng Dictionary để lấy nghĩa (O(1)) thay vì duyệt List Values
                string meaning = _dictManager.GetMeaning(selectedWord);

                txtResult.Text = meaning;

                // Tránh trigger sự kiện TextChanged lặp lại vô tận
                txtSearch.TextChanged -= txtSearch_TextChanged;
                txtSearch.Text = selectedWord;
                txtSearch.TextChanged += txtSearch_TextChanged;
            }
        }

        // Button thêm từ mới
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newWord = txtSearch.Text.Trim();
            string newMeaning = txtResult.Text.Trim();

            if (string.IsNullOrEmpty(newWord) || newWord == "Nhập từ vựng...")
            {
                MessageBox.Show("Vui lòng nhập từ và nghĩa vào ô.");
                return; 
            }

            if (!string.IsNullOrEmpty(newWord) && !string.IsNullOrEmpty(newMeaning))
            {
                if (_dictManager.AddWord(newWord, newMeaning))
                {
                    MessageBox.Show("Thêm thành công!");
                    UpdateListBox(_dictManager.KeyList); 
                }
                else
                {
                    MessageBox.Show("Từ này đã tồn tại!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập từ và nghĩa vào ô.");
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Gọi hàm và lấy số lượng từ đã thêm
                    int addedCount = _dictManager.ImportData(dialog.FileName);

                    UpdateListBox(_dictManager.KeyList);

                    if (addedCount > 0)
                    {
                        MessageBox.Show($"Đã nạp thành công {addedCount} từ mới!", "Kết quả");
                    }
                    else
                    {
                        MessageBox.Show("Không có từ mới nào được thêm vào.\n(Tất cả từ trong file đều đã tồn tại trong từ điển)", "Kết quả");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string word = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(word) || word == "Nhập từ vựng...")
            {
                MessageBox.Show("Vui lòng chọn từ cần xóa.");
                return; 
            }

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa từ '{word}' không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                if (_dictManager.RemoveWord(word))
                {
                    MessageBox.Show("Đã xóa thành công!");

                    txtSearch.Text = "Nhập từ vựng...";
                    txtSearch.ForeColor = Color.Gray;
                    txtResult.Text = "";

                    UpdateListBox(_dictManager.KeyList);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy từ này để xóa.");
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Lấy từ khóa (Key) ở ô tìm kiếm
            string word = txtSearch.Text.Trim();
            // Lấy nghĩa mới (Value) người dùng vừa sửa ở ô kết quả
            string newMeaning = txtResult.Text.Trim();

            if (string.IsNullOrEmpty(word) || word == "Nhập từ vựng...")
            {
                MessageBox.Show("Vui lòng chọn từ cần sửa.");
                return;
            }

            // Gọi hàm sửa bên DictionaryManager
            if (_dictManager.UpdateMeaning(word, newMeaning))
            {
                MessageBox.Show("Đã cập nhật nghĩa mới!");
                // Không cần UpdateListBox vì danh sách từ (Key) không thay đổi
            }
            else
            {
                MessageBox.Show("Từ này chưa có trong từ điển. Hãy dùng nút 'Thêm Từ'.");
            }
        }
    }
}