BEGIN TRANSACTION;
GO

CREATE TABLE [__MigrationScripts] (
    [Id] int NOT NULL,
    [ScriptName] varchar(50) NOT NULL,
    [AppliedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_MigrationScripts] PRIMARY KEY ([Id])
);

GO

CREATE TABLE Persons (
    PersonID int,
    LastName varchar(255),
    FirstName varchar(255),
    Address varchar(255),
    City varchar(255)
);
GO

COMMIT TRANSACTION;
GO