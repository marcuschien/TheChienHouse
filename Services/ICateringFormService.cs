using TheChienHouse.Models;
using static TheChienHouse.Models.CateringFormDTO;

namespace TheChienHouse.Services
{
    public interface ICateringFormService
    {
        Task<CateringFormCreateResponse> CreateCateringFormAsync(CateringFormCreateRequest request);
        Task<CateringForm?> GetCateringFormByIdAsync(Guid id);
        Task<CateringFormCreateResponse> UpdateCateringFormAsync(CateringFormCreateRequest request);
        Task<bool> DeleteCateringFormAsync(Guid id);
        Task<IEnumerable<CateringForm>> GetCateringFormsAsync(Guid? clientId = null, Status? status = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}
