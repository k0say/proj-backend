using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticoliWebService.Services
{
    public class ArticoliRepository : IArticoliRepository
    {
        AlphaShopDbContext alphaShopDbContext;

        public ArticoliRepository(AlphaShopDbContext alphaShopDbContext)
        {
            this.alphaShopDbContext = alphaShopDbContext;
        }

        public async Task<ICollection<Articoli>> SelArticoliByDescrizione(string Descrizione)
        {
            return await this.alphaShopDbContext.Articoli
                .Where(a => a.Descrizione.Contains(Descrizione))
                .Include(a => a.famassort)
                .OrderBy(a => a.Descrizione)
                .ToListAsync();
        }

        public async Task<ICollection<Articoli>> SelArticoliByDescrizione(string Descrizione, string IdCat)
        {
            bool isNumeric = int.TryParse(IdCat, out int n);

            if (string.IsNullOrWhiteSpace(IdCat) || !isNumeric)
            {
                return await this.SelArticoliByDescrizione(Descrizione);
            }

            return await this.alphaShopDbContext.Articoli
                    .Where(a => a.Descrizione.Contains(Descrizione))
                    .Where(a => a.IdFamAss == int.Parse(IdCat))
                    .Include(a => a.famassort)
                    .OrderBy(a => a.Descrizione)
                    .ToListAsync();
        }

        public async Task<Articoli> SelArticoloByCodice(string Code)
        {
            return await this.alphaShopDbContext.Articoli
                .Where(a => a.CodArt.Equals(Code))
                .Include(a => a.Barcode)
                .Include(a => a.famassort)
                .FirstOrDefaultAsync();
        }

        public Articoli SelArticoloByCodice2(string Code)
        {
            return this.alphaShopDbContext.Articoli
                .AsNoTracking()
                .Where(a => a.CodArt.Equals(Code))
                //.Include(a => a.barcode)
                //.Include(a => a.famAssort)
                .FirstOrDefault();
        }

        public async Task<Articoli> SelArticoloByEan(string Ean)
        {
            return await this.alphaShopDbContext.Barcode
                .Where(b => b.Barcode.Equals(Ean))
                .Select(a => a.articolo)
                .Include(a => a.Barcode)
                .Include(a => a.famassort)
                .FirstOrDefaultAsync();
        }

        public bool InsArticoli(Articoli articolo)
        {
            this.alphaShopDbContext.Add(articolo);
            return Salva();
        }

        public bool UpdArticoli(Articoli articolo)
        {
            this.alphaShopDbContext.Update(articolo);
            return Salva();
        }

        public bool DelArticoli(Articoli articolo)
        {
            this.alphaShopDbContext.Remove(articolo);
            return Salva();
        }

        public async Task<bool> ArticoloExists(string Code) => await
                this.alphaShopDbContext
                .Articoli
                .AnyAsync(c => c.CodArt == Code);

        public bool Salva()
        {
            var saved = this.alphaShopDbContext.SaveChanges();
            return saved >= 0 ? true : false;
        }


    }
}