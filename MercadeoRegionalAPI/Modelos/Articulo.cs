﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI.Modelos
{
    public class Articulo
    {
        [Required]
        public int id { get; set; }
        public string marca { get; set; }
        public decimal precio { get; set; }

        public decimal iva { get; set; }
        public int cantidadStock { get; set; }
        public string avatar { get; set; }
        [NotMapped]//Para EF
        public IFormFile avatarFile { get; set; }
        public int estado { get; set; }
        [Display(Name = "Usuario")]
        [ForeignKey(nameof(idUsuario))]
        public int idUsuario { get; set; }

        [Display(Name = "Subcategoria")]
        [ForeignKey(nameof(idSubcategoria))]
        public int idSubcategoria { get; set; }
        

    }
}
