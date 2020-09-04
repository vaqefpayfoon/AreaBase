using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AreaBase.Data;
using AreaBase.Models;
using AreaBase.Models.ViewModel;
using AreaBase.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AreaBase.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Get INDEX
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategories.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategories.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "خطا:زیر گروه برای فهرست " + doesSubCategoryExists.First().Category.Name + "  موجود می باشد لطفا نام دیگری انتخاب نمایید";
                }
                else
                {
                    _db.SubCategories.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }


        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategories
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategories.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategories.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "خطا:زیر گروه برای فهرست " + doesSubCategoryExists.First().Category.Name + "  موجود می باشد لطفا نام دیگری انتخاب نمایید";
                }
                else
                {
                    var subCatFromDb = await _db.SubCategories.FindAsync(model.SubCategory.Id);
                    subCatFromDb.Name = model.SubCategory.Name;

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            //modelVM.SubCategory.Id = id;
            return View(modelVM);
        }

        //GET Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategories.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        //GET Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategories.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _db.SubCategories.SingleOrDefaultAsync(m => m.Id == id);
            _db.SubCategories.Remove(subCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}