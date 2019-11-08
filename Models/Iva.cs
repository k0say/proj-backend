using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArticoliWebService.Models
{
    public class Iva
    {
        [Key]
        public int IdIva { get; set; }
        public string Descrizione { get; set; }
        [Required]
        public Int16 Aliquota { get; set; }

        // qui dovremmo avere un tipo ICollection giacchè uno a molti
        public virtual ICollection<Articoli> Articoli { get; set; }
    }
}