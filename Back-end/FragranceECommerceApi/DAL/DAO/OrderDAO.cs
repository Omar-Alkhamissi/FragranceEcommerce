using FragEcom.DAL.DomainClasses;
using FragEcom.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FragEcom.DAL.DAO
{
    public class OrderDAO
    {
        private readonly AppDbContext _db;
        private const decimal TAX_RATE = 0.13m;

        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<int> AddOrder(int customerId, CartSelectionHelper[] selections)
        {
            int orderId = -1;

            try
            {
                Order order = new()
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.UtcNow,
                    OrderAmount = 0.0M
                };

                await _db.Orders!.AddAsync(order);
                await _db.SaveChangesAsync();

                decimal subtotal = 0.0M;
                List<OrderLineItem> lineItems = new();

                foreach (CartSelectionHelper selection in selections)
                {
                    if (selection.Item != null)
                    {
                        int qtySold = Math.Min(selection.Qty, selection.Item.QtyOnHand);
                        int qtyBackOrdered = Math.Max(0, selection.Qty - selection.Item.QtyOnHand);

                        OrderLineItem lineItem = new()
                        {
                            OrderId = order.Id,
                            ProductId = selection.Item.Id,
                            QtyOrdered = selection.Qty,
                            QtySold = qtySold,
                            QtyBackOrdered = qtyBackOrdered,
                            SellingPrice = selection.Item.MSRP
                        };

                        lineItems.Add(lineItem);

                        subtotal += lineItem.SellingPrice * lineItem.QtyOrdered;
                    }
                }

                await _db.OrderLineItems!.AddRangeAsync(lineItems);
                decimal tax = Math.Round(subtotal * TAX_RATE, 2);
                decimal total = Math.Round(subtotal + tax, 2);

                order.OrderAmount = total;
                _db.Orders!.Update(order);

                await _db.SaveChangesAsync();

                orderId = order.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddOrder: {ex.Message}");
                throw;
            }

            return orderId;
        }

        public async Task<List<Order>> GetAll(int customerId)
        {
            return await _db.Orders!.Where(order => order.CustomerId == customerId).ToListAsync();
        }

        public async Task<List<OrderDetailsHelper>> GetOrderDetails(int orderId, string email)
        {
            Customer? customer = await _db.Customers!.FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null) return new List<OrderDetailsHelper>();
            var results = from o in _db.Orders
                          join oli in _db.OrderLineItems! on o.Id equals oli.OrderId
                          join p in _db.Products! on oli.ProductId equals p.Id
                          where (o.CustomerId == customer.Id && o.Id == orderId)
                          select new OrderDetailsHelper
                          {
                              OrderId = o.Id,
                              CustomerId = customer.Id,
                              OrderAmount = o.OrderAmount,
                              ProductName = p.ProductName,
                              ProductId = oli.ProductId,
                              QtyOrdered = oli.QtyOrdered,
                              QtySold = oli.QtySold,
                              QtyBackOrdered = oli.QtyBackOrdered,
                              SellingPrice = oli.SellingPrice,
                              DateCreated = DateTime.SpecifyKind(o.OrderDate, DateTimeKind.Utc)
                          };

            return await results.ToListAsync();
        }
    }
}
