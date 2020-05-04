# Entity Framework Core SQL MERGE Extension

Adds extension methods to an EntityFrameworkCore.DbContext to perform a SQL MERGE between a DbContext Entity and another table.

![CI](https://github.com/stevekirks/EntityFrameworkCore.MergeExtension/workflows/ci/badge.svg)

### Usage
```
dbContext.MergeInto<SourceEntityType>("[dbo].[TargetTable]");
```
or
```
dbContext.MergeFrom<TargetEntityType>("[dbo].[SourceTable]");
```