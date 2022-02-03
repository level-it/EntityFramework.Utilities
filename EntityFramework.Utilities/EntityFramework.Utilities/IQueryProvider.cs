// Copyright © 2009-2021 Level IT
// All rights reserved as Copyright owner.
//
// You may not use this file unless explicitly stated by Level IT.

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;

namespace EntityFramework.Utilities
{
	public interface IQueryProvider
	{
		bool CanDelete { get; }
		bool CanUpdate { get; }
		bool CanInsert { get; }
		bool CanBulkUpdate { get; }

		string GetDeleteQuery(QueryInformation queryInformation);
		string GetUpdateQuery(QueryInformation predicateQueryInfo, QueryInformation modificationQueryInfo);
		void InsertItems<T>(IEnumerable<T> items, string schema, string tableName, IList<ColumnMapping> properties, DbConnection storeConnection, int? batchSize, int? executeTimeout, SqlBulkCopyOptions copyOptions, DbTransaction transaction);
		IEnumerable<T> InsertItemsIds<T>(IEnumerable<T> items, string schema, string tableName, IList<ColumnMapping> properties, DbConnection connectionToUse, int? batchSize, int? executeTimeout, SqlBulkCopyOptions copyOptions, DbTransaction transaction, DbContext dbContext);

		void UpdateItems<T>(IEnumerable<T> items, string schema, string tableName, IList<ColumnMapping> properties, DbConnection storeConnection, int? batchSize, UpdateSpecification<T> updateSpecification, int? executeTimeout, SqlBulkCopyOptions copyOptions, DbTransaction transaction, DbConnection insertConnection);

		bool CanHandle(DbConnection storeConnection);

		QueryInformation GetQueryInformation<T>(System.Data.Entity.Core.Objects.ObjectQuery<T> query);

	}
}
