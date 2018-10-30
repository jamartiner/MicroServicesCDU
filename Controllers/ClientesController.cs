using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroServicesCDU.Models;
using MicroServicesCDU.Data;

namespace MicroServicesCDU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly MicroServicesCDUContext _context;

        public ClientesController(MicroServicesCDUContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public IEnumerable<Cliente> GetCliente()
        {
            return _context.Cliente;
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCliente([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await _context.Cliente.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente([FromRoute] long id, [FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cliente.Identificacion)
            {
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
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

        // POST: api/Clientes
        [HttpPost]
        public async Task<IActionResult> PostCliente([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Cliente.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCliente", new { id = cliente.Identificacion }, cliente);
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        private bool ClienteExists(long id)
        {
            return _context.Cliente.Any(e => e.Identificacion == id);
        }

        [HttpGet("AutenticarClienteConClave/{identificacion}/{clave}")]
        public async Task<IActionResult> AutenticarClienteConClave([FromRoute] long identificacion, [FromRoute] string clave)
        {
            try
            {
                if (!ModelState.IsValid || clave.Length != 4)
                {
                    //return BadRequest(ModelState);
                    throw new Exception("Datos incorrectos.");
                }

                var cliente = await _context.Cliente.FirstOrDefaultAsync(e => e.Identificacion == identificacion && e.Clave == clave);

                if (cliente == null)
                {
                    //return NotFound();
                    throw new Exception("Datos incorrectos.");
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error de autenticación con clave.", Mensaje = ex.Message });
            }
        }

        /*[HttpPost("AutenticarClienteConClave2")]
        public async Task<IActionResult> AutenticarClienteConClave2(Autenticacion a)
        {
            try
            {
                if (!ModelState.IsValid || a.ClaveO4UltDigProd.Length != 4)
                {
                    //return BadRequest(ModelState);
                    throw new Exception("Datos incorrectos.");
                }

                var cliente = await _context.Cliente.FirstOrDefaultAsync(e => e.Identificacion == a.Identificacion && e.Clave == a.ClaveO4UltDigProd);

                if (cliente == null)
                {
                    //return NotFound();
                    throw new Exception("Datos incorrectos.");
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error de autenticación con clave.", Mensaje = ex.Message });
            }
        }*/

        [HttpGet("AutenticarClienteConProducto/{identificacion}/{UDProd}")]
        public async Task<IActionResult> AutenticarClienteConProducto([FromRoute] long identificacion, [FromRoute] string UDProd)
        {
            try
            {
                if (!ModelState.IsValid || UDProd.Length != 4)
                {
                    //return BadRequest(ModelState);
                    throw new Exception("Datos incorrectos.");
                }

                var cliente = await (from c in _context.Cliente
                                     where c.Identificacion == identificacion
                                     join p in _context.Producto on c.Identificacion equals p.idCliente
                                     where p.Numero.ToString().EndsWith(UDProd)
                                     select c).FirstOrDefaultAsync();

                if (cliente == null)
                {
                    //return NotFound();
                    throw new Exception("Datos incorrectos.");
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error de autenticación con los 4 últimos dígitos de uno de los productos.", Mensaje = ex.Message });
            }
        }
    }
}