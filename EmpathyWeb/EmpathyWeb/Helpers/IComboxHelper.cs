using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmpathyWeb.Helpers
{
	public interface IComboxHelper
	{
		Task<IEnumerable<SelectListItem>> GetComboCategoriesAsync();

		Task<IEnumerable<SelectListItem>> GetComboCountriesAsync();

		Task<IEnumerable<SelectListItem>> GetComboStatesAsync(int countryId);

		Task<IEnumerable<SelectListItem>> GetComboCitiesAsync(int stateId);

	}
}
