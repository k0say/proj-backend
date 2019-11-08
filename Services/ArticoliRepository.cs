using System.Collections.Generic;
using ArticoliWebService.Models;

namespace ArticoliWebService.Services
{
    public class ArticoliRepository : IArticoliRepository
    {
        AlphaShopDbContext alphaShopDbContext;
        public ArticoliRepository(AlphaShopDbContext alphaShopDbContext)
        {
            this.alphaShopDbContext = alphaShopDbContext;
        }/*
        ICollection<Articoli> IArticoliRepository.SelArticoliByDescrizione(string Descrizione)
        {
            return this.alphaShopDbContext.Aritcoli
                .
        }*/

        Articoli IArticoliRepository.SelArticoloByCodice(string Code)
        {
            throw new System.NotImplementedException();
        }

        Articoli IArticoliRepository.SelArticoloByEan(string Ean)
        {
            throw new System.NotImplementedException();
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