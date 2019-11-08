using ArticoliWebService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticoliWebService.Services
{
    public class AlphaShopDbContext : DbContext
    {
        public AlphaShopDbContext(DbContextOptions<AlphaShopDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Articoli> Aritcoli { get; set; }
        public virtual DbSet<Ean> Barcode { get; set; }
        public virtual DbSet<FamAssort> Famassort { get; set; }
        public virtual DbSet<Ingredienti> Ingredienti { get; set; }
        public virtual DbSet<Iva> Iva { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articoli>()
                .HasKey(a => new { a.CodArt });
                
            // relazione 1 to N fra articoli e barcode
            modelBuilder.Entity<Ean>()
                .HasOne<Articoli>(s => s.articolo)          // ad un articolo
                .WithMany(g => g.Barcode)                   // corrispondono molti barcode
                .HasForeignKey(s => s.CodArt);              // la chiave esterna dell'entity barcode

            // relazione 1 to 1 fra articoli e ingredienti
            modelBuilder.Entity<Articoli>()
                .HasOne<Ingredienti>(s => s.ingredienti)    //ad un articolo
                .WithOne(g => g.articolo)                  //corrisponde un ingrediente
                .HasForeignKey<Ingredienti>(s => s.CodArt);

            // relazione 1 to N fra iva e articoli
            modelBuilder.Entity<Articoli>()
                .HasOne<Iva>(s => s.iva)
                .WithMany(g => g.Articoli)
                .HasForeignKey(s => s.IdIva);

            // relazione 1 to N fra FamAssort e Articoli
            modelBuilder.Entity<Articoli>()
                .HasOne<FamAssort>(s => s.famassort)
                .WithMany(g => g.Articoli)
                .HasForeignKey(s => s.IdFamAss);
        }

    }
}