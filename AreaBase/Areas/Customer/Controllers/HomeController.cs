using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AreaBase.Models;
using AreaBase.Data;
using AreaBase.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AreaBase.Utility;
using Microsoft.AspNetCore.Authorization;

namespace AreaBase.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Categories.ToListAsync(),
                Coupon = await _db.Coupons.Where(c => c.IsActive == true).ToListAsync()

            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }


            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemFromDb = await _db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart()
            {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.Id
            };

            return View(cartObj);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCarts.Where(c => c.ApplicationUserId == CartObject.ApplicationUserId
                                                && c.MenuItemId == CartObject.MenuItemId).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await _db.ShoppingCarts.AddAsync(CartObject);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + CartObject.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCarts.Where(c => c.ApplicationUserId == CartObject.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction("Index");
            }
            else
            {

                var menuItemFromDb = await _db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == CartObject.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart cartObj = new ShoppingCart()
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id
                };

                return View(cartObj);
            }
        }
    }
}
