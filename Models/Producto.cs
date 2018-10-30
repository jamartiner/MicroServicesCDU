using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MicroServicesCDU.Models
{
    public class Producto
    {
        [Key]
        [StringLength(40)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Numero { set; get; }

        [Required]
        [StringLength(30)]
        public string Marca { set; get; }

        [Required]
        [Range(0, 99999999.999)]
        [DefaultValue(700000)]
        public decimal Cupo { set; get; }

        [Required]
        public DateTime FechaExpedicion { set; get; }

        [Required]
        public DateTime FechaVencimiento { set; get; }

        [Required]
        [Range(0, 1.00)]
        [DefaultValue(0.02)]
        public decimal TasaInteres { set; get; }

        [Required]
        [StringLength(2)]
        [DefaultValue(17)]
        public int FechaCorte { set; get; }

        [Required]
        [Range(0, 1.00)]
        [DefaultValue(0.10)]
        public decimal PorcAdicAvance { set; get; }

        [Required]
        [Range(0, 99999999.999)]
        public decimal PagoMinimo { set; get; }

        [Required]
        [StringLength(15)]
        public string Estado { set; get; }

        public List<Movimiento> Movimientos { set; get; }

        public long idCliente { set; get; }
        public Cliente Cliente { get; set; }
    }
}