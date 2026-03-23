using Bai_Cuoi_Ky.Data; 
using Bai_Cuoi_Ky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.EntityFrameworkCore; 

namespace Bai_Cuoi_Ky.Controllers 
{
    public class ProductController : Controller 
    {
        private readonly ApplicationDbContext _context; 

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId, string search,
     List<string> material = null, List<string> gender = null,
     List<string> price = null, string sort = null)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search));

            // Lọc chất liệu
            if (material != null && material.Any())
                products = products.Where(p => material.Contains(p.Material));

            // Lọc giới tính
            if (gender != null && gender.Any())
                products = products.Where(p => gender.Contains(p.Gender));

            // Sắp xếp
            products = sort switch
            {
                "price-asc" => products.OrderBy(p => p.Price),
                "price-desc" => products.OrderByDescending(p => p.Price),
                _ => products
            };

            // Lọc giá (phải ToList trước)
            var productList = await products.ToListAsync();
            if (price != null && price.Any())
            {
                productList = productList.Where(p => price.Any(range => {
                    var parts = range.Split('-');
                    var min = decimal.Parse(parts[0]);
                    var max = decimal.Parse(parts[1]);
                    return p.Price >= min && p.Price <= max;
                })).ToList();
            }

            ViewBag.CurrentCategory = categoryId;
            ViewBag.Search = search;

            return View(productList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product, IFormFile? ImageFile1, IFormFile? ImageFile2)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                if (ImageFile1 != null && ImageFile1.Length > 0)
                    product.ImageUrl = await ConvertToBase64DataUri(ImageFile1);

                if (ImageFile2 != null && ImageFile2.Length > 0)
                    product.ImageUrl2 = await ConvertToBase64DataUri(ImageFile2);

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        private async Task<string> ConvertToBase64DataUri(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            var contentType = file.ContentType ?? "image/jpeg";
            return $"data:{contentType};base64,{base64}";
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id); 
            if (product == null) return NotFound();

            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound(); 

            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product); 
                    await _context.SaveChangesAsync(); 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index)); 
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product); 
                await _context.SaveChangesAsync(); 
            }
            return RedirectToAction(nameof(Index)); 
        }

        public async Task<IActionResult> IndexAdmin()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            ViewBag.TotalProducts = products.Count;

            return View(products);
        }
    }
}