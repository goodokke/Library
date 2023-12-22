using Library.Models;
using Microsoft.EntityFrameworkCore;
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
    public partial class Employees : Form
    {
        bool isNewRecord = true;
        Employee employee;
        int index;
        private Journals formJournals;
        public int SelectedCellPosition = -1;

        private List<Employee> _employees;
        public Employees(Journals formJournals = null)
        {
            this.formJournals = formJournals;
            if (formJournals != null)
            {
                formJournals.SelectedCellReader = -1;
            }
            InitializeComponent();
            CenterToScreen();
            getRecords();
            getPositions();
        }

        public void getRecords()
        {
            using (DBContext DBContext = new DBContext())
            {
                _employees = DBContext.Employees.Include(e => e.Position).ToList();

                dataGridView1.DataSource = null;

                var data = _employees
                    .Select(b => new
                    {
                        Id = b.Id,
                        SecondName = b.SecondName,
                        FirstName = b.FirstName,
                        ThirdName = b.ThirdName,
                        Position = b.Position.Name,
                    }).ToList();

                dataGridView1.DataSource = data;
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["SecondName"].HeaderText = "Фамилия";
                dataGridView1.Columns["FirstName"].HeaderText = "Имя";
                dataGridView1.Columns["ThirdName"].HeaderText = "Отчество";
                dataGridView1.Columns["Position"].HeaderText = "Должность";
            }
        }

        void getPositions()
        {
            comboBox2.Items.Clear();
            comboBox2.ResetText();

            List<Position> positions = new List<Position>();

            using (DBContext db = new DBContext())
            {
                positions = db.Positions.ToList();
            }

            foreach (Position position in positions)
            {
                comboBox2.Items.Add(position.Name);
            }
            comboBox2.SelectedIndex = SelectedCellPosition;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (formJournals != null)
            {
                formJournals.SelectedCellEmployee = e.RowIndex;
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox2.ResetText();
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

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0 && dataGridView1.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    using (DBContext DBContext = new DBContext())
                    {
                        Employee employee = DBContext.Employees.Where(e => e.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).FirstOrDefault();
                        Journal journal = DBContext.Journals.Where(b => b.EmployeeId == employee.Id).FirstOrDefault();
                        if (journal == null)
                        {
                            DBContext.Employees.Remove(employee);
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
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (DBContext DBContext = new DBContext())
            {
                Position? position = DBContext.Positions.Where(p => p.Name == comboBox2.Text).FirstOrDefault();

                if (position != null && textBox1.Text.Length > 0 &&
                    textBox2.Text.Length > 0 && textBox3.Text.Length > 0)
                {
                    if (isNewRecord)
                    {
                        Employee employee = new Employee
                        {
                            SecondName = textBox1.Text,
                            FirstName = textBox2.Text,
                            ThirdName = textBox3.Text,
                            PositionId = position!.Id
                        };
                        DBContext.Employees.Add(employee);
                        DBContext.SaveChanges();
                    }
                    else
                    {
                        if (employee != null)
                        {
                            employee.SecondName = textBox1.Text;
                            employee.FirstName = textBox2.Text;
                            employee.ThirdName = textBox3.Text;
                            employee.PositionId = position!.Id;
                            employee.Position = position;
                            DBContext.Employees.Update(employee);
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
            Positions positions = new Positions(this);
            positions.ShowDialog();
            getPositions();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                using (DBContext DBContext = new DBContext())
                {
                    employee = DBContext.Employees.Where(r => r.Id == Convert.ToInt32(dataGridView1[0, dataGridView1.SelectedRows[0].Index].Value)).Include(e => e.Position).FirstOrDefault();

                    dataGridView1.Visible = false;
                    textBox1.Text = employee.SecondName.ToString();
                    textBox2.Text = employee.FirstName.ToString();
                    textBox3.Text = employee.ThirdName.ToString();
                    comboBox2.SelectedItem = employee.Position.Name;
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
            var employees = _employees.FindAll(r => r.Position.Name.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                    r.FirstName.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                    r.SecondName.ToString().ToLower().Contains(searchTextBox.Text.ToLower()) ||
                                                    r.ThirdName.ToString().ToLower().Contains(searchTextBox.Text.ToLower())
                                                );

            dataGridView1.DataSource = null;

            var data = employees
                .Select(b => new
                {
                    Id = b.Id,
                    SecondName = b.SecondName,
                    FirstName = b.FirstName,
                    ThirdName = b.ThirdName,
                    Position = b.Position.Name,
                }).ToList();

            dataGridView1.DataSource = data;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["SecondName"].HeaderText = "Фамилия";
            dataGridView1.Columns["FirstName"].HeaderText = "Имя";
            dataGridView1.Columns["ThirdName"].HeaderText = "Отчество";
            dataGridView1.Columns["Position"].HeaderText = "Должность";
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
