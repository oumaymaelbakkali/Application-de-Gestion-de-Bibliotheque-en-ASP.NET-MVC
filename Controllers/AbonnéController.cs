using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using biblio.Models;

namespace biblio.Controllers
{
    public class AbonnéController : Controller
    {
        private readonly BiblioContext _context;

        public AbonnéController()
        {
            _context = new BiblioContext();
        }

        // GET: Abonné
        public async Task<IActionResult> Index()
        {
              return _context.Abonnés != null ? 
                          View(await _context.Abonnés.ToListAsync()) :
                          Problem("Entity set 'BiblioContext.Abonnés'  is null.");
        }

        // GET: Abonné/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Abonnés == null)
            {
                return NotFound();
            }

            var abonné = await _context.Abonnés
                .FirstOrDefaultAsync(m => m.Id == id);
            if (abonné == null)
            {
                return NotFound();
            }

            return View(abonné);
        }

        // GET: Abonné/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Abonné/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Prénom")] Abonné abonné)
        {
            if (ModelState.IsValid)
            {
                _context.Add(abonné);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(abonné);
        }

        // GET: Abonné/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Abonnés == null)
            {
                return NotFound();
            }

            var abonné = await _context.Abonnés.FindAsync(id);
            if (abonné == null)
            {
                return NotFound();
            }
            return View(abonné);
        }

        // POST: Abonné/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Prénom")] Abonné abonné)
        {
            if (id != abonné.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(abonné);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbonnéExists(abonné.Id))
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
            return View(abonné);
        }

        // GET: Abonné/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Abonnés == null)
            {
                return NotFound();
            }

            var abonné = await _context.Abonnés
                .FirstOrDefaultAsync(m => m.Id == id);
            if (abonné == null)
            {
                return NotFound();
            }

            return View(abonné);
        }

        // POST: Abonné/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Abonnés == null)
            {
                return Problem("Entity set 'BiblioContext.Abonnés'  is null.");
            }
            var abonné = await _context.Abonnés.FindAsync(id);
            var isLivreEmprunte = await _context.Emprunts.AnyAsync(e => e.AbonnéId == id);

            if (isLivreEmprunte)
            {

                return BadRequest("Vous ne pouvez pas supprimer ce Abonne car il est emprunté Un livre .");
            }
            if (abonné != null)
            {
                _context.Abonnés.Remove(abonné);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AbonnéExists(int id)
        {
          return (_context.Abonnés?.Any(e => e.Id == id)).GetValueOrDefault();
        }



        public IActionResult LivreEmprunte(int subscriberId)
        {
            var borrowedBooks = _context.Emprunts
                .Include(e => e.Livre) 
                .Where(e => e.AbonnéId == subscriberId)
                .Select(e => e.Livre)
                .ToList();

            var abonne = _context.Abonnés
                .FirstOrDefault(a => a.Id == subscriberId);

            if (abonne == null)
            {
                return NotFound();
            }

            ViewData["SubscriberName"] = $"{abonne.Nom} {abonne.Prénom}";

            return View(borrowedBooks);
        }

    }
}

