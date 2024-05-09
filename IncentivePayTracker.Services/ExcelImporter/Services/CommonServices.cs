namespace ExcelImporter.Services;

public static class CommonServices
{

    public static int ToMonth(this string path)
    {
        return IncentivePayTrackerReader.Months[Path.GetFileNameWithoutExtension(path).Split(' ').First()];
    }

    public static int ToYear(this string path)
    {
        return int.Parse(Path.GetFileNameWithoutExtension(path).Split(' ').Last());
    }
}
