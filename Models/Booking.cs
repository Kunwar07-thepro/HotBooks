﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotBooks.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public double Total { get; set; }

        // child ref
        public List<BookingDetail> OrderDetails { get; set; }
    }
}
