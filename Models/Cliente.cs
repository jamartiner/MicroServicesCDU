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
    public class Cliente
    {
        [Key]
        [StringLength(15)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Identificacion { set; get; }

        [Required]
        [StringLength(50)]
        public string Nombre { set; get; }

        [Required]
        public DateTime FechaNacimiento { set; get; }

        [StringLength(30)]
        [DataType(DataType.EmailAddress)]
        public string CorreoElectronico { set; get; }

        [StringLength(15)]
        [DataType(DataType.PhoneNumber)]
        public string Celular { set; get; }

        [StringLength(15)]
        [DataType(DataType.PhoneNumber)]
        public string TelFijo { set; get; }

        [Required]
        [StringLength(50)]
        public string Direccion { set; get; }

        [Required]
        [StringLength(20)]
        public string Barrio { set; get; }

        [Required]
        [StringLength(20)]
        public string Municipio { set; get; }

        [Required]
        [StringLength(20)]
        public string Departamento { set; get; }

        [StringLength(100)]
        public string Clave { set; get; }

        public DateTime FechaActualizacionClave { set; get; }

        [Required]
        [StringLength(2)]
        [DefaultValue(90)]
        public int DiasCaducidadClave { set; get; }

        public List<Producto> Productos { set; get; }
    }
}