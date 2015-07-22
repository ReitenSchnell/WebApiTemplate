using System;
using PetaPoco;

namespace Repository.Models
{
    [TableName("Commands")]
    public class Command : BaseEntity
    {
        public DateTime Date { get; set; }
        public string EntityType { get; set; }
        public string User { get; set; }
        public string CommandType { get; set; }
        public string CommandContent { get; set; }
    }
}