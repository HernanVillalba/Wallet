sqlcmd -S .\SQLEXPRESS -E -Q "if db_id(N'WALLET') is not null begin alter database [WALLET] set single_user with rollback immediate drop database [WALLET] end"
sqlcmd -S .\SQLEXPRESS -E -i .\Scripts\Script_DB_Creation.sql
sqlcmd -S .\SQLEXPRESS -E -i .\Scripts\Script_Stored_Procedures.sql
sqlcmd -S .\SQLEXPRESS -E -i .\Scripts\Script_Data.sql
sqlcmd -S .\SQLEXPRESS -E -i .\Scripts\Script_Email_Templates.sql