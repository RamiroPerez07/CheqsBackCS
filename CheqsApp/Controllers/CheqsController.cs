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
    public class CheqsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public CheqsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Cheqs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cheq>>> GetCheqs()
        {
            return await _context.Cheqs.ToListAsync();
        }

        // GET: api/Cheqs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cheq>> GetCheq(int id)
        {
            var cheq = await _context.Cheqs.FindAsync(id);

            if (cheq == null)
            {
                return NotFound();
            }

            return cheq;
        }

        // PUT: api/Cheqs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCheq(int id, Cheq cheq)
        {
            if (id != cheq.Id)
            {
                return BadRequest();
            }

            _context.Entry(cheq).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheqExists(id))
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

        // POST: api/Cheqs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cheq>> PostCheq(Cheq cheq)
        {
            _context.Cheqs.Add(cheq);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCheq", new { id = cheq.Id }, cheq);
        }

        // DELETE: api/Cheqs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCheq(int id)
        {
            var cheq = await _context.Cheqs.FindAsync(id);
            if (cheq == null)
            {
                return NotFound();
            }

            _context.Cheqs.Remove(cheq);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCheqs([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return BadRequest("No se proporcionaron IDs.");
            }

            var cheqs = await _context.Cheqs.Where(cheq => ids.Contains(cheq.Id)).ToListAsync();

            if (cheqs.Count == 0)
            {
                return NotFound("No se encontraron cheques con los IDs proporcionados.");
            }

            _context.Cheqs.RemoveRange(cheqs);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CheqExists(int id)
        {
            return _context.Cheqs.Any(e => e.Id == id);
        }

        [HttpGet("getCheqsWithDetails")]
        public async Task<ActionResult<IEnumerable<CheqDetail>>> GetCheqsWithDetails()
        {
            var query = from ch in _context.Cheqs
                        join bu in _context.BusinessUser on ch.BusinessUserId equals bu.Id into businessUserGroup
                        from bu in businessUserGroup.DefaultIfEmpty() // Left Join
                        join u in _context.Users on bu.UserId equals u.Id into userGroup
                        from u in userGroup.DefaultIfEmpty() // Left Join
                        join b in _context.Businesses on bu.BusinessId equals b.Id into businessGroup
                        from b in businessGroup.DefaultIfEmpty() // Left Join
                        join t in _context.Types on ch.TypeId equals t.Id into typeGroup
                        from t in typeGroup.DefaultIfEmpty() // Left Join
                        join s in _context.States on ch.StateId equals s.Id into stateGroup
                        from s in stateGroup.DefaultIfEmpty() // Left Join
                        join e in _context.Entities on ch.EntityId equals e.Id into entityGroup
                        from e in entityGroup.DefaultIfEmpty() // Left Join
                        where b.Id == 1 // Aquí el filtro de BusinessId que deseas
                        select new CheqDetail
                        {
                            CheqId = ch.Id,
                            IssueDate = ch.IssueDate,
                            CreatedAt = ch.CreatedAt,
                            DueDate = ch.DueDate,
                            Amount = ch.Amount,
                            CheqNumber = ch.CheqNumber,
                            UserId = u.Id,
                            Username = u.Username,
                            Email = u.Email,
                            BusinessId = b.Id,
                            BusinessName = b.BusinessName,
                            Balance = b.Balance,
                            TypeId = t.Id,
                            TypeName = t.TypeName,
                            StateId = s.Id,
                            StateName = s.StateName,
                            EntityId = e.Id,
                            EntityName = e.EntityName
                        };

            var result = await query.ToListAsync();

            return Ok(result);
        }
    }
}
