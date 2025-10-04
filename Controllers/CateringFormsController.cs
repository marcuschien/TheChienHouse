using Microsoft.AspNetCore.Mvc;
using TheChienHouse.Services;
using TheChienHouse.Models;

namespace TheChienHouse.Controllers
{
    public class CateringFormsController:ControllerBase
    {
        private readonly RetailContext _context;
        private readonly ICateringFormService _cateringFormService;

        public CateringFormsController(RetailContext context, ICateringFormService cateringFormService)
        {
            _context = context;
            _cateringFormService = cateringFormService;
        }

        //GET: api/CateringForms/{id}
        //TODO: Implement/Fix Me
        [HttpGet("{id}")]
        public async Task<ActionResult<CateringForm>> GetCateringForm(Guid id)
        {
            CateringForm? response = await _cateringFormService.GetCateringFormByIdAsync(id);
            if (response == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetCateringForm), response);
        }

        //GET: api/CateringForms?clientId={clientId}&status={status}&startDate={startDate}&endDate={endDate}
        //TODO: Implement/Fix Me
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<CateringForm>>> GetCateringFormsByClientId(Guid clientId, Status? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IEnumerable<CateringForm> response = await _cateringFormService.GetCateringFormsByClientIdAsync(clientId);
            return CreatedAtAction(nameof(GetCateringFormsByClientId), response);
        }


        //POST: api/CateringForms
        //TODO: Implement/Fix Me
        [HttpPost]
        public async Task<ActionResult<CateringForm>> UpdateCateringForm(CateringFormDTO.CateringFormCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            CateringFormDTO.CateringFormCreateResponse response = await _cateringFormService.CreateCateringFormAsync(request);
            return CreatedAtAction(nameof(PostCateringForm), new { id = response.Id }, response);
        }

        //PUT: api/CateringForms
        //TODO: Implement/Fix Me
        [HttpPut]
        public async Task<ActionResult<CateringForm>> PostCateringForm(CateringFormDTO.CateringFormCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            CateringFormDTO.CateringFormCreateResponse response = await _cateringFormService.CreateCateringFormAsync(request);
            return CreatedAtAction(nameof(UpdateCateringForm), new { id = response.Id }, response);
        }

        



    }
}
