using LittleBank.Api.Models.Enums;

namespace LittleBank.Api.DTO
{
    public class CardDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Sum { get; set; }
        public string Number { get; set; }
        public CardTypes Type { get; set; }

        public OperationTypes Operation { get; set; }
    }
}
