using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Aid.Dtos
{
    public class BasisAidCreateDto
    {
        public string UserId { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "{0} uzunluğu  {2} ve {1} arasında olmalıdır.", MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "{0} uzunluğu  {2} ve {1} arasında olmalıdır.", MinimumLength = 3)]
        public string Province { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "{0} uzunluğu  {2} ve {1} arasında olmalıdır.", MinimumLength = 3)]
        public string District { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "{0} uzunluğu  {2} ve {1} arasında olmalıdır.", MinimumLength = 3)]
        public string Neighborhood { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(300, ErrorMessage = "{0} uzunluğu  {2} ve {1} arasında olmalıdır.", MinimumLength = 3)]
        public string Address { get; set; }
        public string LocationUrl { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, ErrorMessage = "{0} uzunluğu  {2} ve {1} arasında olmalıdır.", MinimumLength = 3)]
        public string SubTitle { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(600, ErrorMessage = "{0} uzunluğu  {2} ve {1} arasında olmalıdır.", MinimumLength = 3)]
        public string Description { get; set; }
        [Range(1, 1000)]
        [Required]
        public int Amount { get; set; }
        public bool Status { get; set; }
    }
}



