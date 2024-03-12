using ContactsManager.Core.Enums;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is usead as return type of most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }
        public string? Country { get; set; }

        /// <summary>
        /// Compares the current object's data with the parameter object
        /// </summary>
        /// <param name="obj">Object to compare with current object</param>
        /// <returns>True if all parameters are matching, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if(obj.GetType() != typeof(PersonResponse)) return false;
         
            PersonResponse response = (PersonResponse) obj;
            return this.PersonName == response.PersonName &&
                this.Email == response.Email &&
                this.DateOfBirth == response.DateOfBirth &&
                this.Gender == response.Gender &&
                this.CountryID == response.CountryID &&
                this.Address == response.Address &&
                this.ReceiveNewsLetters == response.ReceiveNewsLetters &&
                this.Age == response.Age;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID : {PersonID}, Person Name : {PersonName}, Email : {Email}, DateOfBirth : {DateOfBirth}, Gender : {Gender}, CountryId : {CountryID}, Address : {Address}, ReceiveNewsLetter : {ReceiveNewsLetters}, Age : {Age}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions),Gender,true),
                Address = Address,
                CountryID = CountryID,
                ReceiveNewsLetters = ReceiveNewsLetters
            };      
        }

    }
    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert an object of Person class into PersonResponse
        /// </summary>
        /// <param name="person">Person object to be returned as person response</param>
        /// <returns>Returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                Country = person.Country?.CountryName
            };
        }
    }
}
