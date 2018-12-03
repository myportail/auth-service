﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace authService.Model.Db
{
    public class User
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Password { get; set; }
        [MaxLength(50)]
        public string Role { get; set; }
    }
}
