using System;
using System.Collections.Generic;
using Models;

namespace Dtos {
    public class UserManagerResponse {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public dynamic? Data { get; set; }
    }
}