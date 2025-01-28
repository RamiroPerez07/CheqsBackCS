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
using Azure.Core;
using Mono.TextTemplating;

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
        public async Task<ActionResult<Cheq>> PostCheq(CreateCheqDTO cheq)
        {
            // Recuperamos las entidades correspondientes desde la base de datos
            var entity = await _context.Entities.FindAsync(cheq.EntityId);
            var type = await _context.Types.FindAsync(cheq.TypeId);
            var state = await _context.States.FindAsync(cheq.StateId);
            var bankBusinessUser = await _context.BankBusinessUsers.FindAsync(cheq.BankBusinessUserId);

            // Verificamos que todas las entidades existan
            if (entity == null || type == null || state == null || bankBusinessUser == null)
            {
                return NotFound("Una o más entidades no fueron encontradas.");
            }

            // Convertimos la fecha recibida (en UTC) a la hora de Buenos Aires
            var buenosAiresTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            cheq.IssueDate = TimeZoneInfo.ConvertTimeFromUtc(cheq.IssueDate, buenosAiresTimeZone);
            cheq.DueDate = TimeZoneInfo.ConvertTimeFromUtc(cheq.DueDate, buenosAiresTimeZone);
            cheq.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(cheq.CreatedAt, buenosAiresTimeZone);


            var newCheq = new Cheq
            {
                Amount = cheq.Amount,
                BankBusinessUserId = cheq.BankBusinessUserId,
                CheqNumber = cheq.CheqNumber,
                DueDate = cheq.DueDate,
                CreatedAt = cheq.CreatedAt,
                IssueDate = cheq.IssueDate,
                EntityId = cheq.EntityId,
                TypeId = cheq.TypeId,
                StateId = cheq.StateId,
                Entity = entity,
                State = state,
                Type = type,
                BankBusinessUser = bankBusinessUser
            };


            _context.Cheqs.Add(newCheq);
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
        public async Task<ActionResult<IEnumerable<CheqDetail>>> GetCheqsWithDetails([FromQuery] int bankId, [FromQuery] int businessId)
        {
            var query = from ch in _context.Cheqs
                        join bbu in _context.BankBusinessUsers on ch.BankBusinessUserId equals bbu.Id into bbuGroup
                        from bbu in bbuGroup.DefaultIfEmpty()
                        join u in _context.Users on bbu.UserId equals u.Id into uGroup
                        from u in uGroup.DefaultIfEmpty()
                        join bb in _context.BankBusinesses on bbu.BankBusinessId equals bb.Id into bbGroup
                        from bb in bbGroup.DefaultIfEmpty()
                        join bus in _context.Businesses on bb.BusinessId equals bus.Id into busGroup
                        from bus in busGroup.DefaultIfEmpty()
                        join b in _context.Banks on bb.BankId equals b.Id into bGroup
                        from b in bGroup.DefaultIfEmpty()
                        join t in _context.Types on ch.TypeId equals t.Id into tGroup
                        from t in tGroup.DefaultIfEmpty()
                        join s in _context.States on ch.StateId equals s.Id into sGroup
                        from s in sGroup.DefaultIfEmpty()
                        join e in _context.Entities on ch.EntityId equals e.Id into eGroup
                        from e in eGroup.DefaultIfEmpty()
                        where /*u.Id == userId && */ b.Id == bankId && bus.Id == businessId
                        select new
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
                            BusinessId = bus.Id,
                            BusinessName = bus.BusinessName,
                            Balance = bb.Balance,
                            TypeId = t.Id,
                            TypeName = t.TypeName,
                            StateId = s.Id,
                            StateName = s.StateName,
                            EntityId = e.Id,
                            EntityName = e.EntityName,
                            BankId = b.Id,
                            BankName = b.BankName
                        };

            var result = await query.ToListAsync();

            return Ok(result);
        }


        public class ChangeCheqsStateRequest
        {
            public required List<int> CheqIds { get; set; }
            public int NewStateId { get; set; }
        }

        // PUT: api/Cheqs/ChangeStateId
        [HttpPut("ChangeStateId")]
        public async Task<IActionResult> ChangeCheqsStateId([FromBody] ChangeCheqsStateRequest request)
        {
            // Verifica si los cheques existen
            var cheqs = await _context.Cheqs.Where(c => request.CheqIds.Contains(c.Id)).ToListAsync();
            if (cheqs.Count != request.CheqIds.Count)
            {
                return NotFound("Uno o más cheques no encontrados.");
            }

            // Verifica si el nuevo StateId es válido
            var stateExists = await _context.States.AnyAsync(s => s.Id == request.NewStateId);
            if (!stateExists)
            {
                return BadRequest("El nuevo StateId no existe.");
            }

            // Cambia el StateId de los cheques
            foreach (var cheq in cheqs)
            {
                cheq.StateId = request.NewStateId;
                _context.Entry(cheq).State = EntityState.Modified;
            }

            // Guarda los cambios en la base de datos
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
    }

}
