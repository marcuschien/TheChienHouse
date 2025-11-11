using TheChienHouse.Models;
using static TheChienHouse.Models.SaleDTO;

namespace TheChienHouse.Services
{
    public interface ISaleService
    {
        Task<Sale> CreateSaleAsync(List<LineItem> lineItems, decimal discount = 0);
        Task<List<Sale>> GetAllSalesAsync();
    }
}
