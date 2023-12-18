using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Journal
    {
        public int Id { get; set; }

        public DateTime DateGive { get; set; }
        public DateTime? DateReturn { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }

        public int ReaderId { get; set; }
        public Reader? Reader { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
