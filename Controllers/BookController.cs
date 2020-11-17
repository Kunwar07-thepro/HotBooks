using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotBooks.Data;
using Microsoft.AspNetCore.Mvc;

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

            // pass the categories data to the view for display to the shopper
            return View(hotels);
        }
    }
}
