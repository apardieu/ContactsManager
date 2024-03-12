using System;
using ServiceContracts.DTO;
using ContactsManager.Core.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entities
    /// </summary>
    public interface IPersonsUpdaterService
    {
        /// <summary>
        /// Update an existing Person
        /// </summary>
        /// <param name="request">Person details to update, including person id</param>
        /// <returns>Returns the person response objecct after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
       
    }
}
