using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Exceptions;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;
using ContactsManager.Core.Enums;
using Services.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {

        private readonly IPersonsRepository _personRepository;
        private readonly ILogger<PersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsUpdaterService( IPersonsRepository personsRepository,ILogger<PersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if(personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            ValidationHelper.ModelValidation(personUpdateRequest);

            Person? matchingPerson = await _personRepository.GetPersonByPersonId(personUpdateRequest.PersonID);

            if(matchingPerson == null)
            {
                throw new InvalidPersonIDException("Given person id doesn't exist");
            }

            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _personRepository.UpdatePerson(matchingPerson);

            return matchingPerson.ToPersonResponse();

        }

    }
}
