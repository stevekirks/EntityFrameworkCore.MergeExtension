CREATE SCHEMA target;
GO
CREATE TABLE [target].[SampleItems](
	[Id] integer NOT NULL,
	[Description] [nvarchar](max) NULL,
    [Timestamp] DATETIME2 NOT NULL,
    CONSTRAINT pk_dbo_SampleItems PRIMARY KEY ([Id])
);
GO

CREATE SCHEMA source;
GO
CREATE TABLE [source].[SampleItems](
	[Id] integer NOT NULL,
	[Description] [nvarchar](max) NULL,
    [Timestamp] DATETIME2 NOT NULL,
    CONSTRAINT pk_dbo_SampleItems PRIMARY KEY ([Id])
);