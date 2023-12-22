using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Library
{
    public partial class Genres : Form
    {
        bool isNewRecord = true;
        Genre genre;
        int index;
        private Books formBooks;
        private List<Genre> _genres;
        public Genres(Books formBooks = null)
        {
            this.formBooks = formBooks;
            if (formBooks != null)
            {
                formBooks.SelectedCellGenre = -1;
            }
            InitializeComponent();
            CenterToScreen();
            getRecords();
        }

        public void getRecords()
        {
            using (DBContext DBContext = new DBContext())
            {
                _genres = DBContext.Genres.ToList();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = _genres;
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["Name"].HeaderText = "Наименование";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            textBox1.Text = "";
            groupBox1.Visible = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            index = dataGridView1.RowCount;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0 && dataGridView1.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using (DBContext DBContext = new DBContext())
                    {
                        Genre genre = DBContext.Genres.Where(g => g.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();
                        Book book = DBContext.Books.Where(b => b.GenreId == genre.Id).FirstOrDefault();
                        if (book == null)
                        {
                            DBContext.Genres.Remove(genre);
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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (formBooks != null)
            {
                formBooks.SelectedCellGenre = e.RowIndex;
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (DBContext DBContext = new DBContext())
            {
                if (textBox1.Text.Length > 0)
                {
                    if (isNewRecord)
                    {
                        Genre genre = new Genre
                        {
                            Name = textBox1.Text,
                        };
                        DBContext.Genres.Add(genre);
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        if (genre != null)
                        {
                            genre.Name = textBox1.Text;
                            DBContext.Genres.Update(genre);
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

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            groupBox1.Visible = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button4.Enabled = true;
            isNewRecord = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                using (DBContext DBContext = new DBContext())
                {
                    genre = DBContext.Genres.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();

                    dataGridView1.Visible = false;
                    textBox1.Text = genre.Name.ToString();
                    groupBox1.Visible = true;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button4.Enabled = false;
                    isNewRecord = false;
                    index = dataGridView1.SelectedRows[0].Index;
                }
            }
        }

        /// <summary>
        /// Поиск
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            var genres = _genres.FindAll(r => r.Name.ToString().ToLower().Contains(searchTextBox.Text.ToLower())
                                                );

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = genres;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["Name"].HeaderText = "Наименование";
        }

        /// <summary>
        /// Очистить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            getRecords();
        }
    }
}
