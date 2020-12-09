using HotBooks.Controllers;
using HotBooks.Data;
using HotBooks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HotBooksTests
{
    [TestClass]
    public class RoomsControllerTest
    {
        private ApplicationDbContext _context;
        List<Room> rooms = new List<Room>();
        RoomsController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // creating an in-memory bd
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new ApplicationDbContext(options);

            var hotel = new Hotel { Id = 58, Name = "Fifty eight" };
            _context.Hotels.Add(hotel);
            _context.SaveChanges();

            // creating mock data fro the assignment
            rooms.Add(new Room { Id = 42, RoomNo = 90, Price = 120.23, NoOfBed = "4", HotelId = 58 });
            rooms.Add(new Room { Id = 41, RoomNo = 91, Price = 130.23, NoOfBed = "5", HotelId = 58 });
            rooms.Add(new Room { Id = 43, RoomNo = 92, Price = 140.23, NoOfBed = "1", HotelId = 58 });

            foreach (var r in rooms)
            {
                _context.Rooms.Add(r);
            }
            _context.SaveChanges();
            controller = new RoomsController(_context);

        }
        // Creating Test Method
        // test methods for edit get 
        #region Edit
        [TestMethod]
        public void EditNoIdReturnError()
        {
            var result = (ViewResult)controller.Edit(null).Result;
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public void EditInvalidIdReturnError()
        {
            var result = (ViewResult)controller.Edit(44).Result;
            Assert.AreEqual("Error", result.ViewName);
        }
        [TestMethod]
        public void EditValidIdReturnView()
        {

            var result = (ViewResult)controller.Edit(41).Result;
            Assert.AreEqual("Edit", result.ViewName);
        }

        #endregion Edit

        #region Delete
        [TestMethod]
        public void DeleteNoIdReturnError()
        {
            var result = (ViewResult)controller.Delete(null).Result;
            Assert.AreEqual("Error", result.ViewName);
        }
        [TestMethod]
        public void DeleteInvalidIdReturnError()
        {
            var result = (ViewResult)controller.Delete(30).Result;
            Assert.AreEqual("Error", result.ViewName);

        }
        [TestMethod]
        public void DeleteValidIdReturnView()
        {
            var result = (ViewResult)controller.Delete(41).Result;
            Assert.AreEqual("Delete", result.ViewName);
        }
        #endregion Delete
    }
}



