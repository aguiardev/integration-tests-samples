using Bogus;
using ExpectedObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyEcommerce.Api.Entities;
using MyEcommerce.Api.Repositories;
using MyEcommerce.Api.Repositories.Interface;
using MyEcommerce.Controllers.Api;
using MyEcommerce.IntegrationTest.Fixture;
using MyEcommerce.IntegrationTest.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class CustomerControllerTests : ControllerTestsBase, IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomerController _customerController;
        private readonly DatabaseFixture _databaseFixture;

        public CustomerControllerTests(ITestOutputHelper output, DatabaseFixture databaseFixture) : base(output)
        {
            _logger = new Logger<CustomerController>(LoggerFactory);
            _customerRepository = new CustomerRepository(databaseFixture.Context);
            _customerController = new CustomerController(_logger, _customerRepository);
            _databaseFixture = databaseFixture;
            databaseFixture.Seed();

            output.WriteLine($"Database name: {databaseFixture.DatabaseName}");
        }

        public void Dispose() => _databaseFixture.Reset();

        [Fact(DisplayName = "Given valid customer when deleting by id should be success")]
        public async void GivenValidCustomer_WhenDeletingById_SouldBeSuccess()
        {
            //Arrange
            const int idCustomer = 2;

            //Act
            var response = await _customerController.Delete(idCustomer);

            //Assert
            response.IsHttpStatusCodeNoContent().Should().BeTrue();

            var customerActual = await _customerRepository.GetById(idCustomer);

            customerActual.Should().BeNull();
        }

        [Fact(DisplayName = "Given valid customer when searching by id should be success")]
        public async void GivenValidCustomer_WhenSearchingById_SouldBeSuccess()
        {
            //Arrange
            const int idCustomer = 1;

            var customerExpected = _databaseFixture.Context.Customers
                .FirstOrDefault(f => f.Id == idCustomer);

            //Act
            var response = await _customerController.GetById(customerExpected.Id);

            //Assert
            response.IsHttpStatusCodeOK().Should().BeTrue();

            var customerActual = await _customerRepository.GetById(idCustomer);

            customerActual.ToExpectedObject().ShouldMatch(customerExpected);
        }

        [Fact(DisplayName = "Given valid customer when updating should be success")]
        public async void GivenValidCustomer_WhenUpdating_ShouldBeSuccess()
        {
            //Arrange
            const int idCustomer = 1;

            var customerExpected = new Faker<Customer>(Constants.LOCALE_FAKER)
                .RuleFor(p => p.Id, () => idCustomer)
                .RuleFor(p => p.Name, faker => faker.Person.FirstName)
                .RuleFor(p => p.Email, faker => faker.Person.Email)
                .RuleFor(p => p.Birth, faker => faker.Date.Between(
                    new DateTime(1970, 1, 1), new DateTime(2000, 12, 31)))
                .Generate();

            //Act
            var response = await _customerController.Put(idCustomer, customerExpected);

            //Assert
            response.IsHttpStatusCodeNoContent().Should().BeTrue();

            var customerActual = await _customerRepository.GetById(idCustomer);

            customerActual.ToExpectedObject().ShouldMatch(customerExpected);
        }

        [Fact(DisplayName = "Given valid customer when creating should be success")]
        public async void GivenValidCustomer_WhenCreating_ShouldBeSuccess()
        {
            //Arrange
            var customerExpected = new Faker<Customer>(Constants.LOCALE_FAKER)
                .RuleFor(p => p.Id, () => 0)
                .RuleFor(p => p.Name, faker => faker.Person.FirstName)
                .RuleFor(p => p.Email, faker => faker.Person.Email)
                .RuleFor(p => p.Birth, faker => faker.Date.Between(
                    new DateTime(1970, 1, 1), new DateTime(1991, 01, 01)))
                .Generate();

            //Act
            var response = await _customerController.Post(customerExpected);

            //Assert
            customerExpected.Id = response
                .As<CreatedAtActionResult>().Value
                .As<Customer>().Id;

            response.IsHttpStatusCodeCreated().Should().BeTrue();

            var customerActual = await _customerRepository.GetById(customerExpected.Id);

            customerActual.ToExpectedObject().ShouldMatch(customerExpected);
        }

        [Fact(DisplayName = "Given valid customer when getting all should be success")]
        public async void GivenValidCustomer_WhenGettingAll_ShouldBeSuccess()
        {
            //Arrange
            var customerExpected = _databaseFixture.Context.Customers.ToList();

            //Act
            var response = await _customerController.GetAll();

            //Assert
            var customerActual = response
                .As<OkObjectResult>().Value
                .As<List<Customer>>();

            response.IsHttpStatusCodeOK().Should().BeTrue();

            customerActual.Should().HaveCount(customerExpected.Count);
        }
    }
}