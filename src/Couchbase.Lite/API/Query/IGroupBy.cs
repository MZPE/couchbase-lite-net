﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Couchbase.Lite.Query
{
    public interface IGroupBy : IQuery, IHavingRouter, IOrderByRouter, ILimitRouter
    {
    }
}