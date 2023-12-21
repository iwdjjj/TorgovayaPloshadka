using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TorgovayaPloshadka.Data;
using TorgovayaPloshadka.Models;

namespace TorgovayaPloshadka.Controllers
{
    public class CategoryProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CategoryProducts
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["CategoryName"] = _context.Categories.Select(d => new { id = d.CategoryId, CategoryName = d.CategoryName }).FirstOrDefault(d => d.id == id).CategoryName;
            ViewData["IdCategory"] = _context.Categories.Select(d => new { id = d.CategoryId, CategoryName = d.CategoryName }).FirstOrDefault(d => d.id == id).id;

            var applicationDbContext = _context.Products.Where(d => d.CategoryId == id).Include(p => p.Category).Include(p => p.Manufacturer).Include(p => p.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CategoryProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: CategoryProducts/Create
        [Authorize(Roles = "Administrator,Guest")]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = id;
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "FIO");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "FIO");
            return View();
        }

        // POST: CategoryProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator,Guest")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,CategoryId,ManufacturerId,SupplierId,Price,Weight,Proportions")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = product.CategoryId });
            }
            ViewData["CategoryId"] = product.CategoryId;
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "FIO", product.ManufacturerId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "FIO", product.SupplierId);
            return View(product);
        }

        // GET: CategoryProducts/Edit/5
        [Authorize(Roles = "Administrator,Guest")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["IdCategory"] = product.CategoryId;
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "FIO", product.ManufacturerId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "FIO", product.SupplierId);
            return View(product);
        }

        // POST: CategoryProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator,Guest")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,CategoryId,ManufacturerId,SupplierId,Price,Weight,Proportions")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = product.CategoryId });
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["IdCategory"] = product.CategoryId;
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "FIO", product.ManufacturerId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "FIO", product.SupplierId);
            return View(product);
        }

        // GET: CategoryProducts/Delete/5
        [Authorize(Roles = "Administrator,Guest")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: CategoryProducts/Delete/5
        [Authorize(Roles = "Administrator,Guest")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = product.CategoryId });
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
