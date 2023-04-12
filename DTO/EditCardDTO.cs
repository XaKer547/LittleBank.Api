using LittleBank.Api.Models.Enums;

namespace LittleBank.Api.DTO
{
    public class EditCardDTO
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public OperationTypes Operation { get; set; }
    }
}