using Microsoft.AspNetCore.Identity;

namespace TorgovayaPloshadka.Models
{
    public class CustomUser : IdentityUser
    {
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Secsurname { get; set; }

        public int? DoljnostId { get; set; }

        public virtual Doljnost? Doljnost { get; set; }
    }
}
