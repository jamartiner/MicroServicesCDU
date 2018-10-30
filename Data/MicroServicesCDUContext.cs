using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MicroServicesCDU.Models;

namespace MicroServicesCDU.Data
{
    public class MicroServicesCDUContext : DbContext
    {

        public DbSet<MicroServicesCDU.Models.Cliente> Cliente { get; set; }
        public DbSet<MicroServicesCDU.Models.Producto> Producto { get; set; }
        public DbSet<MicroServicesCDU.Models.Movimiento> Movimiento { get; set; }

        public MicroServicesCDUContext (DbContextOptions<MicroServicesCDUContext> options)
            : base(options)
        {
        }       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>()
                .HasOne(s => s.Cliente)
                .WithMany(c => c.Productos)
                .HasForeignKey(s => s.idCliente)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Movimiento>()
                .HasOne(s => s.Producto)
                .WithMany(c => c.Movimientos)
                .HasForeignKey(s => s.idProducto)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
