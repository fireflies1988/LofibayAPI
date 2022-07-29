using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Responses
{
    public class BaseResponseObject
    {
        public virtual string? Status { get; set; }
        public string? Message { get; set; }
        /// <summary>
        /// Results object for your API call.
        /// </summary>
        public object? Data { get; set; }
    }
}
