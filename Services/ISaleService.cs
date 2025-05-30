using TheChienHouse.Models;
using static TheChienHouse.Models.SaleDTO;

namespace TheChienHouse.Services
{
    public interface ISaleService
    {
        Task<SaleCreateResponse> CreateSaleAsync(SaleCreateRequest request);
    }
}
