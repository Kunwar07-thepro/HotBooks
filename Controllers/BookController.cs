using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotBooks.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotBooks.Controllers
{
    public class BookController : Controller
    {
        // db connection
        private readonly ApplicationDbContext _context;

        // constructor that accepts a db context object
        public BookController(ApplicationDbContext context)
        {
            // instantiate an instance of our db connection when this class is instantiated
            _context = context;
        }
        public IActionResult Index()
        {
            var hotels = _context.Hotels.OrderBy(c => c.Name).ToList();

            
            return View(hotels);

        }
        public IActionResult Browse(int id)
        {
            
            var rooms = _context.Rooms.Include(r => r.Hotel).Where(r => r.HotelId == id).OrderBy(r => r.RoomNo).ToList();

            
            ViewBag.Hotel = rooms[00].Hotel.Name;
            

           
            return View(rooms);
        }
    }
}
