using EnglishVietnameseDictionary;
using System;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;

namespace EnglishVietnameseDictionary
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Chuẩn bị giao diện
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // Lệnh quan trọng nhất: Chạy Form1 lên
            // =========================================================
            Application.Run(new Form1());
            // =========================================================
            // Chạy Benchmark
            RunBenchmarkOnProjectData();
        }

        // HÀM CHẠY KIỂM THỬ TRÊN DỮ LIỆU THỰC
        static void RunBenchmarkOnProjectData()
        {
            // 1. KHỞI TẠO CÁC CLASS CỦA DỰ ÁN (Giống hệt trong Form1)
            DictionaryManager manager = new DictionaryManager();
            DictionaryBasic basic = new DictionaryBasic(manager);
            DictionaryAlgo algo = new DictionaryAlgo(manager);

            // Kết nối (Wiring) để tránh lỗi Null
            basic.SetAlgo(algo);
            algo.SetBasic(basic);

            // WARM-UP (Chạy nháp để mồi cho hệ thống khởi động)
            // Bước này cực quan trọng để công bằng
            algo.SortData();

            // Hàm sinh dữ liệu random kiêm nạp lại dữ liệu sau mỗi vòng lặp
            Action ReloadData = () =>
            {
                manager.DataDict.Clear();
                manager.KeyList.Clear();
                List<string> hugeData = new List<string>();
                Random r = new Random();
                for (int i = 0; i < 10000; i++) 
                    hugeData.Add("word" + r.Next(1, 100000));
                manager.KeyList = new List<string>(hugeData);
            };

            // Lần nạp đầu tiên để kiểm tra
            ReloadData();
            if (manager.KeyList.Count == 0)
            {
                MessageBox.Show("Không tìm thấy file dữ liệu hoặc file rỗng!\nVui lòng kiểm tra lại đường dẫn 'data.csv'.");
                return;
            }

            StringBuilder report = new StringBuilder();
            report.AppendLine($"=== KẾT QUẢ BENCHMARK ({manager.KeyList.Count} từ) ===\n");
            Stopwatch sw = new Stopwatch();
            int loop = 1;

            // ---------------------------------------------------------
            // TEST 1: QUICK SORT
            // ---------------------------------------------------------
            long totalTicks1 = 0;
            for (int i = 0; i < loop; i++) 
            {
                ReloadData(); // Reset dữ liệu về trạng thái lộn xộn ban đầu
                sw.Start();
                algo.SortData(); // Gọi hàm QuickSort trong class Algo của bạn
                sw.Stop();
                totalTicks1 += sw.ElapsedTicks;
                sw.Reset();
            }
            double quickavgtime = (double)totalTicks1 / loop / Stopwatch.Frequency * 1000;
            report.AppendLine($"1. QuickSort: {quickavgtime:F4} ms");
            // ---------------------------------------------------------
            // TEST 2: SELECTION SORT
            // ---------------------------------------------------------
            long totalTicks2 = 0;
            for (int i = 0; i < loop; i++) 
            {
                ReloadData(); // Reset dữ liệu lại (nếu không nó sẽ sort trên list đã xếp rồi -> sai kết quả)
                sw.Start();
                algo.SelectionSort(); // Gọi hàm SelectionSort
                sw.Stop();
                totalTicks2 += sw.ElapsedTicks;
                sw.Reset();
            }
            double seavgtime = (double)totalTicks2 / loop / Stopwatch.Frequency * 1000;
            report.AppendLine($"2. SelectionSort: {seavgtime:F4} ms");

            // ---------------------------------------------------------
            // TEST 3: BUBBLE SORT
            // ---------------------------------------------------------
            long totalTicks3 = 0;
            for (int i = 0; i < loop; i++) 
            {
                ReloadData(); // Reset dữ liệu lại
                sw.Start();
                algo.BubbleSort(); // Gọi hàm BubbleSort
                sw.Stop();
                totalTicks3 += sw.ElapsedTicks;
                sw.Reset();
            }
            double bbavgtime = (double)totalTicks3 / loop / Stopwatch.Frequency * 1000;
            report.AppendLine($"3. BubbleSort: {bbavgtime:F4} ms");

            // ---------------------------------------------------------
            // TEST 4: TÌM KIẾM (Linear vs Binary)
            // ---------------------------------------------------------
            // Đảm bảo list đã sort để Binary Search chạy được
            algo.SortData();
            string targetWord = manager.KeyList[manager.KeyList.Count - 5]; // Lấy từ gần cuối để test

            // Test Linear (dùng hàm có sẵn của List)
            long totalTicks4 = 0;

            for (int k = 0; k < 1000; k++) 
            {
                sw.Start();
                algo.GetSuggestionsManual(targetWord);
                sw.Stop();
                totalTicks4 += sw.ElapsedTicks;
                sw.Reset();
            }
            double linavgtime = (double)totalTicks4 / 1000 / Stopwatch.Frequency * 1000;
            report.AppendLine($"\n4. Linear Search (1000 lần): {linavgtime:F4} ms");

            // Test Binary (dùng hàm của Algo)
            long totalTicks5 = 0;
            for (int k = 0; k < 1000; k++)  
            {
                sw.Start();
                algo.BinarySearchKey(targetWord);
                sw.Stop();
                totalTicks5 += sw.ElapsedTicks;
                sw.Reset();
            }
            double binavgtime = (double)totalTicks5 / 1000 / Stopwatch.Frequency * 1000;
            report.AppendLine($"5. Binary Search (1000 lần): {binavgtime:F4} ms");


            // 3. HIỂN THỊ KẾT QUẢ
            // Vì là WinForms nên ta show Popup thông báo kết quả
            MessageBox.Show(report.ToString(), "Benchmark Report");
        }
    }
}