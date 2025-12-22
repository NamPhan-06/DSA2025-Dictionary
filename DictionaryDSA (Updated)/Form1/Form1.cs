using System;
using System.Drawing;
using System.Windows.Forms;

namespace EnglishVietnameseDictionary
{
    public partial class Form1 : Form
    {
        private DictionaryManager _dictManager;
        private DictionaryBasic _dictBasic;
        private DictionaryAlgo _dictAlgo;

        public Form1()
        {
            InitializeComponent();
            _dictManager = new DictionaryManager();
            _dictBasic = new DictionaryBasic(_dictManager);
            _dictAlgo = new DictionaryAlgo(_dictManager);
            _dictBasic.SetAlgo(_dictAlgo);
            _dictAlgo.SetBasic(_dictBasic);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _dictBasic.ImportData(_dictManager._filePath);
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

            // 1. Xử lý khi ô trống
            if (string.IsNullOrEmpty(keyword) || keyword == "Nhập từ vựng...")
            {
                UpdateListBox(_dictManager.KeyList);
                txtResult.Text = "";
                lblStatus.Text = "Sẵn sàng";
                return;
            }

            // 2. CHẠY THUẬT TOÁN 5 (Tìm đúng/Tiền tố trước)
            List<string> results = _dictAlgo.GetSuggestionsManual(keyword);

            // 3. KIỂM TRA KẾT QUẢ
            if (results.Count > 0)
            {
                // Nếu tìm thấy từ bắt đầu bằng 'keyword' -> Hiển thị bình thường
                UpdateListBox(results);
                lblStatus.Text = $"Tìm thấy {results.Count} từ.";

                // Hiện nghĩa của từ đầu tiên nếu khớp hoàn toàn
                int index = _dictAlgo.BinarySearchKey(keyword);
                if (index != -1) txtResult.Text = _dictBasic.GetMeaning(keyword);
                else txtResult.Text = "";
            }
            else
            {
                // 4. NẾU KHÔNG TÌM THẤY -> KÍCH HOẠT AUTOCORRECT (Thuật toán 6)
                // Đây là lúc bạn gõ "aple" và code nhảy vào đây
                List<string> autoCorrectResults = _dictAlgo.SearchAutocorrect(keyword);

                if (autoCorrectResults.Count > 0)
                {
                    UpdateListBox(autoCorrectResults); // Hiển thị 'apple' lên ListBox

                    // Thông báo cho người dùng biết
                    lblStatus.Text = "Không tìm thấy chính xác. Gợi ý các từ gần đúng:";
                    lblStatus.ForeColor = Color.Red;

                    // Tự động chọn từ đầu tiên apple
                    txtResult.Text = _dictBasic.GetMeaning(autoCorrectResults[0]);
                }
                else
                {
                    // Trường hợp bó tay (sai quá nhiều)
                    UpdateListBox(null);
                    lblStatus.Text = "Không tìm thấy từ nào.";
                    lblStatus.ForeColor = Color.Black;
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
                string meaning = _dictBasic.GetMeaning(selectedWord);

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
                MessageBox.Show("Vui lòng nhập từ và nghĩa vào ô.", "Thông báo");
                return; 
            }

            if (!string.IsNullOrEmpty(newWord) && !string.IsNullOrEmpty(newMeaning))
            {
                if (_dictBasic.AddWord(newWord, newMeaning))
                {
                    MessageBox.Show("Thêm thành công!", "Thông báo");
                    UpdateListBox(_dictManager.KeyList); 
                }
                else
                {
                    MessageBox.Show("Từ này đã tồn tại!", "Thông báo");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập từ và nghĩa vào ô.", "Thông báo");
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
                    int addedCount = _dictBasic.ImportData(dialog.FileName);

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
                MessageBox.Show("Vui lòng chọn từ cần xóa.", "Thông báo");
                return; 
            }

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa từ '{word}' không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                if (_dictBasic.RemoveWord(word))
                {
                    MessageBox.Show("Đã xóa thành công!", "Thông báo");

                    txtSearch.Text = "Nhập từ vựng...";
                    txtSearch.ForeColor = Color.Gray;
                    txtResult.Text = "";

                    UpdateListBox(_dictManager.KeyList);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy từ này để xóa.", "Thông báo");
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

            if (string.IsNullOrEmpty(newMeaning))
            {
                MessageBox.Show("Vui lòng nhập nghĩa mới.");
                return;
            }

            string currentDef = _dictBasic.GetMeaning(word);

            // Lưu ý: Hàm GetMeaning có thể trả về câu thông báo lỗi nếu không tìm thấy
            // Nên ta cần đảm bảo từ đó thực sự tồn tại trước khi so sánh
            if (currentDef == "Chưa có nghĩa trong từ điển" || currentDef == "Vui lòng nhập từ.")
            {
                MessageBox.Show("Từ này chưa có trong từ điển. Hãy dùng nút 'Thêm Từ'.");
                return;
            }

            if (string.Equals(currentDef, newMeaning, StringComparison.OrdinalIgnoreCase))
            {
                // Nếu giống hệt nhau -> Báo lỗi và Dừng lại
                MessageBox.Show($"Nghĩa '{newMeaning}' trùng khớp với nghĩa hiện tại.\nVui lòng thay đổi nghĩa trước khi bấm Sửa.",
                                "Cảnh báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return; // Dòng này quan trọng: Thoát khỏi hàm ngay lập tức
            }

            // Gọi hàm sửa bên DictionaryManager
            if (_dictBasic.UpdateMeaning(word, newMeaning))
            {
                MessageBox.Show("Đã cập nhật nghĩa mới!");
                // Không cần UpdateListBox vì danh sách từ (Key) không thay đổi
            }
            else
            {
                MessageBox.Show("Từ này chưa có trong từ điển. Hãy dùng nút 'Thêm Từ'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận cho chắc ăn (tránh bấm nhầm)
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa toàn bộ dữ liệu từ điển không?",
                "Cảnh báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (_dictManager.KeyList.Count == 0)
            {
                MessageBox.Show("Không thể xóa do dữ liệu hiện đang trống!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return; // Quan trọng: Dừng hàm ngay tại đây, không chạy tiếp đoạn dưới
            }

            if (result == DialogResult.Yes)
            {
                // 1. Gọi hàm dọn dẹp ở tầng dữ liệu
                _dictBasic.ClearAllData();

                // 2. Cập nhật giao diện (Xóa trắng ListBox và các ô nhập)
                UpdateListBox(null); // Hoặc truyền vào list rỗng
                txtResult.Text = "";
                txtSearch.Text = "";
                lblStatus.Text = "Dữ liệu đã được xóa sạch.";

                MessageBox.Show("Đã xóa thành công!", "Thông báo");
            }
        }
    }
}