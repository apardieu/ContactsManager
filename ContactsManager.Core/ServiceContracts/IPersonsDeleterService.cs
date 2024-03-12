using System;
using ServiceContracts.DTO;
using ContactsManager.Core.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entities
    /// </summary>
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Delete an existing Person
        /// </summary>
        /// <param name="personID">PersonId to delete</param>
        /// <returns>Returns true if deletation is successfull else false</returns>
        Task<bool> DeletePerson(Guid? personID);

    }
}
