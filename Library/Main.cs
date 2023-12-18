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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void издательствоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Publishers publishers = new Publishers();
            publishers.Show();
        }

        private void жанрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Genres genres = new Genres();
            genres.Show();
        }

        private void авторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Authors authors = new Authors();
            authors.Show();
        }

        private void книгаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Books books = new Books();
            books.Show();
        }

        private void должностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Positions positions = new Positions();
            positions.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Employees employees = new Employees();
            employees.Show();
        }

        private void читателиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Readers readers = new Readers();
            readers.Show();
        }

        private void выдачаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Journals journals = new Journals();
            journals.Show();
        }
    }
}
