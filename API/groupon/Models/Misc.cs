using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace groupon.Models
{
    public class Result
    {
        public bool Success { get; set; }
        public string Error { get; set; }

        public Result()
        {
            Success = true;
        }

        public Result(string error)
        {
            Success = false;
            Error = error;
        }
    }
}
