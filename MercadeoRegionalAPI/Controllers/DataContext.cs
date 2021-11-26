using MercadeoRegionalAPI.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MercadeoRegionalAPI.Controllers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<DetalleCarrito> Carrito { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Localidad> Localidades { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<Subcategoria> Subcategorias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
