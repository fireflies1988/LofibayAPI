using Domain.Enums;
using Domain.Models.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ResponseTypes
{
    /// <summary>
    /// There was a problem with the data submitted, or some pre-condition of the API call wasn't satisfied.
    /// </summary>
    public class FailResponse<T> : BaseResponse<T>
    {
        public override string? Status => StatusTypes.Fail;
    }
}
