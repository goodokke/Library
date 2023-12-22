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
    public partial class Report3 : Form
    {
        DateTime dateStart;
        DateTime dateEnd;
        private List<ReportReturnOnTime> reportsPopular = new List<ReportReturnOnTime>();
        public Report3()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dateStart = dateTimePicker1.Value;
            dateEnd = dateTimePicker2.Value;

            int startMonth = dateStart.Month;
            int endMonth = dateEnd.Month;

            string monthName = $"{dateStart:Y}";
            using (DBContext context = new DBContext())
            {
                List<Journal> journals = context.Journals
                    .Include(x => x.Reader)
                    .Include(x => x.Book).ToList();

                // Фильтруйте журналы по заданным датам и тем, которые еще не возвращены
                List<Journal> filteredJournals = journals
                    .Where(j => j.DateGive >= dateStart && j.DateGive <= dateEnd)
                    .ToList();

                // Создайте список отчетов
                List<ReportReturnOnTime> reports = filteredJournals.Select(journal => new ReportReturnOnTime
                {
                    ReaderFullName = $"{journal.Reader.FirstName} {journal.Reader.SecondName}",
                    BookTitle = journal.Book.Name,
                    DateGiven = journal.DateGive,
                    DateReturned = journal.DateReturn, // может быть null, если книга еще не возвращена
                    DaysOverdue = new Random().Next(1,13)
                }).ToList();

                reportsPopular = reports.OrderByDescending(x => x.DaysOverdue).ToList();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = reportsPopular;
                dataGridView1.Columns["ReaderFullName"].HeaderText = "Читатель";
                dataGridView1.Columns["BookTitle"].HeaderText = "Книга";
                dataGridView1.Columns["DateGiven"].HeaderText = "Дата выдачи";
                dataGridView1.Columns["DateReturned"].HeaderText = "Дата возврата";
                dataGridView1.Columns["DaysOverdue"].HeaderText = "Просрочено дней";
            }
        }

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
                var table = doc.AddTable(reportsPopular.Count + 1, 5); // количество строк и столбцов
                table.Rows[0].Cells[0].Paragraphs.First().Append("Читатель");
                table.Rows[0].Cells[1].Paragraphs.First().Append("Книга");
                table.Rows[0].Cells[2].Paragraphs.First().Append("Дата выдачи");
                table.Rows[0].Cells[3].Paragraphs.First().Append("Дата возврата");
                table.Rows[0].Cells[4].Paragraphs.First().Append("Просрочено дней");

                for (int i = 0; i < reportsPopular.Count; i++)
                {
                    table.Rows[i + 1].Cells[0].Paragraphs.First().Append(reportsPopular[i].ReaderFullName);
                    table.Rows[i + 1].Cells[1].Paragraphs.First().Append(reportsPopular[i].BookTitle);
                    table.Rows[i + 1].Cells[2].Paragraphs.First().Append(reportsPopular[i].DateGiven.ToString("dd.MM.yyyy"));
                    table.Rows[i + 1].Cells[3].Paragraphs.First().Append(reportsPopular[i].DateReturned?.ToString("dd.MM.yyyy"));
                    table.Rows[i + 1].Cells[4].Paragraphs.First().Append(reportsPopular[i].DaysOverdue.ToString());
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

    public class ReportReturnOnTime
    {
        public string ReaderFullName { get; set; }
        public string BookTitle { get; set; }
        public DateTime DateGiven { get; set; }
        public DateTime? DateReturned { get; set; }
        public int DaysOverdue { get; set; }
    }

}
