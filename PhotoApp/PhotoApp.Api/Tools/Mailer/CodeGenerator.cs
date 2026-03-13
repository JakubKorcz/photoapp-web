namespace PhotoApp.Api.Tools.Mailer
{
    public class CodeGenerator
    {
        private readonly Random _random = new Random();
        public string Generate()
        {
            var number = _random.Next(0, 1000000);
            return number.ToString("D6");
        }
    }
}
