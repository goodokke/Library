using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int YearPublish { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        public int PublisherId { get; set; }
        public Publisher? Publisher { get; set; }

        public List<Author> Authors { get; set; } = new();

        public List<Journal> Journals { get; } = new();
    }
}
