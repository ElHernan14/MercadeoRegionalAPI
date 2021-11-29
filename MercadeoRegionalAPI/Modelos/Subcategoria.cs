using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI.Modelos
{
    public class Subcategoria
    {
        [Required]
        public int id { get; set; }
        public string descripcion { get; set; }
        public string tipo { get; set; }
        [Display(Name = "idCategoria")]
        public int idCategoria { get; set; }
        [ForeignKey(nameof(idCategoria))]
        public Categoria categoria { get; set; }
    }
}
