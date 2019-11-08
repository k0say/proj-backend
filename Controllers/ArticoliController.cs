using Microsoft.AspNetCore.Mvc;
using ArticoliWebService.Services;
using System.Collections.Generic;
using ArticoliWebService.Models;

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
        [ProducesResponseType(200, Type = typeof(IEnumerable<Articoli>))]
        public IActionResult GetArticoliByDesc(string filter)
        {
            var articoli = this.articolirespoitory.SelArticoliByDescrizione(filter);
            return Ok(articoli);
        }

    }
}
