using Bogus;
using ExpectedObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyEcommerce.Api.Entities;
using MyEcommerce.Api.Repositories;
using MyEcommerce.Api.Repositories.Interface;
using MyEcommerce.Controllers.Api;
using MyEcommerce.IntegrationTest.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class CustomerControllerTests : ControllerTestsBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomerController _customerController;

        public CustomerControllerTests(ITestOutputHelper output) : base(output)
        {
            _logger = new Logger<CustomerController>(LoggerFactory);
            _customerRepository = new CustomerRepository(DbContext);
            _customerController = new CustomerController(_logger, _customerRepository);

            output.WriteLine($"Database name: {DatabaseName}");
        }

        [Fact(DisplayName = "Given valid customer when deleting by id should be success")]
        public async void GivenValidCustomer_WhenDeletingById_SouldBeSuccess()
        {
            //Arrange
            int idCustomer = new Random().Next(1, DbContext.Customers.Count());

            //Act
            var response = await _customerController.Delete(idCustomer);

            //Assert
            response.IsHttpStatusCodeNoContent().Should().BeTrue();

            var customerActual = await _customerRepository.GetById(idCustomer);

            customerActual.Should().BeNull();

            //Log
            Output.WriteLine($"Customer id: {idCustomer}");
        }

        [Fact(DisplayName = "Given valid customer when searching by id should be success")]
        public async void GivenValidCustomer_WhenSearchingById_SouldBeSuccess()
        {
            //Arrange
            int idCustomer = new Random().Next(1, DbContext.Customers.Count());

            var customerExpected = DbContext.Customers
                .FirstOrDefault(f => f.Id == idCustomer);

            //Act
            var response = await _customerController.GetById(customerExpected.Id);

            //Assert
            response.IsHttpStatusCodeOK().Should().BeTrue();

            var customerActual = await _customerRepository.GetById(idCustomer);

            customerActual.ToExpectedObject().ShouldMatch(customerExpected);

            //Log
            Output.WriteLine($"Customer id: {idCustomer}");
        }

        [Fact(DisplayName = "Given valid customer when updating should be success")]
        public async void GivenValidCustomer_WhenUpdating_ShouldBeSuccess()
        {
            //Arrange
            int idCustomer = new Random().Next(1, DbContext.Customers.Count());

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

            //Log
            Output.WriteLine($"Customer id: {idCustomer}");
            Output.WriteLine("Customer expected:");
            Output.WriteLine(JsonSerializer.Serialize(customerExpected));
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

            //Log
            Output.WriteLine("Customer expected:");
            Output.WriteLine(JsonSerializer.Serialize(customerExpected));
        }

        [Fact(DisplayName = "Given valid customer when getting all should be success")]
        public async void GivenValidCustomer_WhenGettingAll_ShouldBeSuccess()
        {
            //Arrange
            var customerExpected = DbContext.Customers.ToList();

            //Act
            var response = await _customerController.GetAll();

            var customerActual = response
                .As<OkObjectResult>().Value
                .As<List<Customer>>();

            //Assert
            response.IsHttpStatusCodeOK().Should().BeTrue();

            customerActual.Should().HaveCount(customerExpected.Count);

            //Log
            Output.WriteLine($"Count customer expected: {customerExpected.Count}");
            Output.WriteLine($"Count customer actual: {customerActual.Count}");
        }
    }
}