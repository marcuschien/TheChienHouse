using Microsoft.AspNetCore.Mvc;
using TheChienHouse.Services;
using TheChienHouse.Models;
using static TheChienHouse.Models.CateringFormDTO;

namespace TheChienHouse.Controllers
{
    public class CateringFormsController:ControllerBase
    {
        private readonly ICateringFormService _cateringFormService;

        public CateringFormsController(ICateringFormService cateringFormService)
        {
            _cateringFormService = cateringFormService;
        }

        //GET: api/CateringForms/{id}
        //TODO: Implement/Fix Me
        [HttpGet("form/{id}")]
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
        [HttpGet("forms/{clientId}&{status}&{startDate}&{endDate}")]
        public async Task<ActionResult<IEnumerable<CateringForm>>> GetCateringForms(Guid? clientId = null, Status? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IEnumerable<CateringForm> response = await _cateringFormService.GetCateringFormsAsync(clientId, status, startDate, endDate);
            return CreatedAtAction(nameof(GetCateringForms), response);
        }

        //POST: api/CateringForms
        //TODO: Implement/Fix Me
        [HttpPost]
        public async Task<ActionResult<CateringForm>> UpdateCateringForm(CateringFormCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            CateringFormDTO.CateringFormCreateResponse response = await _cateringFormService.CreateCateringFormAsync(request);
            return CreatedAtAction(nameof(PostCateringForm), new { id = response.Id }, response);
        }

        //PUT: api/CateringForms
        //TODO: Implement/Fix Me
        [HttpPut]
        public async Task<ActionResult<CateringForm>> PostCateringForm(CateringFormCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            CateringFormDTO.CateringFormCreateResponse response = await _cateringFormService.CreateCateringFormAsync(request);
            return CreatedAtAction(nameof(UpdateCateringForm), new { id = response.Id }, response);
        }

        



    }
}
