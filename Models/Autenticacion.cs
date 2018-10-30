using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MicroServicesCDU.Models
{
    [DataContract]
    public class Autenticacion
    {
        /*[DataMember]
        [Required]*/
        public long Identificacion { set; get; }

        /*[DataMember]
        [Required]*/
        public string ClaveO4UltDigProd { set; get; }
    }
}
