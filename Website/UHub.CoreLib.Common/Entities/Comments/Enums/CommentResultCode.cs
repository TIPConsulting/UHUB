﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Entities.Comments
{
    public enum CommentResultCode
    {
        Success = 0,
        UnknownError = 1000,
        NullArgument = 1001,
        InvalidArgument = 1002,
        InvalidArgumentType = 1003,
        InvalidOperation = 1100,
        AccessDenied = 1200,


        ContentInvalid = 1010,
        ContentEmpty = 1011,

    }
}
