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
    public partial class Report2 : Form
    {
        DateTime dateStart;
        DateTime dateEnd;
        private List<ReportPopular> reportsPopular = new List<ReportPopular>();
        public Report2()
        {
            InitializeComponent();
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
                // Получите список жанров
                List<Genre> genres = context.Genres
                    .Include(x=>x.Books)
                    .ThenInclude(x=>x.Journals).ToList();

                // Получите количество взятых книг по каждому жанру за указанный период
                List<ReportPopular> reports = genres.Select(genre => new ReportPopular
                {
                    Genre = genre.Name,
                    BooksTaken = genre.Books
                    .SelectMany(book => book.Journals)
                        .Count(journal => journal.DateGive >= dateStart && journal.DateGive <= dateEnd)
                }).ToList();

                // Вычислите процент относительно общего числа взятых книг
                int totalBooksTaken = reports.Sum(report => report.BooksTaken);
                reports.ForEach(report => report.Percentage = (double)report.BooksTaken / totalBooksTaken * 100);
                
                reportsPopular = reports.OrderByDescending(x => x.Percentage).ToList();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = reportsPopular;
                dataGridView1.Columns["Genre"].HeaderText = "Жанр";
                dataGridView1.Columns["BooksTaken"].HeaderText = "Книг взяли";
                dataGridView1.Columns["Percentage"].HeaderText = "Процент";
            }
        }

        /// <summary>
        /// Экспортировать
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
                var reportTitle = "Название отчета: Актуальные жанры книг\n";
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
                var table = doc.AddTable(reportsPopular.Count + 1, 3); // количество строк и столбцов
                table.Rows[0].Cells[0].Paragraphs.First().Append("Жанр");
                table.Rows[0].Cells[1].Paragraphs.First().Append("Книг выдано");
                table.Rows[0].Cells[2].Paragraphs.First().Append("Процентное соотношение, %");

                for (int i = 0; i < reportsPopular.Count; i++)
                {
                    table.Rows[i + 1].Cells[0].Paragraphs.First().Append(reportsPopular[i].Genre);
                    table.Rows[i + 1].Cells[1].Paragraphs.First().Append(reportsPopular[i].BooksTaken.ToString());
                    table.Rows[i + 1].Cells[2].Paragraphs.First().Append(reportsPopular[i].Percentage.ToString());
                }

                // Добавляем таблицу в документ
                doc.InsertTable(table);

                // Добавляем информацию о директоре и подпись
                var directorInfo = "Александров А.А.";
                var directorSignature = "_________";

                // Добавляем информацию о директоре и подпись в одну строку
                var directorAndSignature = doc.InsertParagraph($"\nДиректор: {directorInfo.PadRight(104)} Подпись: {directorSignature}");
                directorAndSignature.Alignment = Alignment.left;

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
    public class ReportPopular
    {
        public string Genre { get; set; }

        public int BooksTaken { get; set; }

        public double Percentage { get; set; }
    }
}
