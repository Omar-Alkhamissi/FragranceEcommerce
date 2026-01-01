using FragEcom.DAL;
using FragEcom.DAL.DAO;
using FragEcom.DAL.DomainClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FragEcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandsController : ControllerBase
    {
        private readonly AppDbContext? _ctx;

        public BrandsController(AppDbContext context)
        {
            _ctx = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Brands>>> Index()
        {
            BrandsDAO dao = new(_ctx!);
            List<Brands> allBrands = await dao.GetAll();
            return allBrands;
        }
    }
}
