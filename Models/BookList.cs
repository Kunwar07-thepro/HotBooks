using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotBooks.Models
{
    public class BookList
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public double Price { get; set; }

        // parent object ref
        public Room Room { get; set; }
    }
}
