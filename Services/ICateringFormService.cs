using TheChienHouse.Models;

namespace TheChienHouse.Services
{
    public interface ICateringFormService
    {
        Task<CateringFormDTO.CateringFormCreateResponse> CreateCateringFormAsync(CateringFormDTO.CateringFormCreateRequest request);
        Task<CateringForm?> GetCateringFormByIdAsync(Guid id);
        Task<CateringFormDTO.CateringFormCreateResponse?> UpdateCateringFormAsync(CateringFormDTO.CateringFormCreateRequest request);
        Task<bool> DeleteCateringFormAsync(Guid id);
        Task<IEnumerable<CateringForm>> GetCateringFormsByClientIdAsync(Guid clientId);
        Task<IEnumerable<CateringForm>> GetCateringFormsByStatusAsync(Status status, Guid? clientId = null);
        Task<IEnumerable<CateringForm>> GetCateringFormsByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? clientId = null);
    }
}
