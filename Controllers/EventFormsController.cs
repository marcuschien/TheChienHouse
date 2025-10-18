using Microsoft.AspNetCore.Mvc;
using TheChienHouse.Services;
using TheChienHouse.Models;
using static TheChienHouse.Models.EventFormDTO;

namespace TheChienHouse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventFormsController:ControllerBase
    {
        private readonly IEventFormService _eventFormService;

        public EventFormsController(IEventFormService eventFormService)
        {
            _eventFormService = eventFormService;
        }

        //GET: api/EventForms/{id}
        //TODO: Implement/Fix Me
        [HttpGet("form/{id}")]
        public async Task<ActionResult<EventForm>> GetEventForm(Guid id)
        {
            EventForm? response = await _eventFormService.GetEventFormByIdAsync(id);
            if (response == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetEventForm), response);
        }

        //GET: api/EventForms?clientId={clientId}&status={status}&startDate={startDate}&endDate={endDate}
        //TODO: Implement/Fix Me
        [HttpGet("forms/{clientId}&{status}&{startDate}&{endDate}")]
        public async Task<ActionResult<IEnumerable<EventForm>>> GetEventForms(Guid? clientId = null, Status? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            IEnumerable<EventForm> response = await _eventFormService.GetEventFormsAsync(clientId, status, startDate, endDate);
            return CreatedAtAction(nameof(GetEventForms), response);
        }

        [HttpPut]
        public async Task<ActionResult<EventForm>> UpdateEventForm(EventFormUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            EventFormCreateResponse? response = await _eventFormService.UpdateEventFormAsync(request);
            if (response == null)
                return NotFound(new { message = $"Event form with ID '{request.Id}' was not found." });
            return CreatedAtAction(nameof(UpdateEventForm), new { id = response.Id }, response);
        }

        //PUT: api/EventForms
        [HttpPost]
        public async Task<ActionResult<EventForm>> PostEventForm(EventFormCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            EventFormCreateResponse response = await _eventFormService.CreateEventFormAsync(request);
            return CreatedAtAction(nameof(UpdateEventForm), new { id = response.Id }, response);
        }

        



    }
}
