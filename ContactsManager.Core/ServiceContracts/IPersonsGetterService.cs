using System;
using ServiceContracts.DTO;
using ContactsManager.Core.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entities
    /// </summary>
    public interface IPersonsGetterService
    {

        /// <summary>
        /// Getting all persons
        /// </summary>
        /// <returns>The list containing all persons as PersonResponse</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Returns the person object based on the given person id
        /// </summary>
        /// <param name="personID">Person id to searc</param>
        /// <returns>Returns matching person object</returns>
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);
        
        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all the person that matches search field and search string</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Returns persons as CSV
        /// </summary>
        /// <returns>Memory Stream with CSV</returns>
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Returns persons as Excel File
        /// </summary>
        /// <returns>Memory Stream with Excel</returns>
        Task<MemoryStream> GetPersonsExcel();

    }
}
