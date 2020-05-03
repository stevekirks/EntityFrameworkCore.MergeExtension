MERGE TargetTable WITH (HOLDLOCK) AS d
USING SourceTable AS s ON d.[Id]=s.[Id]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Id], [Description], [Timestamp])
    VALUES (s.[Id], s.[Description], s.[Timestamp])
WHEN MATCHED THEN
    UPDATE SET d.[Description]=s.[Description], d.[Timestamp]=s.[Timestamp]
WHEN NOT MATCHED BY SOURCE THEN
    DELETE;