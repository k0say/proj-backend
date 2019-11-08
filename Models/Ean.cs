using System.ComponentModel.DataAnnotations;

namespace ArticoliWebService.Models
{
    public class Ean
    {
        public string CodArt { get; set; }
        [Key]
        [StringLength(13, MinimumLength=8, ErrorMessage="Il barcode deve avere da 8 a 13 cifre")]
        public string Barcode { get; set; }
        [Required]
        public string IdTipoArt { get; set; }
        
        //a tanti barcode un solo articolo
        public virtual Articoli articolo { get; set; }
    }
}