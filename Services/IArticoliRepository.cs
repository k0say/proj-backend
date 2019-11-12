using System.Collections.Generic;
using System.Threading.Tasks;
using ArticoliWebService.Models;

namespace ArticoliWebService.Services
{
    public interface IArticoliRepository
    {
        /*
            rendo di tipo Task anche la collection
        */
        Task<ICollection<Articoli>> SelArticoliByDescrizione(string Descrizione);
        Task<Articoli> SelArticoloByCodice(string Code);
        Task<Articoli> SelArticoloByEan(string Ean);
        bool InsArticoli(Articoli articolo);
        bool UpdArticoli(Articoli articolo);
        bool DelArticoli(Articoli articolo);
        bool Salva();

        Task<bool> ArticoloExists(string Code);
    }
}

