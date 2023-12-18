using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library
{
    public partial class Journals : Form
    {
        bool isNewRecord = true;
        Journal journal;
        int index;
        public int SelectedCellBook = -1;
        public int SelectedCellReader = -1;
        public int SelectedCellEmployee = -1;
        List<Journal> journals;
        public Journals()
        {
            InitializeComponent();
            getRecords();
            getBooks();
            getReaders();
            getEmployees();
        }

        public void getRecords()
        {
            using (DBContext DBContext = new DBContext())
            {
                journals = DBContext.Journals.Include(b => b.Book).Include(b => b.Reader).Include(b => b.Employee).ToList();

                dataGridView1.DataSource = null;
                var data = journals
                    .Select(b => new
                    {
                        Id = b.Id,
                        DateGive = b.DateGive,
                        DateReturn = b.DateReturn,
                        Book = String.Join(", ", b.Book.Authors.Select(a => a.SecondName + ' ' + a.FirstName[0] + ". " + a.ThirdName[0] + '.')) + " \"" + b.Book.Name + "\"",
                        Reader = b.Reader.LibraryCard,
                        Employee = b.Employee.SecondName + ' ' + b.Employee.FirstName + " " + b.Employee.ThirdName
                    }).ToList();

                dataGridView1.DataSource = data;
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["DateGive"].HeaderText = "Дата выдачи";
                dataGridView1.Columns["DateReturn"].HeaderText = "Дата возврата";
                dataGridView1.Columns["Book"].HeaderText = "Книга";
                dataGridView1.Columns["Reader"].HeaderText = "Номер читательского билета";
                dataGridView1.Columns["Employee"].HeaderText = "Библиотекарь";
            }
        }

        void getReaders()
        {
            comboBox1.Items.Clear();
            comboBox1.ResetText();

            List<Reader> readers = new List<Reader>();
            Reader reader = new Reader();

            using (DBContext db = new DBContext())
            {
                readers = db.Readers.ToList();

                if (SelectedCellReader != -1) reader = db.Readers.Single(r => r.Id == SelectedCellReader);
            }

            foreach (Reader r in readers)
            {
                comboBox1.Items.Add(r.SecondName + ' ' + r.FirstName + ' ' + r.ThirdName);
            }

            if (SelectedCellReader != -1) comboBox1.SelectedItem = reader.SecondName + ' ' + reader.FirstName + ' ' + reader.ThirdName;
            else comboBox1.SelectedIndex = SelectedCellReader;
        }

        void getBooks()
        {
            comboBox2.Items.Clear();
            comboBox2.ResetText();

            List<Book> books = new List<Book>();

            using (DBContext db = new DBContext())
            {
                books = db.Books.Include(b => b.Genre).Include(b => b.Publisher).Include(b => b.Authors).ToList();
            }

            foreach (Book b in books)
            {
                comboBox2.Items.Add(String.Join(", ", b.Authors.Select(a => a.SecondName + ' ' + a.FirstName[0] + ". " + a.ThirdName[0] + '.')) + " \"" + b.Name + "\"");
            }
            comboBox2.SelectedIndex = SelectedCellBook;
        }

        void getEmployees()
        {
            comboBox3.Items.Clear();
            comboBox3.ResetText();

            List<Employee> employees = new List<Employee>();

            using (DBContext db = new DBContext())
            {
                employees = db.Employees.ToList();
            }

            foreach (Employee e in employees)
            {
                comboBox3.Items.Add(e.SecondName + ' ' + e.FirstName + " " + e.ThirdName);
            }
            comboBox3.SelectedIndex = SelectedCellEmployee;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.ResetText();
            comboBox2.ResetText();
            comboBox3.ResetText();
            dataGridView1.Visible = false;
            groupBox1.Visible = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
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
                        Journal journal = DBContext.Journals.Where(b => b.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();
                        DBContext.Journals.Remove(journal);
                        DBContext.SaveChanges();
                    }
                    getRecords();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (DBContext DBContext = new DBContext())
            {
                Reader? reader = DBContext.Readers.Where(e => e.SecondName + ' ' + e.FirstName + " " + e.ThirdName == comboBox1.Text).FirstOrDefault();
                List<Book> books;
                Book? book = null;
                books = DBContext.Books.Include(b => b.Genre).Include(b => b.Publisher).Include(b => b.Authors).ToList();
                List<string> namesBook = new List<string>();
                foreach (Book b in books)
                {
                    namesBook.Add(String.Join(", ", b.Authors.Select(a => a.SecondName + ' ' + a.FirstName[0] + ". " + a.ThirdName[0] + '.')) + " \"" + b.Name + "\"");
                }

                for (int i = 0; i < namesBook.Count; i++)
                {
                    if (namesBook[i] == comboBox2.Text)
                    {
                        book = books[i];
                    }
                }
                Employee? employee = DBContext.Employees.Where(e => e.SecondName + ' ' + e.FirstName + " " + e.ThirdName == comboBox3.Text).FirstOrDefault();

                if (reader != null && book != null && employee != null)
                {
                    if (isNewRecord)
                    {
                        Journal journal;
                        if (dateTimePicker2.CustomFormat == " ")
                        {
                            journal = new Journal
                            {
                                DateGive = dateTimePicker1.Value.Date,
                                DateReturn = null,
                                ReaderId = reader!.Id,
                                BookId = book!.Id,
                                EmployeeId = employee!.Id
                            };
                        }
                        else
                        {
                            journal = new Journal
                            {
                                DateGive = dateTimePicker1.Value.Date,
                                DateReturn = dateTimePicker2.Value.Date,
                                ReaderId = reader!.Id,
                                BookId = book!.Id,
                                EmployeeId = employee!.Id
                            };
                        }
                        DBContext.Journals.Add(journal);
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        if (journal != null)
                        {
                            journal = DBContext.Journals.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).Include(b => b.Reader).Include(b => b.Book).Include(b => b.Employee).FirstOrDefault();
                            if (dateTimePicker2.CustomFormat == " ")
                            {
                                journal.DateGive = dateTimePicker1.Value.Date;
                                journal.DateReturn = null;
                                journal.ReaderId = reader!.Id;
                                journal.Reader = reader;
                                journal.BookId = book!.Id;
                                journal.Book = book;
                                journal.EmployeeId = employee!.Id;
                                journal.Employee = employee;
                            }
                            else
                            {
                                journal.DateGive = dateTimePicker1.Value.Date;
                                journal.DateReturn = dateTimePicker2.Value.Date;
                                journal.ReaderId = reader!.Id;
                                journal.Reader = reader;
                                journal.BookId = book!.Id;
                                journal.Book = book;
                                journal.EmployeeId = employee!.Id;
                                journal.Employee = employee;
                            }

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

        private void button9_Click(object sender, EventArgs e)
        {
            Readers readers = new Readers(this);
            readers.ShowDialog();
            getReaders();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Books books = new Books(this);
            books.ShowDialog();
            getBooks();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Employees employees = new Employees(this);
            employees.ShowDialog();
            getEmployees();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                using (DBContext DBContext = new DBContext())
                {
                    journal = DBContext.Journals.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).Include(b => b.Reader).Include(b => b.Book.Authors).Include(b => b.Employee).FirstOrDefault();

                    dataGridView1.Visible = false;
                    dateTimePicker1.Value = journal.DateGive;
                    if (journal.DateReturn != null) dateTimePicker2.Value = (DateTime)journal.DateReturn;
                    else dateTimePicker2.CustomFormat = " ";
                    comboBox1.SelectedItem = journal.Reader.SecondName + ' ' + journal.Reader.FirstName + ' ' + journal.Reader.ThirdName;
                    comboBox2.SelectedItem = String.Join(", ", journal.Book.Authors.Select(a => a.SecondName + ' ' + a.FirstName[0] + ". " + a.ThirdName[0] + '.')) + " \"" + journal.Book.Name + "\"";
                    comboBox3.SelectedItem = journal.Employee.SecondName + ' ' + journal.Employee.FirstName + " " + journal.Employee.ThirdName;

                    groupBox1.Visible = true;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button4.Enabled = false;
                    isNewRecord = false;
                    index = dataGridView1.SelectedRows[0].Index;
                }
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.CustomFormat = "dd MMMM yyyy";
        }

        private void dateTimePicker2_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete))
            {
                dateTimePicker2.CustomFormat = " ";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var _journals = journals.FindAll(r => r.DateGive.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                    r.Book.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                    r.DateReturn.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                    r.Employee.ToString().ToLower().Contains(searchTextBox.Text.ToLower())||
                                                    r.Reader.ToString().ToLower().Contains(searchTextBox.Text.ToLower()));
            dataGridView1.DataSource = null;
            var _data = _journals
                .Select(b => new
                {
                    Id = b.Id,
                    DateGive = b.DateGive,
                    DateReturn = b.DateReturn,
                    Book = String.Join(", ", b.Book.Authors.Select(a => a.SecondName + ' ' + a.FirstName[0] + ". " + a.ThirdName[0] + '.')) + " \"" + b.Book.Name + "\"",
                    Reader = b.Reader.LibraryCard,
                    Employee = b.Employee.SecondName + ' ' + b.Employee.FirstName + " " + b.Employee.ThirdName
                }).ToList();

            dataGridView1.DataSource = _data;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["DateGive"].HeaderText = "Дата выдачи";
            dataGridView1.Columns["DateReturn"].HeaderText = "Дата возврата";
            dataGridView1.Columns["Book"].HeaderText = "Книга";
            dataGridView1.Columns["Reader"].HeaderText = "Номер читательского билета";
            dataGridView1.Columns["Employee"].HeaderText = "Библиотекарь";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            getRecords();
        }
    }
}
