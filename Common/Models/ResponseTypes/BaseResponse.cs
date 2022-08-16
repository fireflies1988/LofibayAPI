using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Models.ResponseTypes
{
    public class BaseResponse<T>
    {
        public virtual string? Status { get; set; }
        public string? Message { get; set; }
        /// <summary>
        /// Results object for your API call.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
    }
}
