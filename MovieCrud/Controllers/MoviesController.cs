using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieCrud.Data;
using MovieCrud.Models;
using MovieCrud.View_Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieCrud.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            var viewmodel = new MovieFormViewModel()
            {
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View(model);
            }
            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select  Movie Poster ");
                return View(model);
            }
            var poster = files.FirstOrDefault();
            var allowExtentions = new List<string> { ".png", ".jpg" };
            if (!allowExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "only accept .png or .jpg ");
                return View(model);
            }
            if (poster.Length > 1048576)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "poster cannot be more than 1Mb ");
                return View(model);
            }
            using var datastream = new MemoryStream();
            await poster.CopyToAsync(datastream);

            var movie = new Movie()
            {
                Title = model.Title,
                Rate = model.Rate,
                GenreId = model.GenreId,
                StoreLine = model.StoreLine,
                Year = model.Year,
                Poster = datastream.ToArray()
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));


        }
          

    }
}
