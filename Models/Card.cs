using LittleBank.Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace LittleBank.Api.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public double Sum { get; set; }
        public CardTypes Type { get; set; }
        public bool IsActive { get; set; } = true;

        public User User { get; set; }
    }
}
