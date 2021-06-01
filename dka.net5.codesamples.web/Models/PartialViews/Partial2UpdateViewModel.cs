using System;
using System.ComponentModel.DataAnnotations;

namespace dka.net5.codesamples.web.Models.PartialViews
{
    public class Partial2UpdateViewModel
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public string LastName { get; set; }
    }
}