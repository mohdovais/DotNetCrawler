namespace Crawler;

public class RobotsTxtParser
{
    public static void Parse(Stream stream)
    {
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            Console.WriteLine(line);
        }
    }
}