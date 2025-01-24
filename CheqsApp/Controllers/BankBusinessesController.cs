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
    public class BankBusinessesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public BankBusinessesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/BankBusinesses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankBusiness>>> GetBankBusinesses()
        {
            return await _context.BankBusinesses.ToListAsync();
        }

        // GET: api/BankBusinesses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BankBusiness>> GetBankBusiness(int id)
        {
            var bankBusiness = await _context.BankBusinesses.FindAsync(id);

            if (bankBusiness == null)
            {
                return NotFound();
            }

            return bankBusiness;
        }

        // PUT: api/BankBusinesses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankBusiness(int id, BankBusiness bankBusiness)
        {
            if (id != bankBusiness.Id)
            {
                return BadRequest();
            }

            _context.Entry(bankBusiness).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankBusinessExists(id))
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

        // POST: api/BankBusinesses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BankBusiness>> PostBankBusiness(BankBusiness bankBusiness)
        {
            _context.BankBusinesses.Add(bankBusiness);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBankBusiness", new { id = bankBusiness.Id }, bankBusiness);
        }

        // DELETE: api/BankBusinesses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankBusiness(int id)
        {
            var bankBusiness = await _context.BankBusinesses.FindAsync(id);
            if (bankBusiness == null)
            {
                return NotFound();
            }

            _context.BankBusinesses.Remove(bankBusiness);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BankBusinessExists(int id)
        {
            return _context.BankBusinesses.Any(e => e.Id == id);
        }


        [HttpPost("balance-by-bank-and-business")]
        public async Task<ActionResult<BankBusinessBalanceDTO>> GetBankBusinesses([FromBody] BankBusinessBalanceReqDTO request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            var BankId = request.BankId;

            var BusinessId = request.BusinessId;

            var bankBusiness = await _context.BankBusinesses
                                                .Where(bb => bb.BankId == BankId && bb.BusinessId == BusinessId)
                                                .OrderByDescending(bb => bb.UpdatedAt)
                                                .Join(
                                                    _context.Users,
                                                    bb => bb.UserId,
                                                    u => u.Id,
                                                    (bb, u) => new
                                                    {
                                                        bb.Balance,
                                                        bb.UpdatedAt,
                                                        u.Username,
                                                        UserId = u.Id
                                                    })
                                                .FirstOrDefaultAsync();

            if (bankBusiness == null)
            {
                return NotFound("No records found for the given bank and business.");
            }

            // Mapear el resultado al DTO
            var result = new BankBusinessBalanceDTO
            {
                Balance = bankBusiness.Balance,
                UpdatedAt = bankBusiness.UpdatedAt,
                Username = bankBusiness.Username,
                UserId = bankBusiness.UserId,
            };

            return Ok(result);
        }

        // PUT: api/BankBusinesses/update-balance
        [HttpPut("update-balance")]
        public async Task<IActionResult> UpdateBankBusinessBalance([FromBody] UpdateBankBusinessBalanceDTO request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            var bankBusiness = await _context.BankBusinesses
                .Where(bb => bb.BankId == request.BankId && bb.BusinessId == request.BusinessId)
                .FirstOrDefaultAsync();

            if (bankBusiness == null)
            {
                return NotFound("No records found for the given bank and business.");
            }

            // Convertimos la fecha recibida (en UTC) a la hora de Buenos Aires
            var buenosAiresTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            var localDate = TimeZoneInfo.ConvertTimeFromUtc(request.UpdatedAt, buenosAiresTimeZone);



            // Actualizar los valores
            bankBusiness.Balance = request.Balance;
            bankBusiness.UpdatedAt = localDate;
            bankBusiness.UserId = request.UserId;

            try
            {
                // Guardar los cambios
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating balance: " + ex.Message);
            }

            return Ok(bankBusiness); // Retorna el objeto actualizado como respuesta
        }

    }
}
