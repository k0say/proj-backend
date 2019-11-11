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
        // aggiungo async anche qui, e cambio ToList() in ToListAsync e l'await
        async Task<ICollection<Articoli>> IArticoliRepository.SelArticoliByDescrizione(string Descrizione)
        {
            return await this.alphaShopDbContext.Articoli
                .Where(a => a.Descrizione.Contains(Descrizione))
                .OrderBy(a => a.Descrizione)
                .ToListAsync();
        }

        async Task<Articoli> IArticoliRepository.SelArticoloByCodice(string Code)
        {
            return await this.alphaShopDbContext.Articoli
                .Where(a => a.CodArt.Equals(Code))
                .Include(a=>a.Barcode)
                .FirstOrDefaultAsync();
        }

        async Task<Articoli> IArticoliRepository.SelArticoloByEan(string Ean)
        {
            return await this.alphaShopDbContext.Barcode
                .Where(b => b.Barcode.Equals(Ean))
                .Select(a => a.articolo)
                .FirstOrDefaultAsync();
        }
        bool IArticoliRepository.InsArticoli(Articoli articolo)
        {
            throw new System.NotImplementedException();
        }
        bool IArticoliRepository.UpdArticoli(Articoli articolo)
        {
            throw new System.NotImplementedException();
        }
        bool IArticoliRepository.DelArticoli(Articoli articolo)
        {
            throw new System.NotImplementedException();
        }
        async Task<bool> IArticoliRepository.ArticoloExists(string Code)
        {
            return await this.alphaShopDbContext.Articoli
                .AnyAsync(c => c.CodArt == Code);
        }



        bool IArticoliRepository.Salva()
        {
            throw new System.NotImplementedException();
        }




    }
}