using System;
using ContactsManager.Core.Enums;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entities
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Adds a new person into the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person to Add</param>
        /// <returns>Returns the same person details, along a newly generated PersonID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

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
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">Represents list of persons to sort</param>
        /// <param name="sortBy">Name of the property (key), based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC or Descending</param>
        /// <returns>List of sorted persons</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Update an existing Person
        /// </summary>
        /// <param name="request">Person details to update, including person id</param>
        /// <returns>Returns the person response objecct after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Delete an existing Person
        /// </summary>
        /// <param name="personID">PersonId to delete</param>
        /// <returns>Returns true if deletation is successfull else false</returns>
        Task<bool> DeletePerson(Guid? personID);

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
