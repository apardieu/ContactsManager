using Entities;
using Newtonsoft.Json.Bson;
using ServiceContracts;
using ServiceContracts.DTO;
using ContactsManager.Core.Enums;
using Services;
using System;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
using Serilog.Extensions.Hosting;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;



        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;  
        private readonly IPersonsRepository _personsRepository;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {

            _fixture = new Fixture();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;

            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var loggerMockAdd = new Mock<ILogger<PersonsAdderService>>();
            var loggerMockGet = new Mock<ILogger<PersonsGetterService>>();
            var loggerMockDel = new Mock<ILogger<PersonsDeleterService>>();
            var loggerMockUpd = new Mock<ILogger<PersonsUpdaterService>>();
            var loggerMockSort = new Mock<ILogger<PersonsSorterService>>();

            _personsGetterService = new PersonsGetterService(_personsRepository, loggerMockGet.Object, diagnosticContextMock.Object);
            _personsAdderService = new PersonsAdderService(_personsRepository, loggerMockAdd.Object, diagnosticContextMock.Object);
            _personsDeleterService = new PersonsDeleterService(_personsRepository, loggerMockDel.Object, diagnosticContextMock.Object);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository, loggerMockUpd.Object, diagnosticContextMock.Object);
            _personsSorterService = new PersonsSorterService(_personsRepository, loggerMockSort.Object, diagnosticContextMock.Object);

            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            PersonAddRequest? personAddRequest = null;
            Func<Task> action = async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();
            Person person = personAddRequest.ToPerson();

            _personsRepositoryMock
                .Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            Func<Task> action = async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };
            await  action.Should().ThrowAsync<ArgumentException>();

        }

        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {

            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(new Person());


            PersonResponse person_response_from_add = await _personsAdderService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;

            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);

        }

        #endregion

        #region GetAllPersons

        [Fact]
        public async Task GetAllPersons_EmptyList_ToBeEmpty()
        {
            List<Person> persons = new List<Person>(); 
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> persons_from_get = await _personsGetterService.GetAllPersons();

            persons_from_get.Should().BeEmpty();

        }

        [Fact]
        public async Task GetAllPersons_AddFewPersons_ToBeSuccessful()
        {

            //List<PersonResponse> person_response_list_from_add = await CreateSamplePersonResponseList();

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email,"someone1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email,"someone2@example.com")
                .With(temp => temp.Country,null as Country)
                .Create()
            };

            List<PersonResponse> person_responses_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> person_responses_from_get_all = await _personsGetterService.GetAllPersons();

            _testOutputHelper.WriteLine("From Get All:");
            foreach (PersonResponse p in person_responses_from_get_all)
            {
                _testOutputHelper.WriteLine(p.ToString());
            }

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse p in person_responses_expected)
            {
                _testOutputHelper.WriteLine(p.ToString());
            }
            person_responses_from_get_all.Should().BeEquivalentTo(person_responses_expected);
        }



        #endregion

        #region GetPersonByPersonID

        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            Guid? personID = null;

            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);
            personResponse.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonByPersonID(person.PersonID);

            person_response_from_get.Should().Be(person_response_expected);
        }

        #endregion

        #region GetFilteredPersons



        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {


            //List<PersonResponse> person_response_list_from_add = await CreateSamplePersonResponseList();

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email,"someone1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email,"someone2@example.com")
                .With(temp => temp.Country,null as Country)
                .Create()
            };

            List<PersonResponse> person_responses_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            List<PersonResponse> person_response_from_search = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse p in person_responses_expected)
            {
                _testOutputHelper.WriteLine(p.ToString());
            }

            _testOutputHelper.WriteLine("From Search:");
            foreach (PersonResponse p in person_response_from_search)
            {
                _testOutputHelper.WriteLine(p.ToString());
            }


            person_response_from_search.Should().BeEquivalentTo(person_responses_expected);

        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {


            //List<Person> persons = new List<Person>()
            //{
            //    _fixture.Build<Person>()
            //    .With(temp => temp.Email,"someone1@example.com")
            //    .With(temp => temp.PersonName, "Patrick")
            //    .With(temp => temp.Country, null as Country)
            //    .Create(),
            //    _fixture.Build<Person>()
            //    .With(temp => temp.PersonName,"Sandra")
            //    .With(temp => temp.Email,"someone2@example.com")
            //    .With(temp => temp.Country,null as Country)
            //    .Create()
            //};

            //List<PersonResponse> person_responses_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //_personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //List<PersonResponse> person_responses_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "Pat");

            //_testOutputHelper.WriteLine("From Search:");
            //foreach (PersonResponse p in person_responses_from_search)
            //{
            //    _testOutputHelper.WriteLine(p.ToString());
            //}



            //_testOutputHelper.WriteLine("Expected:");
            //foreach (PersonResponse p in person_responses_expected)
            //{
            //    _testOutputHelper.WriteLine(p.ToString());
            //}

            //person_responses_from_search.Should().OnlyContain(temp => temp.PersonName.Contains("Pat", StringComparison.OrdinalIgnoreCase));

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email,"someone1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email,"someone2@example.com")
                .With(temp => temp.Country,null as Country)
                .Create()
            };

            List<PersonResponse> person_responses_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            List<PersonResponse> person_response_from_search = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse p in person_responses_expected)
            {
                _testOutputHelper.WriteLine(p.ToString());
            }

            _testOutputHelper.WriteLine("From Search:");
            foreach (PersonResponse p in person_response_from_search)
            {
                _testOutputHelper.WriteLine(p.ToString());
            }


            person_response_from_search.Should().BeEquivalentTo(person_responses_expected);
        }


        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email,"someone1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email,"someone2@example.com")
                .With(temp => temp.Country,null as Country)
                .Create()
            };

            List<PersonResponse> person_responses_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);


            List<PersonResponse> person_response_from_sort = await _personsSorterService.GetSortedPersons(person_responses_expected, (nameof(Person.PersonName)), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse p in person_response_from_sort)
            {
                _testOutputHelper.WriteLine(p.ToString());
            }

            person_response_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);

            //person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();
            //person_response_from_sort.Should().BeEquivalentTo(person_response_list_from_add);


        }

        #endregion

        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            PersonUpdateRequest personUpdateRequest = null;
            Func<Task> action = async () => await _personsUpdaterService.UpdatePerson(personUpdateRequest);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest() { PersonID = new Guid() };
            Func<Task> action = async () => await _personsUpdaterService.UpdatePerson(personUpdateRequest);

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]

        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {


            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Gender, "Male")
                .Create();
            PersonResponse personResponse = person.ToPersonResponse();

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            Func<Task> action = async () => await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            await action.Should().ThrowAsync<ArgumentException>();

        }


        [Fact]

        public async Task UpdatePerson_PersonFullDetailsUpdation_ToBeSuccessful()
        {

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();


            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            PersonUpdateRequest personUpdateRequest = person_response_expected.ToPersonUpdateRequest();

            PersonResponse personResponse_from_update = await _personsUpdaterService.UpdatePerson(personUpdateRequest);

            personResponse_from_update.Should().Be(person_response_expected);

        }

        #endregion

        #region DeletePerson

        [Fact]

        public async Task DeletePerson_ValidPersonID_ToBeTrue()
        {

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse personResponse = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);

            bool isDeleted = await _personsDeleterService.DeletePerson(personResponse.PersonID);

            isDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeletePerson_InvalidPersonID_ToBeFalse()
        {
            bool isDeleted = await _personsDeleterService.DeletePerson(new Guid());
            isDeleted.Should().BeFalse();
        }

        #endregion
        //public async Task<List<PersonResponse>> CreateSamplePersonResponseList()
        //{
        //    CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
        //    CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

        //    CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        //    CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

        //    PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
        //        .With(temp => temp.Email, "someone@example.com")
        //        .With(temp => temp.PersonName, "Patrick")
        //        .With(temp => temp.CountryID, countryResponse1.CountryID)
        //        .Create();

        //    PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
        //        .With(temp => temp.Email, "someone@example.com")
        //        .With(temp => temp.CountryID, countryResponse2.CountryID)
        //        .Create();

        //    PersonResponse personResponse1 = await _personsService.AddPerson(personAddRequest1);
        //    PersonResponse personResponse2 = await _personsService.AddPerson(personAddRequest2);

        //    List<PersonResponse> person_response_list_from_add = new List<PersonResponse> { personResponse1, personResponse2 };

        //    return person_response_list_from_add;
        //}


    }
}
