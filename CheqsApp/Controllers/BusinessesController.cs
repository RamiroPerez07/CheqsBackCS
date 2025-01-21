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
    public class BusinessesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public BusinessesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Businesses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Business>>> GetBusinesses()
        {
            return await _context.Businesses.ToListAsync();
        }

        // GET: api/Businesses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Business>> GetBusiness(int id)
        {
            var business = await _context.Businesses.FindAsync(id);

            if (business == null)
            {
                return NotFound();
            }

            return business;
        }

        // PUT: api/Businesses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusiness(int id, Business business)
        {
            if (id != business.Id)
            {
                return BadRequest();
            }

            _context.Entry(business).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusinessExists(id))
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

        // POST: api/Businesses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Business>> PostBusiness(Business business)
        {
            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBusiness", new { id = business.Id }, business);
        }

        // DELETE: api/Businesses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusiness(int id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound();
            }

            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BusinessExists(int id)
        {
            return _context.Businesses.Any(e => e.Id == id);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<IEnumerable<BusinessesByUserDTO>>> GetBusinessesByUser(int userId)
        {
            var businesses = await (from bus in _context.Businesses
                                    join bb in _context.BankBusinesses on bus.Id equals bb.BusinessId into bbJoin
                                    from bb in bbJoin.DefaultIfEmpty()
                                    join bbu in _context.BankBusinessUsers on bb.Id equals bbu.BankBusinessId into bbuJoin
                                    from bbu in bbuJoin.DefaultIfEmpty()
                                    where bbu.UserId == userId
                                    select new BusinessesByUserDTO
                                    {
                                        BusinessId = bus.Id,
                                        BusinessName = bus.BusinessName
                                    })
                        .GroupBy(b => b.BusinessId) // Agrupar por BusinessId
                        .Select(g => g.First())      // Seleccionar solo el primer resultado de cada grupo
                        .ToListAsync();

            if (businesses == null || businesses.Count == 0)
            {
                return NotFound("No businesses found for the given user.");
            }

            return businesses;
        }

    }
}
