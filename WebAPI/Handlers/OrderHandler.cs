using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Handlers;

public class OrderHandler
{
    private readonly ApplicationDbContext _context;

    public OrderHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<double> GetPriceForProductOrders(ProductOrderModel[] productOrders)
    {
        double price = 0;
        foreach (ProductOrderModel productOrder in productOrders)
        {
            var product = await _context.Products.FirstOrDefaultAsync(product => product.Id == productOrder.ProductId);
            
            if (product == null)
                continue;

            price += product.Price;
        }
        return price;
    }
}