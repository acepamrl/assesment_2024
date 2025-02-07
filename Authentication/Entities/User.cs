﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Entities
{
    [Table("msUser")]
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
