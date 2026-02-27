namespace PhotoApp.Api.Mailer
{
    public class CodeGenerator
    {
        private static readonly Random _random = new Random();
        public int Generate()
        {
            return _random.Next(100000, 999999);
        }
    }
}
