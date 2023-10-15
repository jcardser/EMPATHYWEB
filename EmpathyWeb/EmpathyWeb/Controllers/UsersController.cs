using EmpathyWeb.Data;
using EmpathyWeb.Data.Entities;
using EmpathyWeb.Enums;
using EmpathyWeb.Helpers;
using EmpathyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmpathyWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IComboxHelper _comboxHelper;

        public UsersController(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper, IComboxHelper comboxHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _comboxHelper = comboxHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(U => U.City)
                .ThenInclude(c => c.State)
                .ThenInclude(s => s.Country)
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Id = Guid.Empty.ToString(),
                Countries = await _comboxHelper.GetComboCountriesAsync(),
                States = await _comboxHelper.GetComboStatesAsync(0),
                Cities = await _comboxHelper.GetComboCitiesAsync(0),
                UserType = UserType.UserProfessional,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }
                model.ImageId = imageId;
                User user = await _userHelper.AddUserAsync(model);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    model.Countries = await _comboxHelper.GetComboCountriesAsync();
                    model.States = await _comboxHelper.GetComboStatesAsync(model.CountryId);
                    model.Cities = await _comboxHelper.GetComboCitiesAsync(model.StateId);
                    return View(model);
                }

            }

            return View(model);
        }


        public JsonResult GetStates(int countryId)
        {
            Country country = _context.Countries
                .Include(c => c.States)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.States.OrderBy(d => d.Name));
        }

        public JsonResult GetCities(int stateId)
        {
            State state = _context.States
                .Include(s => s.Cities)
                .FirstOrDefault(s => s.Id == stateId);
            if (state == null)
            {
                return null;
            }

            return Json(state.Cities.OrderBy(c => c.Name));
        }
    }
}
