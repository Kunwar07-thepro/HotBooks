using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotBooks.Data;
using HotBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
       // [Authorize(Roles = "Customer")]
       // [Authorize(Roles = "Administrator")]
        public IActionResult ShowRooms(int id)
        {
            
            var rooms = _context.Rooms.Include(r => r.Hotel).Where(r => r.HotelId == id).OrderBy(r => r.RoomNo).ToList();

            
            ViewBag.Hotel = rooms[0].Hotel.Name;
            

           
            return View(rooms);
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult AddToBookList(int RoomId , int Quantity)
        {
                                                                                   
            var price = _context.Rooms.Find(RoomId).Price;

            // set the customer
            var customerId = GetCustomerId();
            var bookListItem = _context.BookList.SingleOrDefault(c => c.RoomId == RoomId && c.CustomerId == customerId);

           
                
                var bookList = new BookList
                {
                    RoomId = RoomId,
                    Price = price,
                    CustomerId = customerId,
                    DateCreated = DateTime.Now
                };

               
                _context.BookList.Add(bookList);
                _context.SaveChanges();
            
            

           
            return RedirectToAction("BookList");
        }

        // check session for existing Session ID.  If none exists, first create it then send it back.
        private string GetCustomerId()
        {
            // check if there is already a CustomerId session variable
            if (HttpContext.Session.GetString("CustomerId") == null)
            {
                
                HttpContext.Session.SetString("CustomerId", Guid.NewGuid().ToString());
            }

            return HttpContext.Session.GetString("CustomerId");
        }

        // GET: /Shop/BookList
        public IActionResult BookList()
        {
            // get items in current user's BookList
            var bookListRooms = _context.BookList.Include(c => c.Room).Where(c => c.CustomerId == HttpContext.Session.GetString("CustomerId")).ToList();
          
           
            // display a view and pass the items for display
            return View(bookListRooms);
        }
        // GET: /Shop/RemoveFromCBookList/3
        public IActionResult RemoveFromBookList(int id)
        {
            // find the item with this PK value
            var BookListItem = _context.BookList.Find(id);

            // delete record from BookList table
            if (BookListItem != null)
            {
                _context.BookList.Remove(BookListItem);
                _context.SaveChanges();
            }

            

            return RedirectToAction("BookList");
        }

        // GET: /Shop/Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            // load checkout form
            return View();
        }

    }
}
