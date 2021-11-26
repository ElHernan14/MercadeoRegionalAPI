using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI.Modelos
{
    public class Login
    {
        [Required]
        public int id { get; set; }
        public string nick { get; set; }
        public string password { get; set; }
    }
}
