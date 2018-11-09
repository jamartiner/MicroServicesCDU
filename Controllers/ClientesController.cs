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
        [HttpPut("ActualizarCliente/{id}")]
        public async Task<IActionResult> ActualizarCliente([FromRoute] long id, [FromBody] Cliente cliente)
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
                    Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error de actualización.", Mensaje = "El cliente no existe." });
                }
                else
                {
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    return new JsonResult(new Respuesta() { Resultado = "Error de actualización.", Mensaje = "Ha ocurrido un error durante la actualización de los datos del cliente." });
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
                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    throw new Exception("Datos incorrectos.");
                }

                string strClave = HashMD5.FromHexString(clave);
                strClave = HashMD5.CreateMD5(strClave);

                var cliente = await _context.Cliente.FirstOrDefaultAsync(e => e.Identificacion == identificacion && e.Clave == strClave);

                if (cliente == null)
                {
                    //return NotFound();
                    throw new Exception("Datos incorrectos.");
                }

                cliente.Clave = string.Empty;
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error de autenticación con clave.", Mensaje = ex.Message });
            }
        }

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

                cliente.Clave = string.Empty;
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error de autenticación con los 4 últimos dígitos de uno de los productos.", Mensaje = ex.Message });
            }
        }

        [HttpGet("ActualizarCliente/{id}/{ce}/{cel}/{tf}/{dir}/{bar}/{mu}/{dp}")]
        public async Task<IActionResult> ActualizarCliente([FromRoute] long id, [FromRoute] string ce, [FromRoute] string cel, [FromRoute] string tf, [FromRoute] string dir, [FromRoute] string bar, [FromRoute] string mu, [FromRoute] string dp)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Identificacion == id);
            cliente.CorreoElectronico = ce;
            cliente.Celular = cel;
            cliente.TelFijo = tf;
            cliente.Direccion = dir.Replace("NRO", "#");
            cliente.Barrio = bar;
            cliente.Municipio = mu;
            cliente.Departamento = dp;

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
    }
}