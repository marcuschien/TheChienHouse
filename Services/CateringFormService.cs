using TheChienHouse.Models;

namespace TheChienHouse.Services
{
    public class CateringFormService : ICateringFormService
    {
        public Task<CateringFormDTO.CateringFormCreateResponse> CreateCateringFormAsync(CateringFormDTO.CateringFormCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCateringFormAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CateringForm?> GetCateringFormByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CateringForm>> GetCateringFormsByClientIdAsync(Guid clientId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CateringForm>> GetCateringFormsByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? clientId = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CateringForm>> GetCateringFormsByStatusAsync(Status status, Guid? clientId = null)
        {
            throw new NotImplementedException();
        }

        public Task<CateringFormDTO.CateringFormCreateResponse?> UpdateCateringFormAsync(CateringFormDTO.CateringFormCreateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
