using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class Identity
    {
        [Required(ErrorMessage = "Lastname is Required")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Firstname is Required")]
        public string Prenom { get; set; }
        [Required(ErrorMessage = "Gender is Required")]
        public string Genre { get; set; }
        public string Societe { get; set; }
    }
}