using ExcelImporter.Models;
using IncentivePayTracker.DTO;
using OfficeOpenXml;
using System.Globalization;

namespace ExcelImporter.Services;

public class IncentivePayTrackerReader
{
    private readonly double[] Infractions = [200, 200, 200, 500, 300, 500, 1000, 200, 500];
    public static Dictionary<string, int> Months = new Dictionary<string, int>()
    {
        { "January", 1 },
        { "February", 2 },
        { "March", 3 },
        { "April", 4 },
        { "May", 5 },
        { "June", 6 },
        { "July", 7 },
        { "August", 8 },
        { "September", 9 },
        { "October", 10 },
        { "November", 11 },
        { "December", 12 },
    };
    public List<CompositeEmployeeInfraction> Read(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }

        List<CompositeEmployeeInfraction> employeeInfractions = new List<CompositeEmployeeInfraction>();

        using (StreamReader reader = new StreamReader(path))
        {
            while (!reader.EndOfStream)
            {
                string? data = reader.ReadLine();

                if (!string.IsNullOrEmpty(data))
                {
                    string[] strings = data.Split('/');

                    var employee = new Employee
                    {
                        IsExempted = "Y".Equals(strings[0], StringComparison.OrdinalIgnoreCase),
                        FirstName = strings[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? string.Empty,
                        LastName = strings[1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty,
                    };

                    var infractions = new List<CompositeInfraction>();

                    foreach (var inf in Enumerable.Range(3, 9))
                    {
                        string? value = strings[inf];

                        if (string.IsNullOrEmpty(value)) continue;

                        var infraction = new CompositeInfraction
                        {
                            Count = (int)(double.Parse(value) / Infractions[inf - 3]),
                            Infraction = new Infraction { Id = inf - 2, Amount = 0, Description = string.Empty }
                        };

                        infractions.Add(infraction);
                    }

                    var empInf = new CompositeEmployeeInfraction
                    {
                        Employee = employee,
                        Infractions = infractions,
                        Month = Months[Path.GetFileNameWithoutExtension(path).Split(' ').FirstOrDefault() ?? "January"],
                        Year = int.Parse(Path.GetFileNameWithoutExtension(path).Split(' ').LastOrDefault())
                    };

                    employeeInfractions.Add(empInf);
                }
            }

            return employeeInfractions;
        }
    }
}
