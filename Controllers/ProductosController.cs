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
    public class ProductosController : ControllerBase
    {
        private readonly MicroServicesCDUContext _context;

        public ProductosController(MicroServicesCDUContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        [HttpGet]
        public IEnumerable<Producto> GetProducto()
        {
            return _context.Producto;
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProducto([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var producto = await _context.Producto.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto([FromRoute] long id, [FromBody] Producto producto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != producto.Numero)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
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

        // POST: api/Productos
        [HttpPost]
        public async Task<IActionResult> PostProducto([FromBody] Producto producto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Producto.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new { id = producto.Numero }, producto);
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        private bool ProductoExists(long id)
        {
            return _context.Producto.Any(e => e.Numero == id);
        }

        [HttpGet("ObtenerProductosActivosCliente/{idCliente}")]
        public async Task<IActionResult> ObtenerProductosActivosCliente([FromRoute] long idCliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productos = await (from c in _context.Cliente
                                       join p in _context.Producto on c.Identificacion equals p.idCliente
                                       where c.Identificacion == idCliente && p.Estado == EstadosProducto.Activo.ToString()
                                       select p).ToListAsync();

                if (productos == null || productos.Count == 0)
                {
                    //return NotFound();
                    throw new Exception("No se encontraron productos para este cliente.");
                }               

                return Ok(productos);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error consultando los productos del cliente.", Mensaje = ex.Message });
            }
        }

        [HttpGet("ObtenerCupoCompra/{idProducto}")]
        public async Task<IActionResult> ObtenerCupoCompra([FromRoute] long idProducto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string query = "select (isnull(cupo, 0) + deuda.deuda) CupoCompra" +
                               " from producto," +
                               "      (select isnull(sum(valor), 0) deuda" +
                               "       from movimiento" +
                               "       where idProducto = " + idProducto + ") as deuda" +
                               " where Numero = " + idProducto;

                decimal cupoCompra = 0;

                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        var result = await command.ExecuteScalarAsync();

                        if (result == null)
                        {
                            connection.Dispose();
                            command.Dispose();
                            //return NotFound();
                            throw new Exception("No se fue posible obtener el cupo de compra de este producto.");
                        }

                        cupoCompra = Convert.ToDecimal(result);
                    }
                }

                return Ok(cupoCompra);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error consultando el cupo de compra del producto.", Mensaje = ex.Message });
            }
        }

        [HttpGet("ObtenerCupoAvance/{idProducto}")]
        public async Task<IActionResult> ObtenerCupoAvance([FromRoute] long idProducto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string query = "select ((isnull(cupo, 0) + deuda.deuda) + ((isnull(cupo, 0) + deuda.deuda) * PorcAdicAvance)) CupoAvance" +
                               " from producto," +
                               "      (select isnull(sum(valor), 0) deuda" +
                               "       from movimiento" +
                               "       where idProducto = " + idProducto + ") as deuda" +
                               " where Numero = " + idProducto;

                decimal cupoAvance = 0;

                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        var result = await command.ExecuteScalarAsync();

                        if (result == null)
                        {
                            connection.Dispose();
                            command.Dispose();
                            //return NotFound();
                            throw new Exception("No se fue posible obtener el cupo de avance de este producto.");
                        }

                        cupoAvance = Convert.ToDecimal(result);
                    }
                }

                return Ok(cupoAvance);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error consultando el cupo de avance del producto.", Mensaje = ex.Message });
            }
        }

        [HttpGet("ObtenerValorPagoMinimo/{idProducto}")]
        public async Task<IActionResult> ObtenerValorPagoMinimo([FromRoute] long idProducto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var producto = await (from p in _context.Producto
                                      where p.Numero == idProducto
                                      select p).FirstOrDefaultAsync();

                if (producto == null)
                {
                    //return NotFound();
                    throw new Exception("No se fue posible obtener el valor de pago mínimo de este producto.");
                }

                var valorPagoMinimo = producto.PagoMinimo;

                return Ok(valorPagoMinimo);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error consultando el valor de pago mínimo del producto.", Mensaje = ex.Message });
            }
        }

        [HttpGet("ObtenerValorPagoTotal/{idProducto}")]
        public async Task<IActionResult> ObtenerValorPagoTotal([FromRoute] long idProducto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var producto = await (from p in _context.Producto
                                      where p.Numero == idProducto
                                      select p).FirstOrDefaultAsync();

                if (producto == null)
                {
                    //return NotFound();
                    throw new Exception("No se fue posible obtener el valor de pago total de este producto.");
                }

                var valorPagoTotal = await (from m in _context.Movimiento
                                            where m.idProducto == producto.Numero
                                            select m.Valor).SumAsync() + producto.Cupo;

                valorPagoTotal = producto.Cupo - valorPagoTotal;

                return Ok(valorPagoTotal);
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new JsonResult(new Respuesta() { Resultado = "Error consultando el valor de pago total del producto.", Mensaje = ex.Message });
            }
        }
    }
}