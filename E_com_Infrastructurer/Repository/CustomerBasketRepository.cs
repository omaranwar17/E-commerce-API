using E_com_Core.Interfases;
using E_com_Core.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Repository
{
    public class CustomerBasketRepository : ICustomerBasketRepository
    {
        private readonly IDatabase _database;
        public CustomerBasketRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();   
        }
        //public Task<bool> Delete(string id)
        //{
            
        //}

        public Task<bool> Delete(string id)
        {
            return _database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket> GetBasketAsync(string id)
        {
            // 1. بنجيب الداتا من الريدس
            var result = await _database.StringGetAsync(id);

            // 2. بنتأكد إن الداتا مش فاضية ومش null
            if (!string.IsNullOrEmpty(result))
            {
                // 3. بنحول الـ String (JSON) لـ Object
                return JsonSerializer.Deserialize<CustomerBasket>(result);
            }

            // 4. لو مفيش داتا، بنرجع null
            return null;
        }

        public Task<CustomerBasket> GetBasketAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var _basket = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));

            if (_basket)
            {
             
                return await GetBasketAsync(basket.Id);
            }
            return null;
        }
    }
}
