namespace Demo.Areas.FirstStart.Configurations
{
    public static class ConstStrings
    {
        public static string DataProvider = nameof(DataProvider);
        public static string Administrators = nameof(Administrators);
        public static string FirstStart = nameof(FirstStart);

        public static string SqlServerConnection = nameof(SqlServerConnection);
        public static string InMemoryConnection = nameof(InMemoryConnection);
        public static string PostgreSQLConnection = nameof(PostgreSQLConnection);
        public static string SQLiteConnection = nameof(SQLiteConnection);
    }

    public static enum ProviderName
    {
        SqlServer, InMemory, PostgreSQL, SQLite
    }
}
