using LittleBank.Api.Database;
using LittleBank.Api.DTO;
using LittleBank.Api.Models;
using LittleBank.Api.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleBank.Api.Controllers
{
    [ApiController]
    [Route("api/users/{id}/[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{cardId:int}")]
        public async Task<IActionResult> Get(int id, int cardId)
        {
            var user = await _context.Users.Select(u => new
            {
                u.Id,
                u.Name,
                Card = u.Cards.FirstOrDefault(c => c.Id == cardId)
            }).FirstOrDefaultAsync(u => u.Id == id);

            if (!user.Card.IsActive)
                return BadRequest("Данная карта не является активной");

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _context.Users.Select(u => new
            {
                u.Id,
                u.Name,
                Cards = u.Cards.Where(c => c.IsActive != false).Select(c => new
                {
                    c.Id,
                    c.Number,
                    c.Type
                })
            }).FirstOrDefaultAsync(u => u.Id == id);

            return Ok(user);
        }

        [HttpGet("~/api/cards/types")]
        public IActionResult GetTypes()
        {
            var types = Enum.GetNames(typeof(CardTypes));
            return Ok(types);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCardDTO dto, int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return NotFound();

            var card = new Card()
            {
                Number = dto.Number,
                Type = dto.Type,
                User = user
            };

            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] EditCardDTO dto, int id)
        {
            var user = await _context.Users.Select(u => new
            {
                u.Id,
                Card = u.Cards.FirstOrDefault(c => c.Id == dto.Id)
            }).FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return NotFound("Данный пользователь не найден");

            if (user.Card is null)
                return NotFound("Карта не принадлежит данному пользователю");

            if (!user.Card.IsActive)
                return BadRequest("Данная карта не является активной");

            var card = user.Card;

            if (dto.Operation == OperationTypes.Withdraw)
            {
                card.Sum -= dto.Sum;
                if (card.Sum < 0)
                    return BadRequest("Баланс карты не может быть меньше 0");
            }
            else
                card.Sum += dto.Sum;

            _context.Cards.Update(card);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpHead("{cardId:int}")]
        public async Task<IActionResult> Block(int cardId, int id)
        {
            var user = await _context.Users.Select(u => new
            {
                u.Id,
                Card = u.Cards.FirstOrDefault(c => c.Id == cardId)
            }).FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return NotFound("Данный пользователь не найден");

            if (user.Card is null)
                return NotFound("Карта не принадлежит данному пользователю");

            if (!user.Card.IsActive)
                return BadRequest("Данная карта уже является не активной");

            user.Card.IsActive = false;

            _context.Cards.Update(user.Card);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}