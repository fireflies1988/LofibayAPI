using Common.Enums;
using Common.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseTypes
{
    /// <summary>
    /// An error occurred in processing the request, i.e. an exception was thrown.
    /// </summary>
    public class ErrorResponse : BaseResponseObject
    {
        public override string? Status => StatusTypes.Error;
    }
}
