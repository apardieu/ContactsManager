using System;
using ServiceContracts.DTO;
using ContactsManager.Core.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entities
    /// </summary>
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Adds a new person into the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person to Add</param>
        /// <returns>Returns the same person details, along a newly generated PersonID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

    }
}
