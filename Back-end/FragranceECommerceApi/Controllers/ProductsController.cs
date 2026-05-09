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
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext? _ctx;

        public ProductsController(AppDbContext context) // injected here
        {
            _ctx = context;
        }

        [HttpGet]
        [Route("{brandid}")]
        public async Task<ActionResult<List<Products>>> Index(int brandid)
        {
            ProductsDAO dao = new(_ctx!);
            List<Products> productsForBrand = await dao.GetAllByBrand(brandid);
            return productsForBrand;
        }
    }
}
