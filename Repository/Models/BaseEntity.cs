using System;
using PetaPoco;

namespace Repository.Models
{
    [PrimaryKey("Id", autoIncrement = false)]
    public class BaseEntity
    {
        public Guid Id { get; set; }
    }
}