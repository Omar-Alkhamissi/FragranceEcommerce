using FragEcom.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace FragEcom.DAL.DAO
{

    public class ProductsDAO
    {
        private readonly AppDbContext _db;
        public ProductsDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<List<Products>> GetAllByBrand(int id)
        {
            return await _db.Products!.Where(product => product.BrandId == id).ToListAsync();
        }

    }
}
