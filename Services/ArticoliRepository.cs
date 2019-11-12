using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;
using Microsoft.AspNetCore.Mvc;
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
                .Include(a => a.famassort)
                // TEST
                .Include(a => a.Barcode)
                .OrderBy(a => a.Descrizione)
                .ToListAsync();
        }

        async Task<Articoli> IArticoliRepository.SelArticoloByCodice(string Code)
        {
            return await this.alphaShopDbContext.Articoli
                .Where(a => a.CodArt.Equals(Code))
                .Include(a => a.Barcode)
                .Include(a => a.famassort)
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
            this.alphaShopDbContext.Add(articolo);
            return Salva();
        }

        private bool Salva()
        {
            var saved = this.alphaShopDbContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdArticoli(Articoli articolo)  {
            this.alphaShopDbContext.Update(articolo);
            return Salva();
        }

        public bool DelArticoli(Articoli articolo)  {
            this.alphaShopDbContext.Remove(articolo);
            return Salva();
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

        //public IActionResult SaveArticoli([FromBody] Articoli articolo) {}


        bool IArticoliRepository.Salva()
        {
            throw new System.NotImplementedException();
        }

        internal static object SelArticoloByCodice(string codArt)
        {
            throw new NotImplementedException();
        }
    }
}