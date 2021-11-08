result=$(/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P '$SA_PASSWORD' -Q "SELECT name FROM sys.databases WHERE name = N'HBKStorage'")
echo $result