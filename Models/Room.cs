using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotBooks.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        [Range(100, 500)]
        public int RoomNo { get; set; }
        [Range(1, 10)]
        public string NoOfBed { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        [Range(0.01, 1000)]
        public double Price { get; set; }
        public string AboutRoom { get; set; }
        public string Image { get; set; }
        [Display(Name ="Hotel")]
        public int HotelId { get; set; }

        // add  refernece to the parent object
        public Hotel Hotel { get; set; }
        
    }
}
