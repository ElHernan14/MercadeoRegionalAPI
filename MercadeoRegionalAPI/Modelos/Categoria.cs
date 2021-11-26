using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI.Modelos
{
    public class Categoria
    {
        [Required]
        public int id { get; set; }
        public string nombre { get; set; }
    }
}
