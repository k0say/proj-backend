using Microsoft.AspNetCore.Mvc;
using ArticoliWebService.Services;
using System.Collections.Generic;
//using ArticoliWebService.Models;
using ArticoliWebService.Dtos;
using System.Threading.Tasks;

namespace ArticoliWebService.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/articoli")]
    public class ArticoliController : Controller
    {
        private IArticoliRepository articolirespoitory;
        // code inj nel costruttore
        public ArticoliController(IArticoliRepository articolirespoitory)
        {
            this.articolirespoitory = articolirespoitory;
        }

        // ora metodi
        [HttpGet("cerca/descrizione/{filter}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ArticoliDto>))]
        /*
            rendiamo il metodo asincrono aggiungendo la nomenclatura async dopo public
            e rendendo il tipo Task<> che avrà come generico IActionResult
            aggiungo anche await sulla riga di SelArticoliByDescrizione
            Modifico anche l'interfaccia di articolirepository dove vi è il metodo
        */
        public async Task<IActionResult> GetArticoliByDesc(string filter)
        {
            var articoliDto = new List<ArticoliDto>();
            var articoli = await this.articolirespoitory.SelArticoliByDescrizione(filter);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (articoli.Count == 0)
            {
                return NotFound(string.Format("Non è stato trovato alcun articolo con il filtro '{0}'", filter));
            }

            foreach (var articolo in articoli)
            {
                articoliDto.Add(new ArticoliDto
                {
                    CodArt = articolo.CodArt,
                    Descrizione = articolo.Descrizione,
                    Um = articolo.Um,
                    CodStat = articolo.CodStat,
                    PzCart = articolo.PzCart,
                    PesoNetto = articolo.PesoNetto,
                    DataCreazione = articolo.DataCreazione,
                    Categoria = articolo.famassort.Descrizione
                });
            }

            return Ok(articoliDto);
        }

        [HttpGet("cerca/codice/{CodArt}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]

        public async Task<IActionResult> GetArticoloByCode(string CodArt)
        {
            if (!await this.articolirespoitory.ArticoloExists(CodArt))
            {
                return NotFound(string.Format("Non è stato trovato l'articolo con il codice '{0}'", CodArt));
            }
            var articolo = await this.articolirespoitory.SelArticoloByCodice(CodArt);
            
            var barcodeDto = new List<BarcodeDto>();

            foreach (var ean in articolo.Barcode)
            {
                barcodeDto.Add(new BarcodeDto
                {
                    Barcode = ean.Barcode,
                    Tipo = ean.IdTipoArt
                });
            }

            var articoliDto = new ArticoliDto
            {
                CodArt = articolo.CodArt,
                Descrizione = articolo.Descrizione,
                Um = articolo.Um,
                CodStat = articolo.CodStat,
                PzCart = articolo.PzCart,
                PesoNetto = articolo.PesoNetto,
                DataCreazione = articolo.DataCreazione,
                Ean = barcodeDto,
                Categoria = articolo.famassort.Descrizione
            };
            return Ok(articoliDto);
        }

        [HttpGet("cerca/barcode/{Ean}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]
        public async Task<IActionResult> GetArticoloByEan(string Ean)
        {
            var articolo = await this.articolirespoitory.SelArticoloByEan(Ean);

            if (articolo == null)
            {
                return NotFound(string.Format("Non è stato trovato l'articolo con il barcode '{0}'", Ean));
            }

            var articoliDto = new ArticoliDto
            {
                CodArt = articolo.CodArt,
                Descrizione = articolo.Descrizione,
                Um = articolo.Um,
                CodStat = articolo.CodStat,
                PzCart = articolo.PzCart,
                PesoNetto = articolo.PesoNetto,
                DataCreazione = articolo.DataCreazione
            };
            return Ok(articoliDto);
        }
    }

}