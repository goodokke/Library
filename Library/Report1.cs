using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Library
{
    /// <summary>
    /// Количество выданных книг
    /// </summary>
    public partial class Report1 : Form
    {
        private List<ReportCount> reportCounts = new List<ReportCount>();
        DateTime dateStart;
        DateTime dateEnd;
        public Report1()
        {
            InitializeComponent();
            //dataGridView1.Columns.Add("Month", "Месяц");
            //dataGridView1.Columns.Add("Count", "Книг выдано");
            //dataGridView1.Columns.Add("CountReaders", "Новых читателей");
            CenterToScreen();
        }

        /// <summary>
        /// Применить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            dateStart = dateTimePicker1.Value;
            dateEnd = dateTimePicker2.Value;

            int startMonth = dateStart.Month;
            int endMonth = dateEnd.Month;

            string monthName = $"{dateStart:Y}";
            using (DBContext context = new DBContext())
            {
                var countReaders = context.Readers.Include(x => x.Journals)
                    .Where(x =>
                        x.Journals.Where(j => j.DateGive >= dateStart && j.DateGive <= dateEnd).Any()
                        && !x.Journals.Where(j => j.DateGive < dateStart && j.DateGive > dateEnd).Any()).Count();

                var count = context.Journals.Where(j=>j.DateGive >= dateStart && j.DateGive <= dateEnd).Count();
                reportCounts.Add(new ReportCount { Month = monthName, Count = count, CountReaders = countReaders });
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = reportCounts;
                dataGridView1.Columns["Month"].HeaderText = "Месяц";
                dataGridView1.Columns["Count"].HeaderText = "Книг выдано";
                dataGridView1.Columns["CountReaders"].HeaderText = "Новых читателей";
            }
        }

        /// <summary>
        /// Экспорт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            string filePath = $"Отчет_Количество_выданных_книг_{Guid.NewGuid().ToString()}.docx";

            // Создаем документ Word
            using (var doc = DocX.Create(filePath))
            {
                // Добавляем заголовок отчета
                var reportTitle = "Название отчета: Количество выданных книг за год\n";
                doc.InsertParagraph(reportTitle).Alignment = Alignment.center;

                // Добавляем дату отчета
                var start = dateStart.ToString("dd.MM.yyyy");
                var end = dateEnd.ToString("dd.MM.yyyy");
                doc.InsertParagraph($"Выбранный диапазон дат: {start} - {end}\n").Alignment = Alignment.center;

                // Добавляем название (еще одна строка)
                var sectionTitle = $"Дата формирования отчета: {DateTime.UtcNow.ToString("dd.MM.yyyy")}\n";
                doc.InsertParagraph(sectionTitle).Alignment = Alignment.center;

                // Добавляем заголовок
                //doc.InsertParagraph("Отчет");

                // Добавляем таблицу с данными
                var table = doc.AddTable(reportCounts.Count + 1, 3); // количество строк и столбцов
                table.Rows[0].Cells[0].Paragraphs.First().Append("Месяц");
                table.Rows[0].Cells[1].Paragraphs.First().Append("Книг выдано");
                table.Rows[0].Cells[2].Paragraphs.First().Append("Новых читателей");

                for (int i = 0; i < reportCounts.Count; i++)
                {
                    table.Rows[i + 1].Cells[0].Paragraphs.First().Append(reportCounts[i].Month);
                    table.Rows[i + 1].Cells[1].Paragraphs.First().Append(reportCounts[i].Count.ToString());
                    table.Rows[i + 1].Cells[2].Paragraphs.First().Append(reportCounts[i].CountReaders.ToString());
                }

                // Добавляем таблицу в документ
                doc.InsertTable(table);

                // Добавляем информацию о директоре и подпись
                var directorInfo = "Александров А.А.";
                var directorSignature = "_________";

                // Добавляем информацию о директоре и подпись в одну строку
                var directorAndSignature = doc.InsertParagraph($"\nДиректор: {directorInfo.PadRight(104)} Подпись: {directorSignature}");
                directorAndSignature.Alignment = Alignment.left;
                // Сохраняем документ
                OpenWordDocument(doc);
            }
        }

        static void OpenWordDocument(DocX document)
        {
            // Сохраняем документ во временном файле (в памяти)
            string tempFilePath = System.IO.Path.GetTempFileName() + ".docx";
            document.SaveAs(tempFilePath);

            // Укажите полный путь к WINWORD.EXE
            string wordPath = @"C:\Program Files (x86)\Microsoft Office\root\Office16\WINWORD.EXE";

            // Открываем документ в приложении Word
            Process.Start(wordPath, tempFilePath);
        }
    }

    public class ReportCount
    {
        public int Count { get; set; }

        public int CountReaders { get; set; }

        public string Month { get; set; }
    }
}
