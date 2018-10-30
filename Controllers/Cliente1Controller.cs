using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicesCDU.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroServicesCDU.Controllers
{
    [Route("api/cliente1")]
    [ApiController]
    public class Cliente1Controller : ControllerBase
    {
        [HttpPost("AutenticarConClave")]
        public ActionResult<Cliente> AutenticarConClave(Cliente cliente)
        {
            try
            {
                Cliente cliente1 = new Cliente() {
                    Identificacion = cliente.Identificacion,
                    Clave = cliente.Clave         
                };

                return cliente1;
            }
            catch (Exception ex)
            {
                return new JsonResult(new Respuesta() { Resultado = "Error", Mensaje = ex.Message });
            }
        }

        [HttpPost("ObtenerRespuesta")]
        public ActionResult<Respuesta> ObtenerRespuesta(Respuesta r)
        {
            try
            {
                Respuesta r1 = new Respuesta();
                r1 = r;

                return r1;
            }
            catch (Exception ex)
            {
                return new JsonResult(new Respuesta() { Resultado = "Error", Mensaje = ex.Message });
            }
        }
    }
}