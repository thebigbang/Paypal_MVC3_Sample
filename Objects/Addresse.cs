using System.ComponentModel.DataAnnotations;

namespace Objects
{
    public class Addresse
    {
        [Required(ErrorMessage = "You must enter at least a main street.")]
        public string MainStreet { get; set; }
        public string SubStreet { get; set; }
        [Required(ErrorMessage = "The City is required.")]
        public string City { get; set; }
        [Required(ErrorMessage = "The Post Code is Required.")]
        public string PostCode { get; set; }
        public Identity Identity { get; set; }
    }
}