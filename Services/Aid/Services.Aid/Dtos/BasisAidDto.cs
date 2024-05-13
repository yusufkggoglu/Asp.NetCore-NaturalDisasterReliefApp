using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace Services.Aid.Dtos
{
    public  class BasisAidDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Neighborhood { get; set; }
        public string Address { get; set; }
        public string LocationUrl { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
