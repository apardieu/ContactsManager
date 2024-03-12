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
    public class PersonsAdderService : IPersonsAdderService
    {

        private readonly IPersonsRepository _personRepository;
        private readonly ILogger<PersonsAdderService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsAdderService( IPersonsRepository personsRepository,ILogger<PersonsAdderService> logger, IDiagnosticContext diagnosticContext)
        {
            _personRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //Model validations

            ValidationHelper.ModelValidation(personAddRequest);

            //if(string.IsNullOrEmpty(personAddRequest.PersonName)) 
            //    throw new ArgumentNullException("Person Name can't be blank");

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();
            await _personRepository.AddPerson(person);
            //_db.sp_InsertPerson(person);

            return person.ToPersonResponse();
        }

    }
}
