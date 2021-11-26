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
        [Display(Name = "idSubcategoria")]
        [ForeignKey(nameof(idSubcategoria))]
        public int idSubcategoria { get; set; }

    }
}
