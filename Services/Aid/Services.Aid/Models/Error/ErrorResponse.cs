using System.Collections.Generic;

namespace Services.Aid.Models.Error
{
    public class ErrorResponse
    {
        public List<ErrorModel> Error { get; set; } = new List<ErrorModel>();
        public bool Successful { get; set; }
    }
}
