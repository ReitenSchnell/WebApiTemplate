using System;
using PetaPoco;

namespace Repository.Models
{
    [TableName("Users")]
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public Guid PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
    }
}