using EnglishVietnameseDictionary;
using System;
using System.Windows.Forms;

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

            // Lệnh quan trọng nhất: Chạy Form1 lên
            // Nếu chữ Form1 bị đỏ, hãy đảm bảo Form1.cs của bạn 
            // đang có namespace là DictionaryDSA
            Application.Run(new Form1());
        }
    }
}