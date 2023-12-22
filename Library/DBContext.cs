using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Library.Models;

namespace Library
{
    internal class DBContext : DbContext
    {
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Journal> Journals => Set<Journal>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<Publisher> Publishers => Set<Publisher>();
        public DbSet<Reader> Readers => Set<Reader>();

        //public DB() => Database.EnsureCreated();

        public DBContext()
        {
            //Database.EnsureDeleted(); // гарантируем, что бд удалена
            //Database.EnsureCreated(); // гарантируем, что бд будет создана
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=\"C:\\Users\\denis\\RiderProjects\\Library\\Library\\library.db\"");
        }
    }
}
