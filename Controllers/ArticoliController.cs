using Microsoft.AspNetCore.Mvc;
using ArticoliWebService.Services;
using System.Collections.Generic;
using ArticoliWebService.Models;
using ArticoliWebService.Dtos;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;

namespace ArticoliWebService.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/articoli")]
    public class ArticoliController : Controller
    {
        // apporre READONLY quando si dichiara la D-INJ
        private readonly IArticoliRepository articolirepository;
        // code inj nel costruttore
        public ArticoliController(IArticoliRepository articolirepository)
        {
            this.articolirepository = articolirepository;
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
            var articoli = await this.articolirepository.SelArticoliByDescrizione(filter);
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
                var barcodeDto = new List<BarcodeDto>();

                foreach (var ean in articolo.Barcode)
                {
                    barcodeDto.Add(new BarcodeDto
                    {
                        Barcode = ean.Barcode,
                        Tipo = ean.IdTipoArt
                    });
                }

                articoliDto.Add(new ArticoliDto
                {
                    CodArt = articolo.CodArt,
                    Descrizione = articolo.Descrizione,
                    Um = articolo.Um,
                    CodStat = articolo.CodStat,
                    PzCart = articolo.PzCart,
                    PesoNetto = articolo.PesoNetto,
                    DataCreazione = articolo.DataCreazione,
                    Categoria = articolo.famassort.Descrizione,
                    Ean = barcodeDto,
                    IdStatoArt = articolo.IdStatoArt
                });
            }

            return Ok(articoliDto);
        }

        [HttpGet("cerca/codice/{CodArt}", Name = "GetArticoli")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]

        public async Task<IActionResult> GetArticoloByCode(string CodArt)
        {
            if (!await this.articolirepository.ArticoloExists(CodArt))
            {
                return NotFound(string.Format("Non è stato trovato l'articolo con il codice '{0}'", CodArt));
            }
            var articolo = await this.articolirepository.SelArticoloByCodice(CodArt);

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
                Categoria = articolo.famassort.Descrizione,
                IdStatoArt = articolo.IdStatoArt
            };
            return Ok(articoliDto);
        }

        [HttpGet("cerca/barcode/{Ean}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]
        public async Task<IActionResult> GetArticoloByEan(string Ean)
        {
            var articolo = await this.articolirepository.SelArticoloByEan(Ean);

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
                DataCreazione = articolo.DataCreazione,
                IdStatoArt = articolo.IdStatoArt
            };
            return Ok(articoliDto);
        }


        [HttpPost("inserisci")]
        [ProducesResponseType(201, Type = typeof(Articoli))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult SaveArticoli([FromBody] Articoli articolo)
        {
            if (articolo == null)
            {
                return BadRequest(ModelState);
            }

            //verifico esistenza articolo
            var isPresent = ArticoliRepository.SelArticoloByCodice(articolo.CodArt);

            if (isPresent != null)
            {
                ModelState.AddModelError("", $"Articolo {articolo.CodArt} presente in anagrafica! Impossibile utilizzare il metodo POST!");
                return StatusCode(422, ModelState);
            }

            // verifichiamo che i dati siano corretti
            if (!ModelState.IsValid)
            {
                string ErrVal = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        ErrVal += modelError.ErrorMessage + "|";
                    }

                }
                return BadRequest(ErrVal);
            }

            // verifichiamo che i dati siano stati regolarmente inseriti nel database
            //if (!ArticoliRepository.InsArticoli(articolo))
            if (!articolirepository.InsArticoli(articolo))
            {
                ModelState.AddModelError("", $"Ci sono stati problemi nell'inserimento dell'Articolo {articolo.CodArt}.  ");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetArticoli", new { codart = articolo.CodArt }, articolo);
        }


    }
}