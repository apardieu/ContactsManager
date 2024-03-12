﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Add a person object to the data store
        /// </summary>
        /// <param name="person">Person to add</param>
        /// <returns>Returns the person object after adding it ot the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Get all persons object stored in data store
        /// </summary>
        /// <returns>List of all persons in data store</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns a person object based on the given Guid if it exists, otherwise it returns null
        /// </summary>
        /// <param name="personID">Guid to match</param>
        /// <returns>Matching person or null if no person is matching</returns>
        Task<Person?> GetPersonByPersonId(Guid personID);

        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Deletes a person object based on the person ID
        /// </summary>
        /// <param name="personID">Person ID to delete</param>
        /// <returns>Returns true if the deletion is successful, otherwise false</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);


        /// <summary>
        /// Updates a person object based on the given person id
        /// </summary>
        /// <param name="person">Person object to update</param>
        /// <returns>Returns the update person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
