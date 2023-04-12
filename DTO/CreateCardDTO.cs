using LittleBank.Api.Models.Enums;

namespace LittleBank.Api.DTO
{
    public class CreateCardDTO
    {
        public double Sum { get; set; }
        public string Number { get; set; }
        public CardTypes Type { get; set; }
    }
}