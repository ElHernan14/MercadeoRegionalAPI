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
        public int id { get; set; }

        [Display(Name = "Usuario")]
        public int idUsuario { get; set; }
        [ForeignKey(nameof(idUsuario))]
        public Usuario usuario { get; set; }

        [Display(Name = "idCarrito")]
        public int idCarrito { get; set; }
        [ForeignKey(nameof(idCarrito))]
        public DetalleCarrito carrito { get; set; }
        public string pago { get; set; }
        public string envio { get; set; }
        public decimal montoTotal { get; set; }
        public int cantidad { get; set; }
    }
}
