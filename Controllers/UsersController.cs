using LittleBank.Api.Database;
using LittleBank.Api.DTO;
using LittleBank.Api.Models;
using LittleBank.Api.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleBank.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? cardType, bool? AllowBlocked)
        {
            var users = _context.Users.AsQueryable();

            if (cardType.HasValue)
                users = users.Where(u => u.Cards.Any(c => c.Type == (CardTypes)cardType.Value));

            if (AllowBlocked == true)
                users = users.Where(u => u.Cards.Any(c => c.IsActive == true));

            return Ok(await users.Select(u => new
            {
                u.Id,
                u.Name
            }).ToArrayAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.Login,
                user.Name
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == dto.Login);

            if (user != null)
                return BadRequest("Пользователь с таким логином существует");

            user = new User()
            {
                Name = dto.Name,
                Login = dto.Login,
                Password = dto.Password,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.Id);

            if (user is null)
                return NotFound();

            user.Login = dto.Login;
            user.Name = dto.Name;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return NotFound();

            var cardsCheck = await _context.Cards.AnyAsync(c => c.User.Id == user.Id && c.IsActive == true);

            if (cardsCheck)
                return BadRequest("У данного пользователя имеются активные карты");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}