using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotBooks.Models
{
    public class BookingDetail
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int RoomId { get; set; }
        public double Cost { get; set; }

        // parent refs
        public Booking Booking { get; set; }
        public Room Room { get; set; }
    }
}
