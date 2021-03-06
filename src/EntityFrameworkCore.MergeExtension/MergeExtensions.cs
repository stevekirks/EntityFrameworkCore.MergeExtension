﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("EntityFrameworkCore.MergeExtension.Tests")]
namespace EntityFrameworkCore.MergeExtension
{
    /// <summary>
    /// Merge Extensions
    /// </summary>
    public static class MergeExtensions
    {
        /// <summary>
        /// Merge a table from the DbContext into the target table
        /// Requires source table to be identical to target table
        /// </summary>
        public static void MergeInto<TSource>(this DbContext dbContext, string targetTable)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext), "DbContext must not be null");
            var sourceTable = dbContext.GetTableName<TSource>();
            var rawSql = dbContext.GetRawSqlMerge<TSource>(sourceTable, targetTable);
            dbContext.Database.ExecuteSqlRaw(rawSql);
        }

        /// <summary>
        /// Merge a table from the DbContext into the target table
        /// Requires source table to be identical to target table
        /// </summary>
        public static async Task MergeIntoAsync<TSource>(this DbContext dbContext, string targetTable)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext), "DbContext must not be null");
            var sourceTable = dbContext.GetTableName<TSource>();
            var rawSql = dbContext.GetRawSqlMerge<TSource>(sourceTable, targetTable);
            await dbContext.Database.ExecuteSqlRawAsync(rawSql).ConfigureAwait(false);
        }

        /// <summary>
        /// Merge a source table into a table from the DbContext
        /// Requires target table to be identical to source table
        /// </summary>
        public static void MergeFrom<TTarget>(this DbContext dbContext, string sourceTable)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext), "DbContext must not be null");
            var targetTable = dbContext.GetTableName<TTarget>();
            var rawSql = dbContext.GetRawSqlMerge<TTarget>(sourceTable, targetTable);
            dbContext.Database.ExecuteSqlRaw(rawSql);
        }

        /// <summary>
        /// Merge a source table into a table from the DbContext
        /// Requires target table to be identical to source table
        /// </summary>
        public static async Task MergeFromAsync<TTarget>(this DbContext dbContext, string sourceTable)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext), "DbContext must not be null");
            var targetTable = dbContext.GetTableName<TTarget>();
            var rawSql = dbContext.GetRawSqlMerge<TTarget>(sourceTable, targetTable);
            await dbContext.Database.ExecuteSqlRawAsync(rawSql).ConfigureAwait(false);
        }

        /// <summary>
        /// Truncate a table
        /// </summary>
        public static void Truncate<T>(this DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext), "DbContext must not be null");
            dbContext.Database.ExecuteSqlRaw(dbContext.GetTruncateSqlRaw<T>());
        }

        /// <summary>
        /// Truncate a table
        /// </summary>
        public static async Task TruncateAsync<T>(this DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext), "DbContext must not be null");
            await dbContext.Database.ExecuteSqlRawAsync(dbContext.GetTruncateSqlRaw<T>()).ConfigureAwait(false);
        }

        internal static string GetRawSqlMerge<T>(this DbContext dbContext, string sourceTable, string targetTable)
        {
            var type = typeof(T);
            var entityType = dbContext.Model.FindEntityType(type);
            if (entityType == null)
                throw new InvalidOperationException($"DbContext does not contain Entity Type {type.Name}");

            var primaryKeys = entityType.FindPrimaryKey()?.Properties.Select(a => a.Name).ToList();
            if (primaryKeys == null)
                throw new InvalidOperationException($"Entity Type {type.Name} must have a key");

            var properties = entityType.GetProperties().Select(a => a.Name);
            var propertiesExcludingPrimaryKeys = properties.Where(p => !primaryKeys.Contains(p));

            var bob = new StringBuilder();
            bob.AppendLine($"MERGE {targetTable} WITH (HOLDLOCK) AS d");
            bob.AppendLine($"USING {sourceTable} AS s ON {primaryKeys.GetEqualStatements("d", "s").JoinWithAnds()}");
            bob.AppendLine($"WHEN NOT MATCHED BY TARGET THEN");
            bob.AppendLine($"    INSERT ({properties.GetFormatedColumns().JoinWithCommas()})");
            bob.AppendLine($"    VALUES ({properties.GetFormatedColumns("s").JoinWithCommas()})");
            bob.AppendLine($"WHEN MATCHED THEN");
            bob.AppendLine($"    UPDATE SET {propertiesExcludingPrimaryKeys.GetEqualStatements("d", "s").JoinWithCommas()}");
            bob.AppendLine($"WHEN NOT MATCHED BY SOURCE THEN");
            bob.Append($"    DELETE;");
            var rawSql = bob.ToString();

            return rawSql;
        }

        private static string GetTruncateSqlRaw<T>(this DbContext dbContext)
        {
            var tableName = dbContext.GetTableName<T>();
            return $"TRUNCATE TABLE {tableName};";
        }

        private static string GetTableName<T>(this DbContext dbContext)
        {
            var type = typeof(T);
            var entityType = dbContext.Model.FindEntityType(type);
            if (entityType == null)
                throw new InvalidOperationException($"DbContext does not contain Entity Type {type.Name}");

            var schema = entityType.GetSchema() ?? "dbo";
            var tableName = entityType.GetTableName();

            var formattedSchema = schema != null ? $"[{schema}]." : "";
            var formatedTable = $"{formattedSchema}[{tableName}]";

            return formatedTable;
        }

        private static IEnumerable<string> GetEqualStatements(this IEnumerable<string> columnNames, string sourcePrefix, string destPrefix)
        {
            var columnNameEqualsList = columnNames.Select(c => $"{sourcePrefix}.[{c}]={destPrefix}.[{c}]");
            return columnNameEqualsList;
        }

        private static IEnumerable<string> GetFormatedColumns(this IEnumerable<string> columnNames, string prefix = null)
        {
            var formattedColumnNames = columnNames.Select(c => {
                var prefixOrEmpty = prefix == null ? "" : $"{prefix}.";
                return $"{prefixOrEmpty}[{c}]";
            });
            return formattedColumnNames;
        }

        private static string JoinWithAnds(this IEnumerable<string> vals)
        {
            var result = string.Join(" AND ", vals);
            return result;
        }

        private static string JoinWithCommas(this IEnumerable<string> vals)
        {
            var result = string.Join(", ", vals);
            return result;
        }
    }
}
