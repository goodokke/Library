using Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library
{
    public partial class Authors : Form
    {
        bool isNewRecord = true;
        Author author;
        int index;
        private Books formBooks;

        private List<Author> _authors;
        public Authors(Books formBooks = null)
        {
            this.formBooks = formBooks;
            //if (formBooks != null)
            //{
            //    formBooks.SelectedCellAuthor = -1;
            //}
            InitializeComponent();
            CenterToScreen();
            getRecords();
        }

        public void getRecords()
        {
            using (DBContext DBContext = new DBContext())
            {
                _authors = DBContext.Authors.ToList();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = _authors;
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["SecondName"].HeaderText = "Фамилия";
                dataGridView1.Columns["FirstName"].HeaderText = "Имя";
                dataGridView1.Columns["ThirdName"].HeaderText = "Отчество";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            groupBox1.Visible = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            index = dataGridView1.RowCount;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (formBooks != null)
            {
                using (DBContext DBContext = new DBContext())
                {
                    formBooks.SelectedAuthor = DBContext.Authors.Where(a => a.Id == Int32.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString())).FirstOrDefault();
                }
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (DBContext DBContext = new DBContext())
            {
                if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0 && textBox3.Text.Length > 0)
                {
                    if (isNewRecord)
                    {
                        Author author = new Author
                        {
                            SecondName = textBox1.Text,
                            FirstName = textBox2.Text,
                            ThirdName = textBox3.Text,
                        };
                        DBContext.Authors.Add(author);
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        if (author != null)
                        {
                            author.SecondName = textBox1.Text;
                            author.FirstName = textBox2.Text;
                            author.ThirdName = textBox3.Text;
                            DBContext.Authors.Update(author);
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

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0 && dataGridView1.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using (DBContext DBContext = new DBContext())
                    {
                        bool flag = true;
                        Author author = DBContext.Authors.Where(g => g.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();
                        List<List<Author>> authors = DBContext.Books.Select(b => b.Authors).ToList();
                        foreach (var a in authors)
                        {
                            foreach (var _a in a)
                            {
                                if (author.Id == _a.Id) flag = false;
                            }
                        }
                        if (flag)
                        {
                            DBContext.Authors.Remove(author);
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                using (DBContext DBContext = new DBContext())
                {
                    author = DBContext.Authors.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();

                    dataGridView1.Visible = false;
                    textBox1.Text = author.SecondName.ToString();
                    textBox2.Text = author.FirstName.ToString();
                    textBox3.Text = author.ThirdName.ToString();
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
        /// Найти
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            var authors = _authors.FindAll(r => r.FirstName.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                    r.SecondName.ToLower().ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                    r.ThirdName.ToLower().ToString().ToLower().Contains(searchTextBox.Text.ToLower())
                                                    );
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = authors;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["SecondName"].HeaderText = "Фамилия";
            dataGridView1.Columns["FirstName"].HeaderText = "Имя";
            dataGridView1.Columns["ThirdName"].HeaderText = "Отчество";
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
