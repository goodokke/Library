using Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library
{
    public partial class Readers : Form
    {
        bool isNewRecord = true;
        Reader reader;
        int index;
        private Journals formJournals;
        List<Reader> readers;
        public Readers(Journals formJournals = null)
        {
            this.formJournals = formJournals;
            if (formJournals != null)
            {
                formJournals.SelectedCellReader = -1;
            }
            InitializeComponent();
            CenterToScreen();
            getRecords();
        }

        public void getRecords()
        {
            using (DBContext DBContext = new DBContext())
            {
                readers = DBContext.Readers.ToList();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = readers;
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["SecondName"].HeaderText = "Фамилия";
                dataGridView1.Columns["FirstName"].HeaderText = "Имя";
                dataGridView1.Columns["ThirdName"].HeaderText = "Отчество";
                dataGridView1.Columns["LibraryCard"].HeaderText = "Читательский билет";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            searchTextBox.Enabled = false;
            groupBox1.Visible = true;
            button7.Enabled = false;
            button5.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            index = dataGridView1.RowCount;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (formJournals != null)
            {
                formJournals.SelectedCellReader = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                this.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0 && dataGridView1.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using (DBContext DBContext = new DBContext())
                    {
                        Reader reader = DBContext.Readers.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();
                        Journal journal = DBContext.Journals.Where(b => b.ReaderId == reader.Id).FirstOrDefault();
                        if (journal == null)
                        {
                            DBContext.Readers.Remove(reader);
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

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            groupBox1.Visible = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button4.Enabled = true;
            isNewRecord = true;
            button7.Enabled = true;
            button5.Enabled = true;
            searchTextBox.Enabled = true;
        }

        private int GenerateNumber()
        {
            Random random = new Random();
            int fiveDigitNumber = random.Next(10000, 100000);
            return fiveDigitNumber;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (DBContext DBContext = new DBContext())
            {
                if (textBox4.Text.Length > 0 
                    && textBox2.Text.Length > 0
                    && textBox3.Text.Length > 0)
                {
                    int number;
                    while (true)
                    {
                        number = GenerateNumber();
                        var test = DBContext.Readers.Where(x => x.LibraryCard == number);
                        if (!test.Any())
                        {
                            break;
                        }
                    }
                    
                    if (isNewRecord)
                    {
                        Reader reader = new Reader
                        {
                            SecondName = textBox2.Text,
                            FirstName = textBox3.Text,
                            ThirdName = textBox4.Text,
                            LibraryCard = number,
                        };
                        DBContext.Readers.Add(reader);
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        if (reader != null)
                        {
                            reader.SecondName = textBox2.Text;
                            reader.FirstName = textBox3.Text;
                            reader.ThirdName = textBox4.Text;
                            reader.LibraryCard = number;
                            DBContext.Readers.Update(reader);
                            DBContext.SaveChanges();
                            isNewRecord = true;
                        }
                    }

                    dataGridView1.Visible = true;
                    groupBox1.Visible = false;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button4.Enabled = true;
                    button7.Enabled = true;
                    button5.Enabled = true;
                    searchTextBox.Enabled = true;

                    getRecords();
                    dataGridView1.Rows[index].Selected = true;
                }
                else
                {
                    MessageBox.Show("Данные введены неверно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                using (DBContext DBContext = new DBContext())
                {
                    reader = DBContext.Readers.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();

                    dataGridView1.Visible = false;
                    textBox2.Text = reader.SecondName.ToString();
                    textBox3.Text = reader.FirstName.ToString();
                    textBox4.Text = reader.ThirdName.ToString();
                    groupBox1.Visible = true;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button4.Enabled = false;
                    button7.Enabled = false;
                    button5.Enabled = false;
                    searchTextBox.Enabled = false;
                    isNewRecord = false;
                    index = dataGridView1.SelectedRows[0].Index;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var _readers = readers.FindAll(r => r.LibraryCard.ToString().ToLower().Contains(searchTextBox.Text) ||
                                                r.SecondName.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                r.FirstName.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                r.ThirdName.ToString().ToLower().Contains(searchTextBox.Text.ToLower()));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _readers;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["SecondName"].HeaderText = "Фамилия";
            dataGridView1.Columns["FirstName"].HeaderText = "Имя";
            dataGridView1.Columns["ThirdName"].HeaderText = "Отчество";
            dataGridView1.Columns["LibraryCard"].HeaderText = "Читательский билет";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            getRecords();
        }
    }
}
