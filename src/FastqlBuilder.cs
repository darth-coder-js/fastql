﻿using Fastql.Attributes;
using Fastql.Utilities;
using System;
using System.Linq;
using System.Reflection;

namespace Fastsql
{
    /// <summary>
    /// A small and fast library for building SQL queries from entity classes
    /// in a better way than regular string concatenation.
    /// </summary>
    public class FastqlBuilder<TEntity>
    {
        private readonly TEntity _entity;
        public FastqlBuilder(TEntity entity)
        {
            _entity = entity;
        }

        public string TableName()
        {
            var type = _entity.GetType();
            if (type.CustomAttributes.Count() > 0)
            {
                var attribute = type.CustomAttributes.FirstOrDefault();
                if (attribute.AttributeType.Name == "TableAttribute")
                {
                    TableAttribute table = (TableAttribute)Attribute.GetCustomAttribute(type, typeof(TableAttribute));
                    return $"[{table.Schema}].[{table.TableName}]";
                }
            }
            return "";
        }

        public string InsertQuery()
        {
            QueryBuilder qb = new QueryBuilder(TableName());
            foreach (var propertyInfo in _entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Attribute.IsDefined(propertyInfo, typeof(IsPrimaryKeyAttribute)))
                {
                    qb.AddIdentityColumn(propertyInfo.Name);
                }

                if ((!Attribute.IsDefined(propertyInfo, typeof(IsPrimaryKeyAttribute))) &&
                (!Attribute.IsDefined(propertyInfo, typeof(IsNotInsertableAttribute))))
                {
                    qb.Add(propertyInfo.Name, propertyInfo.GetValue(_entity));
                }
            }
            return qb.InsertSql;
        }

        public string UpdateQuery(TEntity entity, string where)
        {
            QueryBuilder qb = new QueryBuilder(TableName(), $" WHERE {where}");
            foreach (var propertyInfo in entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if ((!Attribute.IsDefined(propertyInfo, typeof(IsPrimaryKeyAttribute))) &&
                (!Attribute.IsDefined(propertyInfo, typeof(IsNotUpdatableAttribute))))
                {
                    qb.Add(propertyInfo.Name, propertyInfo.GetValue(entity));
                }
            }
            return qb.UpdateSql;
        }
    }
}
