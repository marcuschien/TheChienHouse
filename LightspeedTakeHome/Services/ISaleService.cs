using LightspeedTakeHome.Models;
using static LightspeedTakeHome.Models.SaleDTO;

namespace LightspeedTakeHome.Services
{
    public interface ISaleService
    {
        Task<SaleCreateResponse> CreateSaleAsync(SaleCreateRequest request);
    }
}
