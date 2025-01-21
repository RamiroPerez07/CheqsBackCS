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
    public class BanksController : ControllerBase
    {
        private readonly AppDBContext _context;

        public BanksController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Banks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bank>>> GetBanks()
        {
            return await _context.Banks.ToListAsync();
        }

        // GET: api/Banks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bank>> GetBank(int id)
        {
            var bank = await _context.Banks.FindAsync(id);

            if (bank == null)
            {
                return NotFound();
            }

            return bank;
        }

        // PUT: api/Banks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBank(int id, Bank bank)
        {
            if (id != bank.Id)
            {
                return BadRequest();
            }

            _context.Entry(bank).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankExists(id))
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

        // POST: api/Banks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bank>> PostBank(Bank bank)
        {
            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBank", new { id = bank.Id }, bank);
        }

        // DELETE: api/Banks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound();
            }

            _context.Banks.Remove(bank);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BankExists(int id)
        {
            return _context.Banks.Any(e => e.Id == id);
        }


        [HttpGet("by-user/{userId}/by-business/{businessId}")]
        public async Task<ActionResult<IEnumerable<BanksDTO>>> GetBanksByUserAndBusiness(int userId, int businessId)
        {
            var banks = await (from b in _context.Banks
                               join bb in _context.BankBusinesses on b.Id equals bb.BankId into bbJoin
                               from bb in bbJoin.DefaultIfEmpty()
                               join bbu in _context.BankBusinessUsers on bb.Id equals bbu.BankBusinessId into bbuJoin
                               from bbu in bbuJoin.DefaultIfEmpty()
                               join u in _context.Users on bbu.UserId equals u.Id into uJoin
                               from u in uJoin.DefaultIfEmpty()
                               join bus in _context.Businesses on bb.BusinessId equals bus.Id into busJoin
                               from bus in busJoin.DefaultIfEmpty()
                               where u.Id == userId && bus.Id == businessId
                               select new BanksDTO
                               {
                                   BankId = b.Id,
                                   BankName = b.BankName
                               })
                   .GroupBy(b => b.BankId)  // Agrupar por BankId
                   .Select(g => g.First())   // Seleccionar solo el primer resultado de cada grupo
                   .ToListAsync();

            if (banks == null || banks.Count == 0)
            {
                return NotFound("No banks found for the given user and business.");
            }

            return banks;
        }
    }
}
