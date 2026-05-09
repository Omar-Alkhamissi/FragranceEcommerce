using FragEcom.DAL;
using FragEcom.DAL.DAO;
using FragEcom.DAL.DomainClasses;
using FragEcom.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FragEcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public OrderController(AppDbContext context)
        {
            _ctx = context;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Index(OrderHelper helper)
        {
            try
            {
                CustomerDAO cDao = new(_ctx);
                Customer? orderOwner = await cDao.GetByEmail(helper.Email);

                if (orderOwner == null)
                    return "Customer not found";

                OrderDAO oDao = new(_ctx);
                int orderId = await oDao.AddOrder(orderOwner.Id, helper.Selections!);

                return orderId > 0
                    ? $"Order {orderId} saved!"
                    : "Order not saved";
            }
            catch (Exception ex)
            {
                return $"Order not saved: {ex.Message}";
            }
        }

        [Route("{email}")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> List(string email)
        {
            CustomerDAO cDao = new(_ctx);
            Customer? orderOwner = await cDao.GetByEmail(email);

            if (orderOwner == null)
                return new List<Order>();

            OrderDAO oDao = new(_ctx);
            return await oDao.GetAll(orderOwner.Id);
        }

        [Route("{orderid}/{email}")]
        [HttpGet]
        public async Task<ActionResult<List<OrderDetailsHelper>>> GetOrderDetails(int orderid, string email)
        {
            try
            {
                OrderDAO dao = new(_ctx);
                return await dao.GetOrderDetails(orderid, email);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving order details: {ex.Message}");
            }
        }
    }
}