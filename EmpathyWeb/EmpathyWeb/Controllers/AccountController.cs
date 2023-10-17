using EmpathyWeb.Data;
using EmpathyWeb.Data.Entities;
using EmpathyWeb.Enums;
using EmpathyWeb.Helpers;
using EmpathyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmpathyWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
		private readonly DataContext _context;
		private readonly IComboxHelper _comboxHelper;
		private readonly IBlobHelper _blobHelper;

		public AccountController(IUserHelper userHelper, DataContext context, IComboxHelper comboxHelper, IBlobHelper blobHelper)
        {
            _userHelper = userHelper;
			_context = context;
			_comboxHelper = comboxHelper;
			_blobHelper = blobHelper;
		}

        public IActionResult Login()
        {
            //Devuelve el usuario logueado
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }
		//Registro de usuarios
		public async Task<IActionResult> Register()
		{
			AddUserViewModel model = new AddUserViewModel
			{
				Id = Guid.Empty.ToString(),
				Countries = await _comboxHelper.GetComboCountriesAsync(),
				States = await _comboxHelper.GetComboStatesAsync(0),
				Cities = await _comboxHelper.GetComboCitiesAsync(0),
				UserType = UserType.User,
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(AddUserViewModel model)
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

				LoginViewModel loginViewModel = new LoginViewModel
				{
					Password = model.Password,
					RememberMe = false,
					Username = model.Username
				};

				var result2 = await _userHelper.LoginAsync(loginViewModel);

				if (result2.Succeeded)
				{
					return RedirectToAction("Index", "Home");
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

        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new()
            {
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImageId = user.ImageId,
                Cities = await _comboxHelper.GetComboCitiesAsync(user.City.State.Id),
                CityId = user.City.Id,
                Countries = await _comboxHelper.GetComboCountriesAsync(),
                CountryId = user.City.State.Country.Id,
                StateId = user.City.State.Id,
                States = await _comboxHelper.GetComboStatesAsync(user.City.State.Country.Id),
                Id = user.Id,
                Document = user.Document
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = model.ImageId;

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }

                User user = await _userHelper.GetUserAsync(User.Identity.Name);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.ImageId = imageId;
                user.City = await _context.Cities.FindAsync(model.CityId);
                user.Document = model.Document;

                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        //Cambio de contraseña
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User no found.");
                }
            }

            return View(model);
        }




    }
}
