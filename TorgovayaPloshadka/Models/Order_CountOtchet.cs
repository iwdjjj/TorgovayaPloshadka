using Microsoft.EntityFrameworkCore;

namespace TorgovayaPloshadka.Models
{
    [Keyless]
    public class Order_CountOtchet
    {
        public int? id { get; set; }
        public string? nm { get; set; }
        public int? kol { get; set; }
    }
}
