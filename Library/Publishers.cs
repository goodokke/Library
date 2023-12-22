using Library.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Library
{
    public partial class Publishers : Form
    {
        bool isNewRecord = true;
        Publisher publisher;
        int index;
        private Books formBooks;
        List<Publisher> publishers;
        public Publishers(Books formBooks = null)
        {
            this.formBooks = formBooks;
            if (formBooks != null)
            {
                formBooks.SelectedCellPublisher = -1;
            }
            InitializeComponent();
            CenterToScreen();
            getRecords();
        }

        public void getRecords()
        {
            using (DBContext DBContext = new DBContext())
            {
                publishers = DBContext.Publishers.ToList();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = publishers;
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["Name"].HeaderText = "Наименование";
                dataGridView1.Columns["Address"].HeaderText = "Адрес";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            textBox1.Text = "";
            textBox2.Text = "";
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
                formBooks.SelectedCellPublisher = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (DBContext DBContext = new DBContext())
            {
                if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
                {
                    if (isNewRecord)
                    {
                        Publisher publisher = new Publisher
                        {
                            Name = textBox1.Text,
                            Address = textBox2.Text
                        };
                        DBContext.Publishers.Add(publisher);
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        if (publisher != null)
                        {
                            publisher.Name = textBox1.Text;
                            publisher.Address = textBox2.Text;
                            DBContext.Publishers.Update(publisher);
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

        private void button5_Click(object sender, EventArgs e)
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
                if (MessageBox.Show("Вы уверены удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using (DBContext DBContext = new DBContext())
                    {
                        Publisher publisher = DBContext.Publishers.Where(p => p.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();
                        Book book = DBContext.Books.Where(b => b.PublisherId == publisher.Id).FirstOrDefault();
                        if (book == null)
                        {
                            DBContext.Publishers.Remove(publisher);
                            DBContext.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Данная запись имеет связь с другими записями.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    publisher = DBContext.Publishers.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();

                    dataGridView1.Visible = false;
                    textBox1.Text = publisher.Name.ToString();
                    textBox2.Text = publisher.Address.ToString();
                    groupBox1.Visible = true;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button4.Enabled = false;
                    isNewRecord = false;
                    index = dataGridView1.SelectedRows[0].Index;
                }
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            var _publishers = publishers.FindAll(r => r.Name.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                r.Address.ToString().ToLower().Contains(searchTextBox.Text.ToLower()));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _publishers;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["Name"].HeaderText = "Наименование";
            dataGridView1.Columns["Address"].HeaderText = "Адрес";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            getRecords();
        }
    }
}