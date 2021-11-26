using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI.Modelos
{
    public class DetalleCarrito
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "idUsuario")]
        [ForeignKey(nameof(idUsuario))]
        public int idUsuario { get; set; }

        [Display(Name = "idArticulo")]
        [ForeignKey(nameof(idArticulo))]
        public int idArticulo { get; set; }
        public int estado { get; set; }
        public int cantidad { get; set; }
    }
}
