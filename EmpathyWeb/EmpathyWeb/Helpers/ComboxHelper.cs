using EmpathyWeb.Data;
using EmpathyWeb.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmpathyWeb.Helpers
{
	public class ComboxHelper : IComboxHelper
	{
		private readonly DataContext _context;

		public ComboxHelper(DataContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<SelectListItem>> GetComboCategoriesAsync()
		{
			List<SelectListItem> List = await _context.Categories.Select(c => new SelectListItem
			{
				Text = c.Name,
				Value = c.Id.ToString(),
			}).OrderBy(c => c.Text)
				.ToListAsync();
			List.Insert(0, new SelectListItem { Text = "[Seleccione una Categoía...", Value = "0" });

			return List;
		}

		public async Task<IEnumerable<SelectListItem>> GetComboCitiesAsync(int stateId)
		{
			List<SelectListItem> List = await _context.Cities
				.Where(s => s.State.Id == stateId)
				.Select(c => new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString(),
				}).OrderBy(c => c.Text)
				.ToListAsync();
			List.Insert(0, new SelectListItem { Text = "[Seleccione una ciudad...", Value = "0" });

			return List;
		}

		public async Task<IEnumerable<SelectListItem>> GetComboCountriesAsync()
		{
			List<SelectListItem> List = await _context.Countries.Select(c => new SelectListItem
			{
				Text = c.Name,
				Value = c.Id.ToString(),
			}).OrderBy(c => c.Text)
				.ToListAsync();
			List.Insert(0, new SelectListItem { Text = "[Seleccione un país...", Value = "0" });

			return List;
		}

		public async Task<IEnumerable<SelectListItem>> GetComboStatesAsync(int countryId)
		{
			List<SelectListItem> List = await _context.States
				.Where(s => s.Country.Id == countryId)
				.Select(c => new SelectListItem
			{
				Text = c.Name,
				Value = c.Id.ToString(),
			}).OrderBy(c => c.Text)
				.ToListAsync();
			List.Insert(0, new SelectListItem { Text = "[Seleccione un Departamento...", Value = "0" });

			return List;
		}
	}
}
