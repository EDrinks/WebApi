namespace EDrinks.EventSourceSql.Model
{
    public class User
    {
        public long Id { get; set; }

        public string AuthIdentifier { get; set; }

        public string EventDbFile { get; set; }
    }
}