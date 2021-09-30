﻿using System;

namespace Fastql.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class IsPrimaryKeyAttribute : Attribute
    {
    }
}
