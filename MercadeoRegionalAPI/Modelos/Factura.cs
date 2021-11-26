using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI.Modelos
{
    public class Factura
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "idUsuario")]
        [ForeignKey(nameof(idUsuario))]
        public int idUsuario { get; set; }

        [Display(Name = "idCarrito")]
        [ForeignKey(nameof(idCarrito))]
        public int idCarrito { get; set; }
        public int estado { get; set; }
        public string pago { get; set; }
        public string envio { get; set; }
        public decimal montoTotal { get; set; }
    }
}
