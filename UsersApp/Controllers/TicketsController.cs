using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UsersApp.Data;
using UsersApp.Models;



namespace UsersApp.Controllers
{
    public class TicketsController(AppDbContext context, IWebHostEnvironment environment) : Controller
    {
        private readonly AppDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;
      

        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket, IFormFile file)
        {
            if (ModelState.IsValid)

            {
                return View(ticket);
            }

            if (file != null && file.Length > 0)

            {
                
                string uploads = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
               
                    var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    var safeFileName = originalFileName.Replace(" ", "_") + "_" + Guid.NewGuid() + extension;


                var filePath = Path.Combine(uploads, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }



                    ticket.FilePath = "/uploads/" + file.FileName;



                }
                else
                {
                    // ✅ No file uploaded, just continue saving ticket
                    ticket.FilePath = null;
                }

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Tickets");
            }
            

        

        public IActionResult Index()
        {
            var tickets = _context.Tickets.ToList();
            return View(tickets);
        }
    }
}
