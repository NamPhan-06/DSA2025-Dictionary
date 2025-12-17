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
            // BƯỚC 1: Tắt sự kiện để ListBox không báo linh tinh khi đang nạp dữ liệu
            lstWords.SelectedIndexChanged -= lstWords_SelectedIndexChanged;

            // BƯỚC 2: Nạp dữ liệu mới
            lstWords.DataSource = null;
            lstWords.DataSource = source;

            // BƯỚC 3: Quan trọng - Bỏ chọn tất cả (để không tự chọn dòng đầu tiên)
            lstWords.SelectedIndex = -1;

            // BƯỚC 4: Bật lại sự kiện
            lstWords.SelectedIndexChanged += lstWords_SelectedIndexChanged;
        }

        // Xử lý sự kiện khi gõ phím (Real-time Search)
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(keyword) || keyword == "Nhập từ vựng...")
            {
                UpdateListBox(_dictManager.KeyList); // Hiển thị lại toàn bộ nếu rỗng
                txtResult.Text = "";
                lblStatus.Text = "Sẵn sàng";
                return;
            }

            // 1. Thử tìm kiếm chính xác bằng Binary Search trên List Keys
            int index = _dictManager.BinarySearchKey(keyword);

            if (index >= 0)
            {
                // Tìm thấy chính xác
                lstWords.SelectedIndex = -1; // Bỏ chọn cũ
                // Tự động cuộn đến từ đó nhưng không cần lọc lại ListBox để người dùng thấy các từ xung quanh
                lstWords.SelectedIndex = lstWords.FindString(keyword);

                string meaning = _dictManager.GetMeaning(keyword);
                txtResult.Text = meaning;
                lblStatus.Text = "Tìm thấy chính xác!";
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                // 2. Không tìm thấy chính xác -> Lọc theo Prefix (Gợi ý)
                var suggestions = _dictManager.GetSuggestions(keyword);
                if (suggestions.Count > 0)
                {
                    UpdateListBox(suggestions);
                    lblStatus.Text = $"Gợi ý: {suggestions.Count} từ.";
                    lblStatus.ForeColor = Color.Blue;
                    txtResult.Text = "";
                }
                else
                {
                    // 3. Không có Prefix -> Dùng thuật toán Levenshtein (Fuzzy)
                    var fuzzyMatches = _dictManager.FuzzySearch(keyword);
                    if (fuzzyMatches.Count > 0)
                    {
                        UpdateListBox(fuzzyMatches);
                        lblStatus.Text = "Có phải ý bạn là...?";
                        lblStatus.ForeColor = Color.OrangeRed;
                    }
                    else
                    {
                        UpdateListBox(null);
                        lblStatus.Text = "Không tìm thấy từ nào.";
                        lblStatus.ForeColor = Color.Red;
                    }
                    txtResult.Text = "";
                }
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
            // Giả sử bạn có 1 form con hoặc nhập liệu từ 2 textbox khác
            // Ở đây tôi demo lấy từ chính ô tìm kiếm và ô kết quả để làm ví dụ nhanh
            string newWord = txtSearch.Text.Trim();
            string newMeaning = txtResult.Text.Trim();

            if (string.IsNullOrEmpty(newWord) || newWord == "Nhập từ vựng...")
            {
                MessageBox.Show("Vui lòng nhập từ và nghĩa vào ô.");
                return; // Dừng lại ngay, không làm gì tiếp theo
            }

            if (!string.IsNullOrEmpty(newWord) && !string.IsNullOrEmpty(newMeaning))
            {
                if (_dictManager.AddWord(newWord, newMeaning))
                {
                    MessageBox.Show("Thêm thành công!");
                    UpdateListBox(_dictManager.KeyList); // Refresh lại list
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

                    // Cập nhật lại giao diện
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

            // SỬA Ở ĐÂY: Chặn nếu ô trống HOẶC đang hiện chữ placeholder
            if (string.IsNullOrEmpty(word) || word == "Nhập từ vựng...")
            {
                MessageBox.Show("Vui lòng chọn từ cần xóa.");
                return; // Dừng lại ngay, không làm gì tiếp theo
            }

            // Phần code cũ giữ nguyên bên dưới
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

                    // Xử lý sau khi xóa: Đặt lại placeholder
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