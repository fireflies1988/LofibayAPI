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
    /// All went well, and (usually) some data was returned.
    /// </summary>
    public class SuccessResponse : BaseResponseObject
    {
        public override string Status => StatusTypes.Success;
    }
}
