using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheqsApp.Contexts;
using CheqsApp.Models;
using CheqsApp.DTO;

namespace CheqsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankBusinessUsersController : ControllerBase
    {
        private readonly AppDBContext _context;

        public BankBusinessUsersController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/BankBusinessUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankBusinessUser>>> GetBankBusinessUsers()
        {
            return await _context.BankBusinessUsers.ToListAsync();
        }

        // GET: api/BankBusinessUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BankBusinessUser>> GetBankBusinessUser(int id)
        {
            var bankBusinessUser = await _context.BankBusinessUsers.FindAsync(id);

            if (bankBusinessUser == null)
            {
                return NotFound();
            }

            return bankBusinessUser;
        }

        // PUT: api/BankBusinessUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankBusinessUser(int id, BankBusinessUser bankBusinessUser)
        {
            if (id != bankBusinessUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(bankBusinessUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankBusinessUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BankBusinessUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BankBusinessUser>> PostBankBusinessUser(BankBusinessUser bankBusinessUser)
        {
            _context.BankBusinessUsers.Add(bankBusinessUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBankBusinessUser", new { id = bankBusinessUser.Id }, bankBusinessUser);
        }

        // DELETE: api/BankBusinessUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankBusinessUser(int id)
        {
            var bankBusinessUser = await _context.BankBusinessUsers.FindAsync(id);
            if (bankBusinessUser == null)
            {
                return NotFound();
            }

            _context.BankBusinessUsers.Remove(bankBusinessUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BankBusinessUserExists(int id)
        {
            return _context.BankBusinessUsers.Any(e => e.Id == id);
        }

        [HttpPost("by-user-business-bank")]
        public async Task<ActionResult<int>> GetBankBusinessUserId([FromBody] BankBusinessRequestDTO request)
        {
            var result = await (from bbu in _context.BankBusinessUsers
                                join bb in _context.BankBusinesses on bbu.BankBusinessId equals bb.Id into bbJoin
                                from bb in bbJoin.DefaultIfEmpty()
                                where bbu.UserId == request.UserId && bb.BankId == request.BankId && bb.BusinessId == request.BusinessId
                                select (int?)bbu.Id)  // Hacemos que el Id sea nullable
                        .Distinct()
                        .FirstOrDefaultAsync();  // Esto retornará un Nullable<int> o null

            if (result == null)
            {
                return NotFound("No user bank business relationship found for the given criteria.");
            }

            return Ok(result.Value);  // Devolvemos el valor como int
        }
    }
}
