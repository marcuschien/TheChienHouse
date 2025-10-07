using Microsoft.EntityFrameworkCore;
using TheChienHouse.Models;
using static TheChienHouse.Models.CateringFormDTO;

namespace TheChienHouse.Services
{
    public class CateringFormService : ICateringFormService
    {
        private readonly RetailContext _context;
        private readonly ILogger<CateringFormService> _logger;

        public CateringFormService(RetailContext context, ILogger<CateringFormService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<CateringFormCreateResponse> CreateCateringFormAsync(CateringFormCreateRequest request)
        {
            var newCateringForm = new CateringForm
            {
                Id = Guid.NewGuid(),
                CateringType = request.CateringType,
                DietaryRestrictions = request.DietaryRestrictions,
                ClientId = request.ClientId,
                EventDate = request.EventDate,
                ClientName = request.ClientName,
                ClientEmail = request.ClientEmail,
                ClientPhoneNumber = request.ClientPhoneNumber,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };
            _context.CateringForms.Add(newCateringForm);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new catering form with ID: {CateringFormId}", newCateringForm.Id);

            return MapToResponse(newCateringForm);
        }

        public async Task<bool> DeleteCateringFormAsync(Guid id)
        {
            CateringForm? form = await _context.CateringForms.FindAsync(id);
            if (form == null)
            {
                _logger.LogWarning("Catering form with ID: {CateringFormId} not found for deletion", id);
                return false;
            }
            else
            {
                _context.CateringForms.Remove(form);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted catering form with ID: {CateringFormId}", id);
                return true;
            }
        }

        public async Task<CateringForm?> GetCateringFormByIdAsync(Guid id)
        {
            CateringForm? form = await _context.CateringForms.FindAsync(id);
            if (form == null)
            {
                _logger.LogWarning("Catering form with ID: {CateringFormId} not found", id);
                return null;
            }
            else
            {
                return form;
            }
        }

        public async Task<IEnumerable<CateringForm>> GetCateringFormsByClientIdAsync(Guid clientId) // Why do I want this to be async?
        {
            List<CateringForm> cateringForms = await _context.CateringForms.ToListAsync(); //Optimization: Filter at the database level instead of in-memory.
            return cateringForms.Where(cf => cf.ClientId == clientId);
        }

        public async Task<IEnumerable<CateringForm>> GetCateringFormsByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? clientId = null)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be earlier than or equal to end date.");
            }
            if (clientId.HasValue)
            {
                List<CateringForm> cateringForms = GetCateringFormsByClientIdAsync(clientId.Value).Result.ToList();
                return cateringForms.Where(cf => cf.EventDate >= startDate && cf.EventDate <= endDate);
            }
            else
            {
                List<CateringForm> cateringForms = await _context.CateringForms.ToListAsync();
                return cateringForms.Where(cf => cf.EventDate >= startDate && cf.EventDate <= endDate);
            }
        }

        public async Task<IEnumerable<CateringForm>> GetCateringFormsByStatusAsync(Status status, Guid? clientId = null)
        {
            if (clientId.HasValue)
            {
                List<CateringForm> cateringForms = GetCateringFormsByClientIdAsync(clientId.Value).Result.ToList();
                return cateringForms.Where(cf => cf.Status == status);
            }
            else
            {
                List<CateringForm> cateringForms = await _context.CateringForms.ToListAsync();
                return cateringForms.Where(cf => cf.Status == status);
            }
        }

        public async Task<CateringFormCreateResponse?> UpdateCateringFormAsync(CateringFormCreateRequest request)
        {
            CateringForm? form = await _context.CateringForms.FindAsync(request.Id);
            if (form == null)
            {
                _logger.LogWarning("Catering form with ID: {CateringFormId} not found for update", request.Id);
                return null;
            }
            else
            {
                form.CateringType = request.CateringType;
                form.DietaryRestrictions = request.DietaryRestrictions;
                form.ClientId = request.ClientId;
                form.EventDate = request.EventDate;
                form.ClientName = request.ClientName;
                form.ClientEmail = request.ClientEmail;
                form.ClientPhoneNumber = request.ClientPhoneNumber;
                form.Status = request.Status;
                form.UpdatedAt = DateTime.UtcNow;
                _context.CateringForms.Update(form);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated catering form with ID: {CateringFormId}", request.Id);
                return MapToResponse(form);
            }
        }

        private static CateringFormCreateResponse MapToResponse(CateringForm cateringForm) => new(cateringForm.Id, cateringForm.CateringType, cateringForm.DietaryRestrictions, cateringForm.ClientId, cateringForm.EventDate, cateringForm.ClientName, cateringForm.ClientEmail, cateringForm.ClientPhoneNumber, cateringForm.Status, cateringForm.CreatedAt, cateringForm.UpdatedAt);
    }
}
