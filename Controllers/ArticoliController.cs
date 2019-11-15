using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArticoliWebService.Dtos;
using ArticoliWebService.Models;
using ArticoliWebService.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArticoliWebService.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/articoli")]
    public class ArticoliController : Controller
    {
        private readonly IArticoliRepository articolirepository;
        private readonly IMapper mapper;
        public ArticoliController(IArticoliRepository articolirepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.articolirepository = articolirepository;
        }

        [HttpGet("cerca/descrizione/{filter}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ArticoliDto>))]
        public async Task<ActionResult<IEnumerable<ArticoliDto>>> GetArticoliByDesc(string filter, [FromQuery] string idCat)
        {
            var articoliDto = new List<ArticoliDto>();

            var articoli = await this.articolirepository.SelArticoliByDescrizione(filter, idCat);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (articoli.Count == 0)
            {
                return NotFound(string.Format("Non è stato trovato alcun articolo con il filtro '{0}'", filter));
            }
            /*
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
                        Categoria = (articolo.famassort != null) ? articolo.famassort.Descrizione : null,
                        IdStatoArt = articolo.IdStatoArt
                    });
                }
            */
            return Ok(mapper.Map<IEnumerable<ArticoliDto>>(articoli));
        }

        private ArticoliDto CreateArticoloDTO(Articoli articolo)
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

            var articoliDto = new ArticoliDto
            {
                CodArt = articolo.CodArt,
                Descrizione = articolo.Descrizione,
                Um = (articolo.Um != null) ? articolo.Um.Trim() : "",
                CodStat = (articolo.CodStat != null) ? articolo.CodStat.Trim() : "",
                PzCart = articolo.PzCart,
                PesoNetto = articolo.PesoNetto,
                DataCreazione = articolo.DataCreazione,
                Ean = barcodeDto,
                IdIva = articolo.IdIva,
                IdFamAss = articolo.IdFamAss,
                Categoria = (articolo.famassort != null) ? articolo.famassort.Descrizione : "Non definito",
                IdStatoArt = (articolo.IdStatoArt != null) ? articolo.IdStatoArt.Trim() : null
            };

            return articoliDto;
        }

        [HttpGet("cerca/codice/{CodArt}", Name = "GetArticoli")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]
        public async Task<ActionResult<IEnumerable<ArticoliDto>>> GetArticoloByCode(string CodArt)
        {
            if (!await this.articolirepository.ArticoloExists(CodArt))
            {
                return NotFound(string.Format("Non è stato trovato l'articolo con il codice '{0}'", CodArt));
            }

            var articolo = await this.articolirepository.SelArticoloByCodice(CodArt);

            return Ok(CreateArticoloDTO(articolo));
        }

        [HttpGet("cerca/barcode/{Ean}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]
        public async Task<ActionResult<IEnumerable<ArticoliDto>>> GetArticoloByEan(string Ean)
        {
            var articolo = await this.articolirepository.SelArticoloByEan(Ean);

            if (articolo == null)
            {
                return NotFound(string.Format("Non è stato trovato l'articolo con il barcode '{0}'", Ean));
            }

            return Ok(CreateArticoloDTO(articolo));

        }

        [HttpPost("inserisci")]
        [ProducesResponseType(201, Type = typeof(Articoli))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult SaveArticoli([FromBody] Articoli articolo)
        {
            if (articolo == null)
            {
                return BadRequest(ModelState);
            }

            //Verifichiamo che i dati siano corretti
            if (!ModelState.IsValid)
            {
                string ErrVal = "";

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        ErrVal += modelError.ErrorMessage + " - ";
                    }
                }
                return BadRequest(new InfoMsg(DateTime.Today, ErrVal));
            }

            //Contolliamo se l'articolo è presente
            var isPresent = articolirepository.SelArticoloByCodice2(articolo.CodArt);

            if (isPresent != null)
            {
                //ModelState.AddModelError("", $"Articolo {articolo.CodArt} presente in anagrafica! Impossibile utilizzare il metodo POST!");
                return StatusCode(422, new InfoMsg(DateTime.Today, $"Articolo {articolo.CodArt} presente in anagrafica! Impossibile utilizzare il metodo POST!"));
            }

            articolo.DataCreazione = DateTime.Today;

            //verifichiamo che i dati siano stati regolarmente inseriti nel database
            if (!articolirepository.InsArticoli(articolo))
            {
                //ModelState.AddModelError("", $"Ci sono stati problemi nell'inserimento dell'Articolo {articolo.CodArt}.  ");
                return StatusCode(500, new InfoMsg(DateTime.Today, $"Ci sono stati problemi nell'inserimento dell'Articolo {articolo.CodArt}.  "));
            }

            //return CreatedAtRoute("GetArticoli", new { codart = articolo.CodArt }, CreateArticoloDTO(articolo));
            return Ok(new InfoMsg(DateTime.Today, $"Inserimento articolo {articolo.CodArt} eseguita con successo!"));

        }

        [HttpPut("modifica")]
        [ProducesResponseType(201, Type = typeof(InfoMsg))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateArticoli([FromBody] Articoli articolo)
        {
            if (articolo == null)
            {
                return BadRequest(ModelState);
            }

            //Verifichiamo che i dati siano corretti
            if (!ModelState.IsValid)
            {
                string ErrVal = "";

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        ErrVal += modelError.ErrorMessage + " - ";
                    }
                }
                return BadRequest(new InfoMsg(DateTime.Today, ErrVal));
            }

            //Contolliamo se l'articolo è presente (Usare il metodo senza Traking)
            var isPresent = articolirepository.SelArticoloByCodice2(articolo.CodArt);

            if (isPresent == null)
            {
                //ModelState.AddModelError("", $"Articolo {articolo.CodArt} NON presente in anagrafica! Impossibile utilizzare il metodo PUT!");
                return StatusCode(422, $"Articolo {articolo.CodArt} NON presente in anagrafica! Impossibile utilizzare il metodo PUT!");
            }

            //verifichiamo che i dati siano stati regolarmente inseriti nel database
            if (!articolirepository.UpdArticoli(articolo))
            {
                //ModelState.AddModelError("", $"Ci sono stati problemi nella modifica dell'Articolo {articolo.CodArt}.  ");
                return StatusCode(500, $"Ci sono stati problemi nella modifica dell'Articolo {articolo.CodArt}.  ");
            }

            return Ok(new InfoMsg(DateTime.Today, $"Modifica articolo {articolo.CodArt} eseguita con successo!"));

        }

        [HttpDelete("elimina/{codart}")]
        [ProducesResponseType(201, Type = typeof(InfoMsg))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(InfoMsg))]
        [ProducesResponseType(422, Type = typeof(InfoMsg))]
        [ProducesResponseType(500)]
        public IActionResult DeleteArticoli(string codart)
        {
            if (codart == "")
            {
                return BadRequest(new InfoMsg(DateTime.Today, $"E' necessario inserire il codice dell'articolo da eliminare!"));
            }

            //Contolliamo se l'articolo è presente (Usare il metodo senza Traking)
            var articolo = articolirepository.SelArticoloByCodice2(codart);

            if (articolo == null)
            {
                return StatusCode(422, new InfoMsg(DateTime.Today, $"Articolo {codart} NON presente in anagrafica! Impossibile Eliminare!"));
            }

            //verifichiamo che i dati siano stati regolarmente eliminati dal database
            if (!articolirepository.DelArticoli(articolo))
            {
                //ModelState.AddModelError("", $"Ci sono stati problemi nella eliminazione dell'Articolo {articolo.CodArt}.  ");
                return StatusCode(500, $"Ci sono stati problemi nella eliminazione dell'Articolo {articolo.CodArt}.  ");
            }

            return Ok(new InfoMsg(DateTime.Today, $"Eliminazione articolo {codart} eseguita con successo!"));

        }
    }
}