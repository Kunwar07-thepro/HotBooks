using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotBooks.Data;
using HotBooks.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace HotBooks.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Rooms.Include(r => r.Hotel).OrderBy(R => R.RoomNo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Rooms/Details/5
        [AllowAnonymous]
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

        // GET: Rooms/Create
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hotels.OrderBy(o => o.Name), "Id", "Name");
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoomNo,NoOfBed,Price,AboutRoom,HotelId")] Room room, IFormFile Image )
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    
                    var temporyPath = Path.GetTempFileName();

                    
                    var uniqueName = Guid.NewGuid() + "-" + Image.FileName;

                    
                    var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\img\\room-uploads\\" + uniqueName;

                    // use a stream to copy the file to the destination folder w/a unique name
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                        await Image.CopyToAsync(stream);

                    // set the product Image property equal the new unique image file name
                    room.Image = uniqueName;
                }
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", room.HotelId);
            return View(room);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                //return NotFound();
                return View("Error");
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                // return NotFound();
                return View("Error");
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels.OrderBy(h => h.Name), "Id", "Name", room.HotelId);
            return View("Edit",room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomNo,NoOfBed,Price,AboutRoom,HotelId")] Room room, IFormFile Image , string CurrentImage)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Image != null)
                    {

                        var temporyPath = Path.GetTempFileName();


                        var uniqueName = Guid.NewGuid() + "-" + Image.FileName;


                        var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\img\\room-uploads\\" + uniqueName;

                        // use a stream to copy the file to the destination folder w/a unique name
                        using (var stream = new FileStream(uploadPath, FileMode.Create))
                            await Image.CopyToAsync(stream);

                        // set the product Image property equal the new unique image file name
                        room.Image = uniqueName;
                    }
                    else
                    {
                        room.Image = CurrentImage;
                    }
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels.OrderBy(h => h.Name), "Id", "Name", room.HotelId);
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                //return NotFound();
                return View("Error");
            }

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                //return NotFound();
                return View("Error");
            }

            return View("Delete", room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
