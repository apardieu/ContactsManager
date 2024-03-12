using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using AutoFixture;
using AutoFixture.Kernel;
using RepositoryContracts;
using FluentAssertions;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly Fixture _fixture;
        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            
            _countriesService = new CountriesService(_countriesRepository);
        }

        //When CountryAddRequest is null, it should throw ArgumentNullException
        #region AddCountry
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrange
            CountryAddRequest? request = null;
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest countryAddRequest = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();
            Country country = countryAddRequest.ToCountry();

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(countryAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();

        }

        //When the CountryName is duplicate, is shoud throw ArgumentException

        [Fact]
        public async Task AddCountry_Duplicate_ToBeArgumentException()
        {
            CountryAddRequest countryAddRequest = _fixture.Build<CountryAddRequest>()
                .Create();

            Country country = countryAddRequest.ToCountry();
            await _countriesService.AddCountry(countryAddRequest);

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);
            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            await _countriesService.AddCountry(countryAddRequest);

            Func<Task> action = async () =>
            {
                _countriesRepositoryMock
                    .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                    .ReturnsAsync(country);
                _countriesRepositoryMock
                    .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                    .ReturnsAsync(country);

                await _countriesService.AddCountry(countryAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When you supply proper country name, it should insert (add) the country to the existing list of countries

        [Fact]
        public async Task AddCountry_ProperCountryDetails_ToBeSuccessfull()
        {
            //Arrange
            CountryAddRequest? country_add_request = _fixture.Create<CountryAddRequest>();

            Country country = country_add_request.ToCountry();
            CountryResponse country_response_expected = country.ToCountryResponse();

            _countriesRepositoryMock
            .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
    .       ReturnsAsync(country);
            _countriesRepositoryMock
            .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);

            //Act
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            country_response_expected.CountryID = country_response_from_add.CountryID;

            country_response_expected.CountryID.Should().NotBe(Guid.Empty);
            country_response_expected.Should().Be(country_response_from_add);
        }
        #endregion

        #region GetAllCountries

        [Fact]
        //The list of countries should be empty by default (before adding any countries)
        public async Task GettAllCountries_EmptyList_ToBeEmpty()
        {
            List<Country> countries = new List<Country>();
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            List<CountryResponse> countries_from_get = await _countriesService.GetAllCountries();

            countries_from_get.Should().BeEmpty();

        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries_ToBeSuccessful()
        {

            List<Country> countries = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).CreateMany().ToList();

            List<CountryResponse> countries_expected = countries.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            List<CountryResponse> countries_from_get_all = await _countriesService.GetAllCountries();

            countries_from_get_all.Should().BeEquivalentTo(countries_expected);

        }
        #endregion

        #region GetCountryByCountryID

        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            Guid? countryID = null;
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(null as Country);

            CountryResponse? country_response_from_get_by_id = await _countriesService.GetCountryByCountryID(countryID);
            country_response_from_get_by_id.Should().BeNull();
        }

        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
        {
            Country country = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create();
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(country);

            CountryResponse countryResponse_expected = country.ToCountryResponse();

            CountryResponse? country_from_get_by_id = await _countriesService.GetCountryByCountryID(country.CountryID);

            country_from_get_by_id.Should().Be(countryResponse_expected);
        }
        #endregion
    }
}
