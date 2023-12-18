using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string SecondName { get; set; }
        public string FirstName { get; set; }
        public string ThirdName { get; set; }

        public int PositionId { get; set; }
        public Position? Position { get; set; }

        public List<Journal> Journals { get; } = new();
    }
}
