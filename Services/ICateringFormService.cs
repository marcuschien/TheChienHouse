using TheChienHouse.Models;

namespace TheChienHouse.Services
{
    public interface ICateringFormService
    {
        Task<CateringFormDTO.CateringFormCreateResponse> CreateCateringFormAsync(CateringFormDTO.CateringFormCreateRequest request);
        Task<CateringForm?> GetCateringFormByIdAsync(Guid id);
        Task<CateringFormDTO.CateringFormCreateResponse?> UpdateCateringFormAsync(CateringFormDTO.CateringFormCreateRequest request);
        Task<bool> DeleteCateringFormAsync(Guid id);
        Task<IEnumerable<CateringForm>> GetCateringFormsAsync(Guid? clientId = null, Status? status = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}
