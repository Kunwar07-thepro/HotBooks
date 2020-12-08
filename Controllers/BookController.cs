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
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
namespace HotBooks.Controllers
{
  
    public class BookController : Controller
    {
        // db connection
        private readonly ApplicationDbContext _context;

        private IConfiguration _iconfiguration;

        // constructor that accepts a db context object
        public BookController(ApplicationDbContext context, IConfiguration configuration)
        {
            // instantiate an instance of our db connection when this class is instantiated
            _context = context;
            _iconfiguration = configuration;
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

        // GET
        public IActionResult BookList()
        {
            
            var bookListRooms = _context.BookList.Include(c => c.Room).Where(c => c.CustomerId == HttpContext.Session.GetString("CustomerId")).ToList();
          
           
            
            return View(bookListRooms);
        }
       
        public IActionResult RemoveFromBookList(int id)
        {
            
            var BookListItem = _context.BookList.Find(id);

            
            if (BookListItem != null)
            {
                _context.BookList.Remove(BookListItem);
                _context.SaveChanges();
            }

            

            return RedirectToAction("BookList");
        }

       
        [Authorize]
        public IActionResult Checkout()
        {
            
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("Address,City,Province,PostalCode")] Models.Booking booking)
        {
            
            booking.OrderDate = DateTime.Now;
            booking.CustomerId = User.Identity.Name;
            booking.Total = (from c in _context.BookList
                           where c.CustomerId == HttpContext.Session.GetString("CustomerId")
                           select  c.Price).Sum();

            
            HttpContext.Session.SetObject("Booking", booking);

           
            return RedirectToAction("Payment");
        }

        [Authorize]
        public IActionResult Payment()
        {
            // get the order from the session
            var booking = HttpContext.Session.GetObject<Models.Booking>("Booking");

            // send the total to the view for display using the ViewBag
            ViewBag.Total = booking.Total;

            // read the Stripe Publishable Key from the configuration and put in ViewBag for the payment form
            ViewBag.PublishableKey = _iconfiguration.GetSection("Stripe")["PublishableKey"];

            // load the Payment view
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProcessPayment()
        {
            // get the order from the session variable
            var booking = HttpContext.Session.GetObject<Models.Booking>("Booking");

            // get the Stripe Secret Key from the configuration and pass it before we can create a new checkout session
            StripeConfiguration.ApiKey = _iconfiguration.GetSection("Stripe")["SecretKey"];

            // code will go here to create and submit Stripe payment charge
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                  "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      UnitAmount = (long?)(booking.Total * 100),
                      Currency = "cad",
                      ProductData = new SessionLineItemPriceDataProductDataOptions
                      {
                        Name = "HotBooks Bookings",
                      },
                    },
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = "https://" + Request.Host + "/Book/SaveBooking",
                CancelUrl = "https://" + Request.Host + "/Booking/BookList"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return Json(new { id = session.Id });
        }

        [Authorize]
        public IActionResult SaveBooking()
        {
            // get the order from the session variable
            var booking = HttpContext.Session.GetObject<Models.Booking>("Booking");

            // save as new order to the db
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // save the line items as new order details records
            var cartItems = _context.BookList.Where(c => c.CustomerId == HttpContext.Session.GetString("CustomerId"));
            foreach (var item in cartItems)
            {
                var bookingDetail = new BookingDetail
                {
                    RoomId = item.RoomId,
                   
                    Cost = item.Price,
                    BookId = booking.Id
                };

                _context.BookingDetails.Add(bookingDetail);
            }
            _context.SaveChanges();

           
            foreach (var item in cartItems)
            {
                _context.BookList.Remove(item);
            }
            _context.SaveChanges();

            // set the Session ItemCount variable (which shows in the navbar) back to zero
            HttpContext.Session.SetInt32("ItemCount", 0);

            
            return RedirectToAction("Details", "Bookings", new { @id = booking.Id });
        }
    }
}
