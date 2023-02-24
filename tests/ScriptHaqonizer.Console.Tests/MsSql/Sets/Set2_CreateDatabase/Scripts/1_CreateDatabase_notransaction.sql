CREATE DATABASE NICEDB;

GO


CREATE TABLE NICEDB.dbo.[__MigrationScripts] (
    [Id] int NOT NULL,
    [ScriptName] varchar(50) NOT NULL,
    [AppliedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_MigrationScripts] PRIMARY KEY ([Id])
);

GO