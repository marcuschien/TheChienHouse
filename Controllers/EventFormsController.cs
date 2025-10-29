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
        public async Task<ActionResult<EventFormResponse>> GetEventForm(EventFormRequest request)
        {
            EventForm ? eventForm = null;
            if (request.Id.HasValue)
            {
                eventForm = await _eventFormService.GetEventFormByIdAsync(request.Id.Value);
            }
            else
            {
                return BadRequest(new { message = "Event Form ID is required." });
            }
            if (eventForm == null)
            {
                return NotFound();
            }
            else
            {
                return CreatedAtAction(nameof(GetEventForm), MapToResponse(eventForm));
            }
                
        }

        //GET: api/EventForms?clientId={clientId}&status={status}&startDate={startDate}&endDate={endDate}
        //TODO: Implement/Fix Me
        [HttpGet("forms/{clientId}&{status}&{startDate}&{endDate}")]
        public async Task<ActionResult<IEnumerable<EventFormsResponse>>> GetEventForms(EventFormRequest request)
        {
            IEnumerable<EventForm> eventForms = await _eventFormService.GetEventFormsAsync(request.ClientId, request.Status, request.StartDate, request.EndDate);
            return CreatedAtAction(nameof(GetEventForms), new EventFormsResponse(eventForms));
        }

        [HttpPut]
        public async Task<ActionResult<EventFormCreateResponse>> UpdateEventForm(EventFormUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            EventForm? eventForm = await _eventFormService.UpdateEventFormAsync(request.Id, request.EventType, request.DietaryRestrictions, request.ClientId, request.EventDate, request.FirstName, request.LastName, request.Status, request.Location, request.ClientEmail, request.ClientPhoneNumber, request.BudgetPerPerson, request.NumberOfGuests, request.ExtraNotes);
            if (eventForm == null)
                return NotFound(new { message = $"Event form with ID '{request.Id}' was not found." });
            return CreatedAtAction(nameof(UpdateEventForm), MapToCreateResponse(eventForm));
        }

        //PUT: api/EventForms
        [HttpPost]
        public async Task<ActionResult<EventFormCreateResponse>> PostEventForm(EventFormCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            EventForm eventForm = await _eventFormService.CreateEventFormAsync(request.EventType, request.DietaryRestrictions, request.ClientId, request.EventDate, request.FirstName, request.LastName, request.Status, request.Location, request.ClientEmail, request.ClientPhoneNumber, request.BudgetPerPerson, request.NumberOfGuests, request.ExtraNotes);
            return CreatedAtAction(nameof(UpdateEventForm), MapToCreateResponse(eventForm));
        }

        private static EventFormResponse MapToResponse(EventForm eventForm) => new(eventForm.Id, eventForm.EventType, eventForm.DietaryRestrictions, eventForm.ClientId, eventForm.EventDate, eventForm.FirstName, eventForm.LastName, eventForm.ClientEmail, eventForm.ClientPhoneNumber, eventForm.Status, eventForm.Location, eventForm.BudgetPerPerson, eventForm.NumberOfGuests, eventForm.ExtraNotes, eventForm.CreatedAt, eventForm.UpdatedAt);

        private static EventFormCreateResponse MapToCreateResponse(EventForm eventForm) => new(eventForm.Id, eventForm.EventType, eventForm.DietaryRestrictions, eventForm.ClientId, eventForm.EventDate, eventForm.FirstName, eventForm.LastName, eventForm.ClientEmail, eventForm.ClientPhoneNumber, eventForm.Status, eventForm.CreatedAt, eventForm.UpdatedAt, eventForm.Location, eventForm.BudgetPerPerson, eventForm.NumberOfGuests, eventForm.ExtraNotes);
    }
}
