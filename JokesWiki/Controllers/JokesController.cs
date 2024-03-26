using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JokesWiki.Data;
using JokesWiki.Models;
using JokesWiki.Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using NuGet.Versioning;

namespace JokesWiki.Controllers
{
    public class JokesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JokesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Jokes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Joke.ToListAsync());
        }

        // GET: Jokes
        public async Task<IActionResult> Index_WrongUser()
        {
            ViewData["WrongUser"] = false;
            return View("Index",await _context.Joke.ToListAsync());
        }

        public async Task<IActionResult> UserIndex()
        {
            var model = await _context.Joke
                                .Where(a => a.ApplicationUserID == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                                .ToListAsync();
            return View(model);
        }

        // GET: Search Jokes
        public IActionResult SearchJokes()
        {
            return View();
        }

        // POST: Show search result
        public async Task<IActionResult> SearchJokesResults(String SearchQuery)
        {
            return View(await _context.Joke.Where( j => j.JokeQuestion.Contains(SearchQuery)).ToListAsync());
        }

        // GET: Jokes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.id == id);

            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // GET: Jokes/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jokes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JokeViewModel jokeVM)
        {
            if (ModelState.IsValid)
            {
                var joke = new Joke()
                {
                    JokeAnswer = jokeVM.JokeAnswer,
                    JokeQuestion = jokeVM.JokeQuestion,
                    ApplicationUserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    UserEmail = User.FindFirstValue(ClaimTypes.Email)
                };

                _context.Add(joke);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(jokeVM);
        }

        // GET: Items


        [Authorize]
        // GET: Jokes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke.FindAsync(id);
            var current_user_id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (joke == null)
            {
                return NotFound();
            }
            if (current_user_id != joke.ApplicationUserID)
            {
                return RedirectToAction(nameof(Index_WrongUser));
            }

            return View(joke);
        }

        // POST: Jokes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JokeViewModel jokeVM)
        {

            if (!ModelState.IsValid)
            {
                return View(jokeVM);
            }

            var joke = _context.Joke.Find(id);

            if (joke == null)
            {
                return NotFound();
            }


            try
            {   
                joke.JokeQuestion = jokeVM.JokeQuestion;
                joke.JokeAnswer = jokeVM.JokeAnswer;
                _context.Update(joke);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JokeExists(joke.id))
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

        // GET: Jokes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.id == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // POST: Jokes/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var joke = await _context.Joke.FindAsync(id);
            if (joke != null)
            {
                _context.Joke.Remove(joke);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JokeExists(int id)
        {
            return _context.Joke.Any(e => e.id == id);
        }
    }
}
