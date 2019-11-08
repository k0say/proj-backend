using System.Collections.Generic;
using System.Linq;
using ArticoliWebService.Models;

namespace ArticoliWebService.Services
{
    public class ArticoliRepository : IArticoliRepository
    {
        AlphaShopDbContext alphaShopDbContext;
        public ArticoliRepository(AlphaShopDbContext alphaShopDbContext)
        {
            this.alphaShopDbContext = alphaShopDbContext;
        }
        ICollection<Articoli> IArticoliRepository.SelArticoliByDescrizione(string Descrizione)
        {
            return this.alphaShopDbContext.Articoli
                .Where(a => a.Descrizione.Contains(Descrizione))
                .OrderBy(a => a.Descrizione)
                .ToList();
        }

        Articoli IArticoliRepository.SelArticoloByCodice(string Code)
        {
            return this.alphaShopDbContext.Articoli
                .Where(a => a.CodArt.Equals(Code))
                .FirstOrDefault();
        }

        Articoli IArticoliRepository.SelArticoloByEan(string Ean)
        {
            return this.alphaShopDbContext.Barcode
                .Where(b => b.Barcode.Equals(Ean))
                .Select(a => a.articolo)
                .FirstOrDefault();
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
        bool IArticoliRepository.ArticoloExists(string Code)
        {
            throw new System.NotImplementedException();
        }



        bool IArticoliRepository.Salva()
        {
            throw new System.NotImplementedException();
        }




    }
}