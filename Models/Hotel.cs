using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotBooks.Models
{
    public class Hotel
    {

        public int Id { get; set; }

        [Required]

        public string Name { get; set; }
        public string Address { get; set; }
        public string Discription { get; set; }
        public string Image { get; set; }
        public List<Room> Rooms { get; set; }

    }
}
