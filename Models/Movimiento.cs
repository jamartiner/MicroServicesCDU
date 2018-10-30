using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MicroServicesCDU.Models
{
    public class Movimiento
    {
        [Key]
        public long IdMovimiento { set; get; }

        [Required]
        [Range(0, 99999999.999)]
        public decimal Valor { set; get; }

        [Required]
        public DateTime Fecha { set; get; }

        [Required]
        [StringLength(50)]
        public string Concepto { set; get; }

        public long idProducto { set; get; }
        public Producto Producto { set; get; }
    }
}