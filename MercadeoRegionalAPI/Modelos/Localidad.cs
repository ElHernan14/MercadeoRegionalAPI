using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI.Modelos
{
    public class Localidad
    {
        [Required]
        public int id { get; set; }
        public string poblacion { get; set; }
        public string avatar { get; set; }
        public string nombre { get; set; }
        public string provincia { get; set; }
        [NotMapped]//Para EF
        public IFormFile avatarFile { get; set; }
    }
}
