using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    //Admin profile may want to see all customer profiles. We can use this to autopopulate a profile for each client. 
    public class ContactForm
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public required string FirstName { get; set; }

        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        public required string Subject { get; set; }
        [Required(ErrorMessage = "Message is required")]
        public required string Message { get; set; }
        public Guid? ClientId { get; set; }
        public DateTime CreatedAt { get; init; }
    }
}
