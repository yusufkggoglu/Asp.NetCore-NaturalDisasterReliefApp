using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Aid.Dtos
{
    public class BasisAidCreateDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(300, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Location { get; set; }
        [Required]
        [DataType(DataType.Url)]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string LocationUrl { get; set; }
        [DataType(DataType.Text)]
        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
        public string HelpType { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(500, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Description { get; set; }
        [Range(1, 1000)]
        [Required]
        public int Amount { get; set; }
        [DataType(DataType.Text)]
        [Required]
        public string Picture { get; set; }
        [Required]
        public bool Status { get; set; }
    }
}
