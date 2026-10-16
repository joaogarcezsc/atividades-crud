using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using appPetShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace appPetShop.Controllers
{
    [Authorize]
    public class ReservaServicoController : Controller
    {
        private readonly DbPetShopContext _context;

        public ReservaServicoController(DbPetShopContext context)
        {
            _context = context;
        }

        // GET: ReservaServico
        public async Task<IActionResult> Index()
        {
            var dbPetShopContext = _context.ReservaServicos.Include(r => r.Cliente).Include(r => r.Veterinario);
            return View(await dbPetShopContext.ToListAsync());
        }

        // GET: ReservaServico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservaServico = await _context.ReservaServicos
                .Include(r => r.Cliente)
                .Include(r => r.Veterinario)
                .FirstOrDefaultAsync(m => m.Codigo == id);

            if (reservaServico == null)
            {
                return NotFound();
            }

            return View(reservaServico);
        }

        // GET: ReservaServico/Create
        public IActionResult Create()
        {
            ViewData["VeterinarioId"] = new SelectList(_context.Veterinarios, "Codigo", "Nome");
            return View();
        }

        // POST: ReservaServico/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DataHora,Valor,StatusServico,VeterinarioId")] ReservaServico reservaServico)
        {
            // Captura o ID do cliente logado pelas Claims do Cookie
            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(clienteIdClaim))
            {
                reservaServico.ClienteId = int.Parse(clienteIdClaim);
            }

            // Define o status padrão
            if (string.IsNullOrEmpty(reservaServico.StatusServico))
            {
                reservaServico.StatusServico = "Agendada";
            }

            // Remove a validação dos objetos relacionais para não travar o ModelState.IsValid no .NET 9
            ModelState.Remove("Cliente");
            ModelState.Remove("Veterinario");
            ModelState.Remove("ClienteId");
            ModelState.Remove("StatusServico");

            if (ModelState.IsValid)
            {
                _context.Add(reservaServico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["VeterinarioId"] = new SelectList(_context.Veterinarios, "Codigo", "Nome", reservaServico.VeterinarioId);
            return View(reservaServico);
        }

        // GET: ReservaServico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservaServico = await _context.ReservaServicos.FindAsync(id);
            if (reservaServico == null)
            {
                return NotFound();
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Codigo", "Nome", reservaServico.ClienteId);
            ViewData["VeterinarioId"] = new SelectList(_context.Veterinarios, "Codigo", "Nome", reservaServico.VeterinarioId);
            return View(reservaServico);
        }

        // POST: ReservaServico/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,DataHora,Valor,StatusServico,ClienteId,VeterinarioId")] ReservaServico reservaServico)
        {
            if (id != reservaServico.Codigo)
            {
                return NotFound();
            }

            // Remove a validação dos objetos relacionais
            ModelState.Remove("Cliente");
            ModelState.Remove("Veterinario");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservaServico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaServicoExists(reservaServico.Codigo))
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

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Codigo", "Nome", reservaServico.ClienteId);
            ViewData["VeterinarioId"] = new SelectList(_context.Veterinarios, "Codigo", "Nome", reservaServico.VeterinarioId);
            return View(reservaServico);
        }

        // GET: ReservaServico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservaServico = await _context.ReservaServicos
                .Include(r => r.Cliente)
                .Include(r => r.Veterinario)
                .FirstOrDefaultAsync(m => m.Codigo == id);

            if (reservaServico == null)
            {
                return NotFound();
            }

            return View(reservaServico);
        }

        // POST: ReservaServico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservaServico = await _context.ReservaServicos.FindAsync(id);
            if (reservaServico != null)
            {
                _context.ReservaServicos.Remove(reservaServico);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaServicoExists(int id)
        {
            return _context.ReservaServicos.Any(e => e.Codigo == id);
        }
    }
}