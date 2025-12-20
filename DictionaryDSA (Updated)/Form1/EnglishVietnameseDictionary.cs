using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EnglishVietnameseDictionary
{
    public class DictionaryManager
    {
        // CẤU TRÚC DỮ LIỆU
        public Dictionary<string, string> DataDict { get; private set; }
        public List<string> KeyList { get; private set; }   // List cho Keys
        public List<string> ValueList { get; private set; } // List cho Values (ít dùng hơn nếu đã có Dict, nhưng vẫn tạo theo yêu cầu)


        private string _filePath = "data.txt";

        public DictionaryManager()
        {
            DataDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            KeyList = new List<string>();
            ValueList = new List<string>();
        }

        // Hàm nạp dữ liệu từ một đường dẫn file cụ thể
        // Kiểu trả về là int để đếm số từ thêm được
        public int ImportData(string path)
        {
            if (!File.Exists(path)) return 0;

            int countNewWords = 0; // Biến đếm số từ mới

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(new char[] { ',' }, 2);

                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim().Trim('"');
                        string value = parts[1].Trim().Trim('"');

                        // Kiểm tra trùng: Chỉ thêm nếu chưa có
                        if (!DataDict.ContainsKey(key))
                        {
                            DataDict.Add(key, value);
                            countNewWords++; // Tăng biến đếm lên 1
                        }
                        // Nếu trùng thì bỏ qua
                    }
                }
            }

            // Nếu có từ mới thì mới cần sắp xếp lại danh sách
            if (countNewWords > 0)
            {
                SyncListsFromDict();
            }

            return countNewWords; // Trả về kết quả
        }

        // Thêm từ mới
        public bool AddWord(string en, string vn)
        {
            if (!DataDict.ContainsKey(en))
            {
                DataDict.Add(en, vn);
                SyncListsFromDict(); // Cập nhật lại List Keys và Values
                SaveToFile();
                return true;
            }
            return false;
        }

        // Hàm Xóa từ
        public bool RemoveWord(string key)
        {
            // Kiểm tra xem từ có tồn tại không
            if (DataDict.ContainsKey(key))
            {
                DataDict.Remove(key); // Xóa khỏi Dictionary
                SyncListsFromDict();  // Cập nhật lại 2 List (quan trọng)
                SaveToFile();         // Lưu lại file ngay lập tức
                return true;
            }
            return false;
        }

        // Hàm Sửa nghĩa (Chỉ sửa nghĩa tiếng Việt)
        public bool UpdateMeaning(string key, string newMeaning)
        {
            if (DataDict.ContainsKey(key))
            {
                DataDict[key] = newMeaning; // Gán nghĩa mới
                                            // Không cần sort lại vì Key không đổi, nhưng cần cập nhật List Values
                SyncListsFromDict();
                SaveToFile();
                return true;
            }
            return false;
        }

        public void ClearAllData()
        {
            // 1. Xóa sạch dữ liệu trong Dictionary (O(n))
            DataDict.Clear();

            // 2. BẮT BUỘC: Xóa sạch cả các List đi kèm
            KeyList.Clear();

            if (ValueList != null)
            {
                ValueList.Clear();
            }
        }
        public string GetMeaning(string key)
        {
            // Kiểm tra đầu vào
            if (string.IsNullOrEmpty(key)) return "Vui lòng nhập từ.";

            // Kiểm tra trong kho dữ liệu
            if (DataDict.ContainsKey(key))
            {
                return DataDict[key];
            }
            else
            {
                return "Chưa có nghĩa trong từ điển";
            }
        }
        // Lưu file
        private void SaveToFile()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in DataDict)
            {
                sb.AppendLine($"{kvp.Key},{kvp.Value}");
            }
            File.WriteAllText(_filePath, sb.ToString());
        }

        // =========================================================
        // THUẬT TOÁN 1: Stream Processing (Đọc file dữ liệu)
        // Kỹ thuật đọc file tối ưu bộ nhớ
        // =========================================================
        public void LoadData()
        {
            // Đổi tên file thành đuôi .csv
            _filePath = "data.csv";

            if (!File.Exists(_filePath)) InitSampleData();

            DataDict.Clear();
            KeyList.Clear();
            ValueList.Clear();

            using (StreamReader sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // QUAN TRỌNG: Cắt chuỗi bằng dấu phẩy, tối đa 2 phần
                    var parts = line.Split(new char[] { ',' }, 2);

                    if (parts.Length == 2)
                    {
                        // Trim chả 2 đầu và xóa cả dấu ngoặc kép " nếu có (do Excel hay tự thêm vào)
                        string key = parts[0].Trim().Trim('"');
                        string value = parts[1].Trim().Trim('"');

                        if (!DataDict.ContainsKey(key))
                        {
                            DataDict.Add(key, value);
                        }
                    }
                }
            }

            SyncListsFromDict();
        }
        // =========================================================
        // THUẬT TOÁN 2: QuickSort Thủ Công (Custom QuickSort)
        // Thay thế hoàn toàn List.Sort() mặc định
        // Độ phức tạp trung bình: O(n log n)
        // =========================================================
        // Hàm chính để gọi từ bên ngoài
        public void SortData()
        {
            if (KeyList.Count > 1)
            {
                QuickSort(KeyList, 0, KeyList.Count - 1);
            }
        }

        // Hàm đệ quy (Chia để trị)
        private void QuickSort(List<string> list, int low, int high)
        {
            if (low < high)
            {
                // pi là chỉ số phân hoạch (partitioning index)
                // Sau lệnh này, list[pi] đã nằm đúng vị trí
                int pi = Partition(list, low, high);

                // Sắp xếp đệ quy các phần tử trước phân hoạch
                QuickSort(list, low, pi - 1);

                // Sắp xếp đệ quy các phần tử sau phân hoạch
                QuickSort(list, pi + 1, high);
            }
        }

        // Hàm phân đoạn (Partition) - Trái tim của QuickSort
        // Nhiệm vụ: Chọn chốt (pivot), đưa nhỏ hơn chốt sang trái, lớn hơn sang phải
        private int Partition(List<string> list, int low, int high)
        {
            // Chọn phần tử cuối cùng làm chốt (Pivot)
            string pivot = list[high];

            // Chỉ số của phần tử nhỏ hơn chốt
            int i = (low - 1);

            for (int j = low; j < high; j++)
            {
                // So sánh chuỗi (Không phân biệt hoa thường)
                // Result <= 0 nghĩa là list[j] nhỏ hơn hoặc bằng pivot
                if (String.Compare(list[j], pivot, StringComparison.OrdinalIgnoreCase) <= 0)
                {
                    i++;
                    // Đổi chỗ list[i] và list[j]
                    Swap(list, i, j);
                }
            }

            // Đổi chỗ phần tử chốt vào đúng vị trí của nó (giữa 2 phần)
            Swap(list, i + 1, high);

            return i + 1;
        }

        // Hàm phụ trợ để đổi chỗ 2 phần tử trong List
        private void Swap(List<string> list, int indexA, int indexB)
        {
            string temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }

        // Đồng bộ lại List từ Dictionary (Reset)
        private void SyncListsFromDict()
        {
            KeyList = DataDict.Keys.ToList();
            ValueList = DataDict.Values.ToList();
            SortData(); // Luôn sắp xếp sau khi load
        }

        // =========================================================
        // THUẬT TOÁN 3: Binary Search Thủ Công (Custom Binary Search)
        // Tìm kiếm nhị phân: O(log n) - Nhanh hơn rất nhiều so với duyệt tuần tự
        // ĐIỀU KIỆN BẮT BUỘC: Danh sách KeyList phải được SẮP XẾP trước
        // =========================================================
        public int BinarySearchKey(string keyword)
        {
            // Khởi tạo 2 mốc giới hạn: Đầu và Cuối danh sách
            int left = 0;
            int right = KeyList.Count - 1;

            // Chạy vòng lặp khi khoảng cách tìm kiếm vẫn còn hợp lệ
            while (left <= right)
            {
                // Tính vị trí ở giữa (Mid)
                // Công thức này tránh tràn số (overflow) tốt hơn (left + right) / 2
                int mid = left + (right - left) / 2;

                // Lấy từ ở vị trí giữa ra để so sánh
                string midWord = KeyList[mid];

                // So sánh từ ở giữa với từ khóa (keyword)
                // Sử dụng StringComparison.OrdinalIgnoreCase để không phân biệt hoa thường
                int comparison = String.Compare(midWord, keyword, StringComparison.OrdinalIgnoreCase);

                // TRƯỜNG HỢP 1: Tìm thấy chính xác! (0)
                if (comparison == 0)
                {
                    return mid; // Trả về vị trí tìm thấy
                }

                // TRƯỜNG HỢP 2: Từ ở giữa nhỏ hơn từ khóa (-1)
                // Ví dụ: mid="apple", key="banana" -> Phải tìm ở nửa bên PHẢI
                if (comparison < 0)
                {
                    left = mid + 1; // Dời mốc trái lên trên mid
                }
                // TRƯỜNG HỢP 3: Từ ở giữa lớn hơn từ khóa (1)
                // Ví dụ: mid="zoo", key="apple" -> Phải tìm ở nửa bên TRÁI
                else
                {
                    right = mid - 1; // Dời mốc phải xuống dưới mid
                }
            }

            // Nếu chạy hết vòng lặp mà không thấy -> Trả về -1
            return -1;
        }

        // =========================================================
        // THUẬT TOÁN 4: Bubble Sort (A-Z) - Full List
        // Mục đích: Sắp xếp toàn bộ danh sách tăng dần
        // Kỹ thuật: Duyệt trọn vẹn 2 vòng lặp (Naive Approach)
        // Độ phức tạp: O(N^2) -> Rất nặng nếu list dài
        // =========================================================
        public void BubbleSort()
        {
            int n = KeyList.Count; // Lấy toàn bộ số lượng từ

            // VÒNG LẶP 1: Duyệt qua từng phần tử
            for (int i = 0; i < n - 1; i++)
            {
                // VÒNG LẶP 2: So sánh cặp đôi liền kề
                for (int j = 0; j < n - i - 1; j++)
                {
                    // So sánh: Nếu từ đứng trước (j) LỚN HƠN từ đứng sau (j+1)
                    // (Ví dụ: "Zoo" > "Apple") -> Thì đổi chỗ
                    if (string.Compare(KeyList[j], KeyList[j + 1], StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        // Hoán đổi (Swap)
                        string temp = KeyList[j];
                        KeyList[j] = KeyList[j + 1];
                        KeyList[j + 1] = temp;
                    }
                }
            }
        }

        // =========================================================
        // THUẬT TOÁN 5: Lọc gợi ý thủ công (Thay thế LINQ)
        // Chức năng: Tìm tất cả các từ bắt đầu bằng 'keyword'
        // Cơ chế: Duyệt tuần tự từng từ (giống hệt cách LINQ hoạt động ngầm bên dưới)
        // =========================================================
        public List<string> GetSuggestionsManual(string keyword)
        {
            // 1. Tạo một danh sách trống để chứa kết quả
            List<string> suggestions = new List<string>();

            // Nếu từ khóa rỗng thì trả về danh sách rỗng (hoặc trả về tất cả tùy bạn)
            if (string.IsNullOrEmpty(keyword)) return suggestions;

            // 2. Chạy vòng lặp duyệt qua tất cả từ vựng hiện có
            foreach (string word in KeyList)
            {
                // 3. Kiểm tra: Nếu từ này BẮT ĐẦU bằng từ khóa (Không phân biệt hoa thường)
                // Ví dụ: word="Apple", keyword="ap" -> True
                if (word.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    suggestions.Add(word); // Thêm vào danh sách gợi ý
                }
            }

            // 4. Trả về danh sách đã lọc
            return suggestions;
        }

        // =========================================================
        // THUẬT TOÁN 6: Autocorrect (Dựa trên Levenshtein Distance)
        // Gõ sai vẫn tìm ra từ đúng (Ví dụ: aple -> apple)
        // =========================================================

        // 1. Hàm toán học tính điểm khác biệt giữa 2 chuỗi
        // Trả về 0 nếu giống hệt, trả về 1 nếu sai 1 ký tự, v.v.
        private int ComputeLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        // 2. Hàm Tìm kiếm Autocorrect
        public List<string> SearchAutocorrect(string keyword)
        {
            List<string> suggestions = new List<string>();

            // Nếu từ quá ngắn (dưới 3 ký tự) thì không sửa lỗi để tránh rác
            if (keyword.Length < 3) return suggestions;

            foreach (string word in KeyList)
            {
                // Tối ưu: Nếu độ dài chênh lệch quá 2 ký tự thì bỏ qua luôn
                if (Math.Abs(word.Length - keyword.Length) > 2) continue;

                // Tính toán xem sai bao nhiêu ký tự
                int dist = ComputeLevenshteinDistance(keyword, word);

                // QUAN TRỌNG: Ngưỡng chấp nhận lỗi
                // dist <= 2 nghĩa là cho phép sai tối đa 2 ký tự (gõ thiếu, thừa, hoặc sai 2 chữ)
                if (dist > 0 && dist <= 2)
                {
                    suggestions.Add(word);
                }

                // Giới hạn chỉ lấy 5 từ gợi ý tốt nhất thôi
                if (suggestions.Count >= 5) break;
            }
            return suggestions;
        }
        private void InitSampleData()
        {
            // Tạo file mẫu chuẩn CSV
            File.WriteAllText(_filePath, "apple,quả táo\nbanana,quả chuối\ncomputer,máy tính");
        }
    }
}