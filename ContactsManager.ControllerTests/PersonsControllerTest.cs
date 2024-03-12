using AutoFixture;
using Moq;
using ServiceContracts;
using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using CRUDExample.Controllers;
using ServiceContracts.DTO;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Entities;
using Microsoft.Extensions.Logging;


namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;

        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;


        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;


        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _countriesServiceMock = new Mock<ICountriesService>();
            
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();

            _personsAdderService = _personsAdderServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsGetterService = _personsGetterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;

            _loggerMock = new Mock<ILogger<PersonsController>>();
            _countriesService = _countriesServiceMock.Object;
            _logger = _loggerMock.Object;
        }

        #region Index

        [Fact]

        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_countriesService, 
                _personsGetterService, 
                _personsAdderService, 
                _personsDeleterService, 
                _personsUpdaterService, 
                _personsSorterService, 
                _logger);

            //Getfiltered et getpersons à mock

            _personsGetterServiceMock
                .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(persons_response_list);
            _personsSorterServiceMock
                .Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(persons_response_list);

            IActionResult result = await personsController.Index(_fixture.Create<string>(),
                 _fixture.Create<string>(),
                 _fixture.Create<string>(),
                 _fixture.Create<SortOrderOptions>());

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(persons_response_list);

        }
        #endregion

        #region Create

        // Not usefull anymore, as validation is made in filter 

        //[Fact]
        //public async void Create_IfModelErros_ToReturnCreateView()
        //{
        //    PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
        //    PersonResponse person_response = _fixture.Create<PersonResponse>();
        //    List<CountryResponse> countries = _fixture.CreateMany<CountryResponse>().ToList();

        //    PersonsController personsController = new PersonsController(_countriesService, _personsService,_logger);

        //    _countriesServiceMock
        //        .Setup(temp => temp.GetAllCountries())
        //        .ReturnsAsync(countries);

        //    _personsServiceMock
        //        .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
        //        .ReturnsAsync(person_response);

        //    personsController.ModelState.AddModelError("PersonName", "PersonName can't be blank");

        //    IActionResult result = await personsController.Create(person_add_request);

        //    ViewResult viewResult = Assert.IsType<ViewResult>(result);
        //    viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>(); // Check that the model returned in view is a PersonAddRequest (which is the case in that CreateView)
        //    viewResult.ViewData.Model.Should().Be(person_add_request);

        //}

        [Fact]
        public async void Create_IfNoModelErros_ToRedirectToIndex()
        {
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
            PersonResponse person_response = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.CreateMany<CountryResponse>().ToList();

            PersonsController personsController = new PersonsController(_countriesService,
            _personsGetterService,
            _personsAdderService,
            _personsDeleterService,
            _personsUpdaterService,
            _personsSorterService,
            _logger);

            _countriesServiceMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _personsAdderServiceMock
                .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            IActionResult result = await personsController.Create(person_add_request);

            RedirectToActionResult redirectResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion

        #region Delete

        #endregion


        #region Edit
        [Fact]
        public async void Edit_IfNoPersonFound_ToRedirectionToIndex()
        {

            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();
            PersonResponse personResponse = _fixture.Create<PersonResponse>();



            _personsGetterServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(null as PersonResponse);


            PersonsController controller = new PersonsController(_countriesService, 
                _personsGetterService, 
                _personsAdderService, 
                _personsDeleterService, 
                _personsUpdaterService, 
                _personsSorterService, 
                _logger);

            IActionResult result = await controller.Edit(personUpdateRequest);
            RedirectToActionResult redirectToActionResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
            redirectToActionResult.ActionName.Should().Be("Index");

        }
        [Fact]
        public async void Edit_WithModelError_ToReturnEditView()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();
            PersonResponse personResponse = personUpdateRequest.ToPerson().ToPersonResponse();
            List<CountryResponse> countriesResponses = _fixture.Create<List<CountryResponse>>();

            PersonsController personsController = new PersonsController(_countriesService,
            _personsGetterService,
            _personsAdderService,
            _personsDeleterService,
            _personsUpdaterService,
            _personsSorterService,
            _logger);

            _personsGetterServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(personResponse);
            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countriesResponses);


            personsController.ModelState.AddModelError("PersonName", "PersonName can't be blank");

            IActionResult result = await personsController.Edit(personUpdateRequest);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();
            viewResult.ViewData.Model.Should().BeEquivalentTo(personUpdateRequest);
        }

        [Fact]
        public async void Edit_IfNoModelError_ToRedirectionToIndex()
        {

            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();
            PersonResponse personResponse = _fixture.Create<PersonResponse>();


            _personsGetterServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(personResponse);
            _personsUpdaterServiceMock.Setup(temp => temp.UpdatePerson(It.IsAny<PersonUpdateRequest>())).ReturnsAsync(personResponse);

            PersonsController controller = new PersonsController(_countriesService,
            _personsGetterService,
            _personsAdderService,
            _personsDeleterService,
            _personsUpdaterService,
            _personsSorterService,
            _logger);

            IActionResult result = await controller.Edit(personUpdateRequest);
            RedirectToActionResult redirectToActionResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
            redirectToActionResult.ActionName.Should().Be("Index");

        }


        #endregion
    }
}

