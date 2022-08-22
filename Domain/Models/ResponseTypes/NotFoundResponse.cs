using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ResponseTypes
{
    public class NotFoundResponse<T> : BaseResponse<T>
    {
        public override string? Status => StatusTypes.NotFound;
    }

    public class NotFoundResponse : BaseResponse<object>
    {
        public override string? Status => StatusTypes.NotFound;
    }
}
