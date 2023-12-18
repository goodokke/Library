using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library
{
    public partial class Books : Form
    {
        bool isNewRecord = true;
        Book book;
        int index;
        public int SelectedCellGenre = -1;
        public int SelectedCellPublisher = -1;
        public Author SelectedAuthor;
        private Journals formJournals;
        public Books(Journals formJournals = null)
        {
            this.formJournals = formJournals;
            if (formJournals != null)
            {
                formJournals.SelectedCellReader = -1;
            }
            InitializeComponent();
            getGenres();
            getPublishers();
            getRecords();
            dataGridView2.Columns.Add("Id", "Id");
            dataGridView2.Columns["Id"].Visible = false;
            dataGridView2.Columns.Add("SecondName", "Фамилия");
            dataGridView2.Columns.Add("FirstName", "Имя");
            dataGridView2.Columns.Add("ThirdName", "Отчество");
        }

        public void getRecords()
        {
            using (DBContext DBContext = new DBContext())
            {
                List<Book> books = DBContext.Books.Include(b => b.Genre).Include(b => b.Publisher).Include(b => b.Authors).ToList();

                dataGridView1.DataSource = null;
                var data = books
                    .Select(b => new
                    {
                        Id = b.Id,
                        Name = b.Name,
                        YearPublish = b.YearPublish,
                        Genre = b.Genre.Name,
                        Publisher = b.Publisher.Name,
                        Authors = String.Join(", ", b.Authors.Select(a => a.SecondName + ' ' + a.FirstName[0] + ". " + a.ThirdName[0] + '.'))
                    }).ToList();

                dataGridView1.DataSource = data;
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["Name"].HeaderText = "Наименование";
                dataGridView1.Columns["YearPublish"].HeaderText = "Год издания";
                dataGridView1.Columns["Genre"].HeaderText = "Жанр";
                dataGridView1.Columns["Publisher"].HeaderText = "Издательство";
                dataGridView1.Columns["Authors"].HeaderText = "Авторы";
            }
        }

        void getGenres()
        {
            comboBox1.Items.Clear();
            comboBox1.ResetText();

            List<Genre> genres = new List<Genre>();

            using (DBContext db = new DBContext())
            {
                genres = db.Genres.ToList();
            }

            foreach (Genre genre in genres)
            {
                comboBox1.Items.Add(genre.Name);
            }
            comboBox1.SelectedIndex = SelectedCellGenre;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (formJournals != null)
            {
                formJournals.SelectedCellBook = e.RowIndex;
                this.Close();
            }
        }

        void getPublishers()
        {
            comboBox2.Items.Clear();
            comboBox2.ResetText();

            List<Publisher> publishers = new List<Publisher>();
            Publisher publisher = new Publisher();

            using (DBContext db = new DBContext())
            {
                publishers = db.Publishers.ToList();

                if (SelectedCellPublisher != -1) publisher = db.Publishers.Single(r => r.Id == SelectedCellPublisher);
            }

            foreach (Publisher p in publishers)
            {
                comboBox2.Items.Add(p.Name);
            }

            if (SelectedCellPublisher != -1) comboBox2.SelectedItem = publisher.Name;
            else comboBox2.SelectedIndex = SelectedCellPublisher;
        }

        public void setAuthors()
        {
            dataGridView2.Rows.Add(SelectedAuthor.Id, SelectedAuthor.SecondName, SelectedAuthor.FirstName, SelectedAuthor.ThirdName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.ResetText();
            comboBox2.ResetText();
            dataGridView1.Visible = false;
            textBox1.Text = "";
            textBox2.Text = "";
            groupBox1.Visible = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            dataGridView2.Rows.Clear();
            index = dataGridView1.RowCount;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            groupBox1.Visible = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button4.Enabled = true;
            isNewRecord = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0 && dataGridView1.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using (DBContext DBContext = new DBContext())
                    {
                        Book book = DBContext.Books.Where(b => b.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();
                        Journal journal = DBContext.Journals.Where(b => b.BookId == book.Id).FirstOrDefault();
                        if (journal == null)
                        {
                            DBContext.Books.Remove(book);
                            DBContext.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Данная запись связана с другими записями.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    getRecords();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (DBContext DBContext = new DBContext())
            {
                Genre? genre = DBContext.Genres.Where(g => g.Name == comboBox1.Text).FirstOrDefault();
                Publisher? publisher = DBContext.Publishers.Where(g => g.Name == comboBox2.Text).FirstOrDefault();
                List<Author> authors = new List<Author>();
                List<Author> authorsAll = DBContext.Authors.ToList();

                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    authors.Add(DBContext.Authors.Where(a => a.Id == Int32.Parse(dataGridView2.Rows[i].Cells[0].Value.ToString())).FirstOrDefault());
                }

                int year = 0;
                if (genre != null && publisher != null && Int32.TryParse(textBox2.Text, out year)
                    && authors.Count != 0 && textBox1.Text.Length > 0)
                {
                    if (isNewRecord)
                    {
                        Book book = new Book
                        {
                            Name = textBox1.Text,
                            YearPublish = year,
                            GenreId = genre!.Id,
                            PublisherId = publisher!.Id
                        };
                        book.Authors.AddRange(authors);
                        DBContext.Books.Add(book);
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        if (book != null)
                        {
                            Book? book = DBContext.Books.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).Include(b => b.Genre).Include(b => b.Publisher).Include(b => b.Authors).FirstOrDefault();

                            book.Name = textBox1.Text;
                            book.YearPublish = year;
                            book.GenreId = genre!.Id;
                            book.Genre = genre;
                            book.PublisherId = publisher!.Id;
                            book.Publisher = publisher;
                            book.Authors.Clear();
                            book.Authors = authors;

                            DBContext.SaveChanges();
                            isNewRecord = true;
                        }
                    }

                    dataGridView1.Visible = true;
                    groupBox1.Visible = false;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button4.Enabled = true;

                    getRecords();
                    dataGridView1.Rows[index].Selected = true;
                }
                else
                {
                    MessageBox.Show("Данные введены неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Genres genres = new Genres(this);
            genres.ShowDialog();
            getGenres();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Publishers publishers = new Publishers(this);
            publishers.ShowDialog();
            getPublishers();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Authors authors = new Authors(this);
            authors.ShowDialog();
            setAuthors();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView2.RowCount != 0)
            {
                for (int i = 0; i < dataGridView2.SelectedRows.Count; i++)
                {
                    dataGridView2.Rows.Remove(dataGridView2.SelectedRows[i]);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                using (DBContext DBContext = new DBContext())
                {
                    book = DBContext.Books.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).Include(b => b.Genre).Include(b => b.Publisher).Include(b => b.Authors).FirstOrDefault();

                    dataGridView1.Visible = false;
                    textBox1.Text = book.Name.ToString();
                    textBox2.Text = book.YearPublish.ToString();
                    comboBox2.SelectedItem = book.Publisher.Name;
                    comboBox1.SelectedItem = book.Genre.Name;

                    dataGridView2.Rows.Clear();
                    for (int i = 0; i < book.Authors.Count; i++)
                    {
                        dataGridView2.Rows.Add(book.Authors[i].Id, book.Authors[i].SecondName, book.Authors[i].FirstName, book.Authors[i].ThirdName);
                    }

                    groupBox1.Visible = true;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button4.Enabled = false;
                    isNewRecord = false;
                    index = dataGridView1.SelectedRows[0].Index;
                }
            }
        }
    }
}
