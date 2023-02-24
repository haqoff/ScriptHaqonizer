# ScriptHaqonizer
It is a console utility with which you can automate the cycle of verification and deployment of migration scripts.
Supports database backup with automatic calculation of affected databases, syntax validation and script execution check. Thus, the utility can be implemented at the CI / CD stage to check the supplied scripts, as well as execute them on the desired environment.

### Concept of work:
1. Scripts are added to the desired folder with a specific name. The name includes various parameters, for example: environments in which the script should be executed.
2. At the CI/CD stage (for example, during PR or Merge in DEV/MASTER), a utility is launched that calculates new scripts to be executed in this environment.
3. If it is a PR, validate the script syntax, and also try to run the script, and then rollback the transaction.
4. If it's Merge, back up the affected databases and then run the new scripts.

### Rules for naming scripts.
Template: *ID_MigrationName[_Environments][_Params]*

Where:

**ID - unique identifier of the script.**
<br/>
Starts at 1. Should increase with each script. The magnification difference is irrelevant.
<br/>
It is in the order of increasing identifier that scripts are executed.

**MigrationName - migration name in ASCII. From 3 to 50 characters.**

**Environments - script environment parameters.**
<br/>
Supported environments: *Local, Development, Integration, Staging, Loading, Prelive, Production*.
<br/>
An exclamation point at the beginning means the specified environments are excluded. For example, *!Local&Development* means that the script will run everywhere except for the Local and Development environments.
If the environment parameters are not specified, the script will be executed everywhere.

**Params - additional execution parameters.**
* NoCheck - a sign that in the check mode it is not necessary to execute the script with the subsequent rollback of the transaction.
  <br/>
  By default, if the console is started in OnlyCheck mode, all new scripts will be attempted to execute them, and after execution, the transaction will be rolled back.
  Thus, the actual possibility of executing scripts is checked.
  The parameter disables such a check and can be useful if the script is heavyweight and generates a large transaction log.
  In this case, the validation of the script syntax will be performed as before.
* NoTransaction - a sign that the script execution process does not need to be wrapped in a transaction.
  <br/>
  By default, each script is executed in a transaction.
  May be useful for statements that do not support transactions. For example, in MSSQL it is CREATE DATABASE.
  This option also disables the check described in the NoCheck option, since it occurs with a transaction.
* NoBackup - a sign that this script does not need to backup the database before using it.
  <br/>
  By default, if backup is enabled, before running all scripts, all existing databases that were affected by new scripts are calculated, and then they are backed up.
  This option excludes the specified script from the backup process.

Parameters are concatenated with &

Title examples:
* 1_AddNewColumn.sql
* 1_AddNewSqlJob_!local.sql
* 1_AddDatabase_NoTransaction.sql
* 3_AddNewRole_Development&Production_Nobackup
* 4_AddDataFromExternalDevDb_Development_NoCheck&NoBackup

### Console launch options.

| Short name | Long name                          | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           | Required |
|------------|------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------|
| -c         | --connectionString                 | Database connection string. The user must have all the rights necessary to modify database objects.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   | Yes      |
| -d         | --databaseName                     | The name of the database that stores the history of executed scripts. Moreover, the database (as well as the history table) does not have to be created at the time the console is launched. It can be created from scripts.                                                                                                                                                                                                                                                                                                                                                                                          | Yes      |
| -s         | --scriptDirectoryPath              | Full path to the folder containing the migration scripts. The folder must exist.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      | Yes      |
| -e         | --environment                      | The name of the current environment. Must be one of the values specified in the available environments section.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | Yes      |
| -m         | --mode                             | Script run mode. OnlyCheck is used to check syntax and check if scripts can run. Full is used to actually execute scripts.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            | Yes      |
| -t         | --dbType                           | Database type. Default: MSSQL                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         | No       |
|            | --specifyingDatabaseNameValidation | true/false. The default is false. If set to true, all expressions in the script must explicitly specify which database to use. For example, for MSSQL a script like: *SELECT * FROM MyTable* would be an error. *USE [MyDb]; GO SELECT * FROM MyTable* is correct, as is *SELECT * FROM MyDb.dbo.MyTable*. By default, the database specified in the databaseName parameter is used as the default database, if it exists. This parameter can be useful when migration scripts are used for different databases and it is necessary to additionally validate for which databases this or that expression is executed. | No       |
|            | --backupPath                       | Path to the folder where full database backups will be stored. If the path is specified and the startup mode is Full, the affected databases will be backed up if new migration scripts are available. Note that this is the path to a folder on the server that hosts the database. Folders with the name of the backup time will be created in this folder, and backups of the affected databases will be located inside the created folders.                                                                                                                                                                       | No       |
|            | --from                             | Launch source name. For example, PR or any other string. Present in each log and serves as an additional parameter for logging. Default: unknown.                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | No       |

### First migration scripts for MSSQL

In MSSQL, the executed scripts are stored in the *__MigrationScripts* table, so the declaration of this table must be included in the first script.
```sql
CREATE TABLE [__MigrationScripts] (
[Id] int NOT NULL,
[ScriptName] varchar(50) NOT NULL,
[AppliedAtUtc] datetime2 NOT NULL,
CONSTRAINT [PK_MigrationScripts] PRIMARY KEY ([Id])
);
```

You can also create a database before creating this table (don't forget to put the NoTransaction parameter in the file name, since CREATE DATABASE cannot be executed using transactions):
```sql
CREATE DATABASE NICEDB;
GO

CREATE TABLE NICEDB.dbo.[__MigrationScripts] (
     [Id] int NOT NULL,
     [ScriptName] varchar(50) NOT NULL,
     [AppliedAtUtc] datetime2 NOT NULL,
     CONSTRAINT [PK_MigrationScripts] PRIMARY KEY ([Id])
);
GO
```