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
        // THUẬT TOÁN 4: Hashing Lookup (Tra cứu từ điển)
        // Lấy nghĩa của từ dựa vào Key. Đây là sức mạnh của Dictionary.
        // Độ phức tạp: O(1)
        // =========================================================
        public string GetMeaning(string key)
        {
            if (DataDict.TryGetValue(key, out string meaning))
            {
                return meaning;
            }
            return null;
        }

        // =========================================================
        // THUẬT TOÁN 5: Binary Search Prefix
        // Tìm vị trí đầu tiên xuất hiện tiền tố, sau đó lấy các từ liền kề
        // ĐIỀU KIỆN: Danh sách KeyList PHẢI ĐƯỢC SẮP XẾP trước
        // =========================================================
        public List<string> FilterListAdvanced(string keyword)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(keyword) || KeyList.Count == 0)
                return result;

            // Dùng Binary Search tìm vị trí đầu tiên (First Occurrence) 
            int firstIndex = -1;
            int low = 0;
            int high = KeyList.Count - 1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                string midWord = KeyList[mid];

                // Kiểm tra xem từ ở giữa có bắt đầu bằng keyword không?
                bool isMatch = midWord.StartsWith(keyword, StringComparison.OrdinalIgnoreCase);

                if (isMatch)
                {
                    firstIndex = mid; // Ghi nhận vị trí này
                    high = mid - 1;   // QUAN TRỌNG: Tiếp tục tìm về phía bên TRÁI để xem có từ nào trước đó không
                }
                else
                {
                    // Nếu không khớp, ta so sánh thứ tự từ điển để biết nên đi trái hay phải
                    // Ví dụ: mid="ant", key="app" -> mid nhỏ hơn -> Đi sang Phải
                    int compare = String.Compare(midWord, keyword, StringComparison.OrdinalIgnoreCase);

                    if (compare < 0)
                        low = mid + 1;
                    else
                        high = mid - 1;
                }
            }

            // Thu thập kết quả
            // Nếu tìm thấy vị trí đầu tiên, ta bắt đầu duyệt từ đó sang phải
            if (firstIndex != -1)
            {
                for (int i = firstIndex; i < KeyList.Count; i++)
                {
                    // Kiểm tra lại: Nếu từ này vẫn bắt đầu bằng keyword -> Lấy
                    if (KeyList[i].StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(KeyList[i]);
                    }
                    else
                    {
                        // Nếu gặp từ không khớp -> DỪNG NGAY LẬP TỨC (Break)
                        // Vì danh sách đã sắp xếp nên các từ sau chắc chắn cũng không khớp
                        break;
                    }
                }
            }

            return result;
        }

        // =========================================================
        // THUẬT TOÁN 6: Levenshtein Distance (Gợi ý sửa lỗi chính tả)
        // Thuật toán quy hoạch động tính khoảng cách khác biệt giữa 2 chuỗi
        // Dùng khi người dùng gõ sai từ (Ví dụ: "helo" -> gợi ý "hello")
        // =========================================================
        public List<string> FuzzySearch(string wrongWord)
        {
            List<string> suggestions = new List<string>();
            foreach (var key in KeyList)
            {
                if (Math.Abs(key.Length - wrongWord.Length) <= 2) // Chỉ so sánh các từ có độ dài gần nhau
                {
                    if (CalculateLevenshtein(wrongWord.ToLower(), key.ToLower()) <= 2)
                    {
                        suggestions.Add(key);
                        if (suggestions.Count >= 5) break; // Chỉ lấy tối đa 5 gợi ý
                    }
                }
            }
            return suggestions;
        }

        private int CalculateLevenshtein(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

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
        private void InitSampleData()
        {
            // Tạo file mẫu chuẩn CSV
            File.WriteAllText(_filePath, "apple,quả táo\nbanana,quả chuối\ncomputer,máy tính");
        }
    }
}