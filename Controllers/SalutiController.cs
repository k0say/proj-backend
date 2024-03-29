//namespace ArticoliWebService.Controllers
using System;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api/saluti")]
    public class SalutiController
    {
        [HttpGet]
        public string getSaluti()
        {
            return "Saluti, sono il primo web service creato in C#";
        }

        [HttpGet("{Nome}")]
        public string getSaluti2(string Nome)
        {
            try
            {
                if (Nome == "Marco")
                    throw new Exception("\"Errore: L'utente Marco è disabilitato!\"");
                else
                    return string.Format("\"Saluti, {0} sono il tuo primo web service creato con c#\"", Nome);
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
/*
    using Microsoft.AspNetCore.Mvc;
    namespace ArticoliWebService.Controllers
    {
        [ApiController]
        [Route("api/saluti")]
        public class SalutiController : ControllerBase
        {
            [HttpGet]
            public IActionResult GetSaluti()
            {
                return Ok("Saluti, sono il tuo primo web service creato in .Net Core 3");
            }

            [HttpGet("{nome}")]
            public IActionResult GetSalutiNome(string nome)
            {
                return Ok(string.Format("Saluti {0}, sono il tuo primo web service creato in .Net Core 3", nome));
            }

        }
    }
}*/