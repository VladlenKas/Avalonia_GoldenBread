using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Domain.Responses
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; } 
        public T Data { get; set; } = default!;
        public string Message { get; set; } = null!;
    }
}
