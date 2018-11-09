using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroServicesCDU.Models;
using MicroServicesCDU.Data;
using MicroServicesCDU.Utilities;

namespace MicroServicesCDU.Controllers
{
    [BasicAuthorize("localhost")]
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly MicroServicesCDUContext _context;

        public MovimientosController(MicroServicesCDUContext context)
        {
            _context = context;
        }

        // GET: api/Movimientos
        [HttpGet]
        public IEnumerable<Movimiento> GetMovimiento()
        {
            return _context.Movimiento;
        }

        // GET: api/Movimientos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovimiento([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movimiento = await _context.Movimiento.FindAsync(id);

            if (movimiento == null)
            {
                return NotFound();
            }

            return Ok(movimiento);
        }

        // PUT: api/Movimientos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovimiento([FromRoute] long id, [FromBody] Movimiento movimiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != movimiento.IdMovimiento)
            {
                return BadRequest();
            }

            _context.Entry(movimiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovimientoExists(id))
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

        // POST: api/Movimientos
        [HttpPost]
        public async Task<IActionResult> PostMovimiento([FromBody] Movimiento movimiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Movimiento.Add(movimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovimiento", new { id = movimiento.IdMovimiento }, movimiento);
        }

        // DELETE: api/Movimientos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimiento([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movimiento = await _context.Movimiento.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }

            _context.Movimiento.Remove(movimiento);
            await _context.SaveChangesAsync();

            return Ok(movimiento);
        }

        private bool MovimientoExists(long id)
        {
            return _context.Movimiento.Any(e => e.IdMovimiento == id);
        }

        [HttpGet("ObtenerMovimientosProducto/{idProducto}/{cantidadDias}/{cantidadMovimientos}")]
        public async Task<IActionResult> ObtenerMovimientosProducto([FromRoute] long idProducto, [FromRoute] int cantidadDias, [FromRoute] int cantidadMovimientos)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var movimientos = await (from m in _context.Movimiento
                                         where m.idProducto == idProducto && DateTime.Now.Subtract(m.Fecha).Days <= cantidadDias
                                         orderby m.Fecha descending
                                         select m).ToListAsync();

                if (movimientos == null)
                {
                    //return NotFound();
                    throw new Exception("No se encontraron movimientos para este producto.");
                }

                if (cantidadMovimientos > 0 && movimientos.Count > cantidadMovimientos)
                {
                    movimientos = movimientos.GetRange(0, cantidadMovimientos);
                }

                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error consultando los movimientos del producto.", Mensaje = ex.Message });
            }
        }
    }
}