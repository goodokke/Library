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
    public partial class Positions : Form
    {
        bool isNewRecord = true;
        Position position;
        int index;
        private Employees formEmployees;
        public Positions(Employees formEmployees = null)
        {
            this.formEmployees = formEmployees;
            if (formEmployees != null)
            {
                formEmployees.SelectedCellPosition = -1;
            }
            InitializeComponent();
            getRecords();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (formEmployees != null)
            {
                formEmployees.SelectedCellPosition = e.RowIndex;
                this.Close();
            }
        }

        public void getRecords()
        {
            using (DBContext DBContext = new DBContext())
            {
                List<Position> positions = DBContext.Positions.ToList();

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = positions;
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
                        Position position = DBContext.Positions.Where(p => p.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();
                        Employee employee = DBContext.Employees.Where(b => b.PositionId == position.Id).FirstOrDefault();
                        if (employee == null)
                        {
                            DBContext.Positions.Remove(position);
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
                if (textBox1.Text.Length > 0)
                {
                    if (isNewRecord)
                    {
                        Position position = new Position
                        {
                            Name = textBox1.Text,
                        };
                        DBContext.Positions.Add(position);
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        if (position != null)
                        {
                            position.Name = textBox1.Text;
                            DBContext.Positions.Update(position);
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
                    position = DBContext.Positions.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();

                    dataGridView1.Visible = false;
                    textBox1.Text = position.Name.ToString();
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
