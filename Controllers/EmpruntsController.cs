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
    public class EmpruntsController : Controller
    {
        private readonly BiblioContext _context;

        public EmpruntsController()
        {
            _context = new BiblioContext();
        }

        // GET: Emprunts
        public async Task<IActionResult> Index()
        {
            var biblioContext = _context.Emprunts.Include(e => e.Abonné).Include(e => e.Livre);
            return View(await biblioContext.ToListAsync());
        }

        // GET: Emprunts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Emprunts == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts
                .Include(e => e.Abonné)
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprunt == null)
            {
                return NotFound();
            }

            return View(emprunt);
        }

        // GET: Emprunts/Create
        public IActionResult Create()
        {
            
            ViewBag.AbonnéId = new SelectList(_context.Abonnés, "Id", "CombinedNomPrenom");
            ViewBag.LivreId = new SelectList(_context.Livres.Where(l => l.EstEmprunte == false), "Id", "CombinedTitreAuteur");


            return View();
        }

        // POST: Emprunts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        // GET: Emprunts/Edit/5
        public async Task<IActionResult> Create([Bind("Id,LivreId,AbonnéId,DateEmprunt,DateRetour")] Emprunt emprunt)
        {
            // Chercher dans la table le nombre d'emprunts pour l'abonné
            var nombreEmpruntsAbonne = _context.Emprunts.Count(e => e.AbonnéId == emprunt.AbonnéId);

            // Vérifier si l'abonné a atteint le nombre maximal d'emprunts (2)
            if (nombreEmpruntsAbonne >= 2)
            {
                ModelState.AddModelError(""," Ce abonné a atteint le nombre maximal d'emprunts (2)");
            }

            if (ModelState.IsValid)
            {
                var dateRetourMax = DateTime.Now.AddDays(14);

                var newEmprunt = new Emprunt
                {
                    LivreId = emprunt.LivreId,
                    AbonnéId = emprunt.AbonnéId,
                    DateEmprunt = DateTime.Now,
                    DateRetour = dateRetourMax
                };

                _context.Emprunts.Add(newEmprunt);

                // Mettre à jour le statut du livre
                var livre = _context.Livres.FirstOrDefault(l => l.Id == emprunt.LivreId);
                if (livre != null)
                {
                    livre.EstEmprunte = true;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AbonnéId = new SelectList(_context.Abonnés, "Id", "CombinedNomPrenom", emprunt.AbonnéId);
            ViewBag.LivreId = new SelectList(_context.Livres, "Id", "CombinedTitreAuteur", emprunt.LivreId);
            return View(emprunt);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Livres == null)
            {
                return NotFound();
            }

            var livre = await _context.Emprunts.FindAsync(id);
            if (livre == null)
            {
                return NotFound();
            }
            return View(livre);
        }
      
        // POST: Emprunts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      
        // GET: Emprunts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Emprunts == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts
                .Include(e => e.Abonné)
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprunt == null)
            {
                return NotFound();
            }

            return View(emprunt);
        }

        // POST: Emprunts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Emprunts == null)
            {
                return Problem("Entity set 'BiblioContext.Emprunts' is null.");
            }

            var emprunt = await _context.Emprunts.FindAsync(id);

            if (emprunt != null)
            {
                // Récupérer le livre associé à l'emprunt
                var livre = await _context.Livres.FindAsync(emprunt.LivreId);

                if (livre != null)
                {
                    // Mettre à jour le statut du livre
                    livre.EstEmprunte = false;
                    _context.Livres.Update(livre);
                }

                // Supprimer l'emprunt
                _context.Emprunts.Remove(emprunt);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        private bool EmpruntExists(int id)
        {
          return (_context.Emprunts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
