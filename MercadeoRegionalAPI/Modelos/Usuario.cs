using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI.Modelos
{
    public enum enRoles
    {
        Administrador = 1,
        Cliente = 2
    }
    public class Usuario
    {
        [Required]
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
        public int direccion { get; set; }
        public string email { get; set; }
        public int codigoPostal { get; set; }
        public string telefono { get; set; }
        public bool estado { get; set; }
        public int rol { get; set; }
        [NotMapped]//Para EF
        public string rolNombre => rol > 0 ? ((enRoles)rol).ToString() : "";
        public decimal saldo { get; set; }
        public string password { get; set; }
        public string avatar { get; set; }
        [NotMapped]//Para EF
        public IFormFile avatarFile { get; set; }

        public static string GetValuesEnum(int clave)
        {
            string valor = null;
            foreach (var value in Enum.GetValues(typeof(enRoles)))
            {
                if ((int)value == clave)
                {
                    valor = ((enRoles)value).ToString();
                }
            }
            return valor;
        }
    }
}
