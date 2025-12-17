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
        // Sửa kiểu trả về từ void thành int (số từ thêm được)
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
                        // Nếu trùng thì kệ nó, không làm gì cả (tức là bỏ qua)
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
        // THUẬT TOÁN 2: QuickSort (Sắp xếp dữ liệu)
        // Sắp xếp KeyList để phục vụ tìm kiếm nhị phân và hiển thị đẹp
        // =========================================================
        public void SortData()
        {
            // List.Sort() trong C# sử dụng thuật toán Introsort (biến thể của QuickSort)
            // Độ phức tạp trung bình: O(n log n)
            KeyList.Sort();

            // Lưu ý: ValueList không cần sort theo, vì ta sẽ tra cứu nghĩa bằng Dictionary
        }

        // Helper: Đồng bộ lại List từ Dictionary (Reset)
        private void SyncListsFromDict()
        {
            KeyList = DataDict.Keys.ToList();
            ValueList = DataDict.Values.ToList();
            SortData(); // Luôn sắp xếp sau khi load
        }

        // =========================================================
        // THUẬT TOÁN 3: Binary Search (Tìm kiếm nhị phân)
        // Tìm kiếm trên List đã sắp xếp. Nhanh hơn nhiều so với duyệt từng phần tử.
        // Độ phức tạp: O(log n)
        // =========================================================
        public int BinarySearchKey(string keyword)
        {
            // C# hỗ trợ sẵn BinarySearch cho List
            return KeyList.BinarySearch(keyword, StringComparer.OrdinalIgnoreCase);
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
        // THUẬT TOÁN 5: Linear Prefix Filtering (Gợi ý từ)
        // Duyệt danh sách để tìm các từ bắt đầu bằng ký tự người dùng nhập
        // =========================================================
        public List<string> GetSuggestions(string prefix)
        {
            // Sử dụng LINQ (duyệt tuyến tính)
            return KeyList.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
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

        // 1. Hàm Xóa từ
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

        // 2. Hàm Sửa nghĩa (Chỉ sửa nghĩa tiếng Việt)
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

        private void InitSampleData()
        {
            // Tạo file mẫu chuẩn CSV
            File.WriteAllText(_filePath, "apple,quả táo\nbanana,quả chuối\ncomputer,máy tính");
        }
    }
}