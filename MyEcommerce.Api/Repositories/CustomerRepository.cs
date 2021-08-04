using Microsoft.EntityFrameworkCore;
using MyEcommerce.Api.Models;
using MyEcommerce.Api.Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyEcommerce.Api.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly MyEcommerceContext _myEcommerceContext;
        
        public CustomerRepository(MyEcommerceContext myEcommerceContext)
            => _myEcommerceContext = myEcommerceContext;

        public async Task Delete(int id)
        {
            var customer = await GetById(id);

            _myEcommerceContext.Customers.Remove(customer);
            await _myEcommerceContext.SaveChangesAsync();
        }

        public async Task<IList<Customer>> GetAll()
            => await _myEcommerceContext.Customers.ToListAsync();

        public async Task<Customer> GetById(int id)
            => await _myEcommerceContext.Customers.FirstOrDefaultAsync(f => f.Id == id);

        public async Task Create(Customer customer)
        {
            _myEcommerceContext.Customers.Add(customer);
            await _myEcommerceContext.SaveChangesAsync();
        }

        public async Task Update(int id, Customer customer)
        {
            var currentCustomer = await GetById(id);

            currentCustomer.Name = customer.Name;
            currentCustomer.Email = customer.Email;
            currentCustomer.Birth = customer.Birth;

            await _myEcommerceContext.SaveChangesAsync();
        }
    }
}