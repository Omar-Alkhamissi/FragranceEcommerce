using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FragEcom.DAL;                 
using FragEcom.DAL.DomainClasses;

namespace FragEcom.DAL.DAO
{
    public class BrandsDAO              
    {
        private readonly AppDbContext _db;

        public BrandsDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<List<Brands>> GetAll()
        {
            return await _db.Brands.ToListAsync();
        }
    }
}
