using System;
using System.Linq; //allows us to access query methods(FirstOrDefault)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;//allow us to use include
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext _context;

        public HomeController(MyContext myContext)
        {
            _context = myContext;
        }


        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            //get the UserId from session and store in userId
            int? userId = HttpContext.Session.GetInt32("UserId"); //? meaning we are not sure if there is a userId in session

            if (userId == null)
            {
                //no user present
                return RedirectToAction("LoginReg", "Users");
            }

            //put the userId in ViewBag to use in cshtml
            ViewBag.User = _context
            .Users
            .Find(userId);

            //extract all the weddings from ViewBag to display in cshtml
            ViewBag.Wedding = _context
            .Weddings
            .Include(wedding => wedding.PlanedBy)
            .Include(wedding => wedding.Rsvps)
            .ToList();

            return View();
        }

        [HttpGet("weddings/new")]
        public IActionResult NewWeddingPage()
        {
            int? userId = HttpContext.Session.GetInt32("UserId"); //get the logged in user

            if (userId == null)
            {
                return RedirectToAction("LoginReg", "Users"); //action: go to LoginReg page, controller: UsersController//
            }

            ViewBag.User = _context
                .Users
                .Find(userId);

            return View();
        }

        [HttpPost("weddings")]
        public IActionResult CreateWedding(Wedding weddingToCreate)//take the wedding form into and name it weddingToCreate
        {

            //validate the date to make sure it is in the future
            if (weddingToCreate.Date < DateTime.Now)
            {
                ModelState.AddModelError("Date", "You must specity a date in the future");
            }

            //if there is false info in the form, direct to the form page
            if (ModelState.IsValid)
            {

                //if the form is valid,get planed user in session
                //the form created all the necessary info except UserId, and we need to save the UserId from Session
                weddingToCreate.UserId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();

                //add and save changes to the DB
                _context.Add(weddingToCreate);
                _context.SaveChanges();

                // after we add the weddingToCreate in the _context and save the change, then the WeddingId will get created
                // to send to the wedding page
                return Redirect($"/weddings/{weddingToCreate.WeddingId}");
            }

            int? userId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();
            
            ViewBag.User = _context
                .Users
                .Find(userId);

            return View("NewWeddingPage");

        }

        //After successful create the wedding, the wedding detail goes to /weddings/{weddingToCreate.WeddingId}
        [HttpGet("weddings/{id}")]
        public IActionResult WeddingPage(int id)
        {
            //get the wedding to display
            ViewBag.Wedding = _context
                .Weddings
                //one to many relationship: each wedding I find, I want to return the movie that is PlanedBy
                .Include(wedding => wedding.PlanedBy)
                //populate the Rsvps first
                .Include(wedding => wedding.Rsvps)
                //and then populate the users
                .ThenInclude(rsvp => rsvp.User)
                .FirstOrDefault(wedding => wedding.WeddingId == id); //take the wedding, whose WeddingId == id

            //go to the wedding display page
            return View();
        }

        // User in session add a RSVP to the wedding
        [HttpPost("weddings/{id}/rsvps")]
        public IActionResult AddRsvp(int id)
        {
            var rsvpToAdd = new Rsvp();

            // Rsvp model has 3 fields(RsvpId, UserId, WeddingId)

            rsvpToAdd.UserId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault(); // add UserId
            rsvpToAdd.WeddingId = id; //add WeddingId

            _context.Add(rsvpToAdd);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        // User in session UN-RSVP to the wedding
        [HttpPost("weddings/{id}/rsvps/delete")]
        public IActionResult RemoveRsvp(int id)
        {
            //get the loggedin userId
            var userId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();
            //get the Rsvp that need to remove
            var rsvpToRemove = _context
                .Rsvps
            //get the first like where the wedding id matches the id we took in the form, and the user id in session matches the user id
                .FirstOrDefault(rsvp => rsvp.WeddingId == id && rsvp.UserId == userId);

            _context.Rsvps.Remove(rsvpToRemove);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
            }
        
        // User who posted can delete
        [HttpPost("weddings/{id}/delete")]
        public IActionResult DeleteWedding(int id)
        {
            //get the wedding that need to remove
            var weddingToDelete = _context
                .Weddings
            //get the first like where the movie id matches the id we took in the form, and the user id in session matches the user id
                .Find(id);

            _context.Weddings.Remove(weddingToDelete);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }
    }
}
