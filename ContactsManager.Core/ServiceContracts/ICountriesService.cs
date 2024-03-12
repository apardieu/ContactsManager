using ServiceContracts.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        ///  Adds a country boject to the list of countries
        /// </summary>
        /// <param name="countryAddRequest"> Country object to add</param>
        /// <returns>Returns the country object after adding it (including newly generated country id)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returs all countries from the list of countries
        /// </summary>
        /// <returns>All countries from the list as List of CoutryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns a country object based on the given country id
        /// </summary>
        /// <param name="countryID">CountryID (guid) to search</param>
        /// <returns>Matching country as a CountryResponse</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);

        /// <summary>
        /// Uploads countries from excel file into Database
        /// </summary>
        /// <param name="formFile">File representing different countries</param>
        /// <returns>Number of countries in file</returns>
        Task<int>UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
