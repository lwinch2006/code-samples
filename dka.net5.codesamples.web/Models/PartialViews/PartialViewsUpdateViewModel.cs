using System.ComponentModel.DataAnnotations;

namespace dka.net5.codesamples.web.Models.PartialViews
{
    public class PartialViewsUpdateViewModel
    {
        [Required]
        public Partial1UpdateViewModel FirstName { get; set; }
        
        [Required]
        public Partial2UpdateViewModel LastName { get; set; }
    }
}