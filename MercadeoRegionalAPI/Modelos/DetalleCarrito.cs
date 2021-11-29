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
        public int id { get; set; }

        [Display(Name = "idArticulo")]
        public int idArticulo { get; set; }
        [ForeignKey(nameof(idArticulo))]
        public Articulo articulo { get; set; }

        [Display(Name = "Usuario")]
        public int idUsuario { get; set; }
        [ForeignKey(nameof(idUsuario))]
        public Usuario usuario { get; set; }
        public int estado { get; set; }
        public int cantidad { get; set; }
    }
}
