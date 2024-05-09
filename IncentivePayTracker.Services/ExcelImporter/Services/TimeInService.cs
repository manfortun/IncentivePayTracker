namespace ExcelImporter.Services;

public class TimeInService
{
    private class Record
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TimeIn { get; set; }
        public DateOnly ApplicableDate => DateOnly.FromDateTime(new DateTime(Year, Month, 1));
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public void GenerateBatch(params string[] paths)
    {
        int id = 1;
        var records = new List<Record>();

        var orderedPaths = paths.OrderBy(p => p.ToYear()).ThenBy(p => p.ToMonth());

        foreach (var path in orderedPaths)
        {
            int month = path.ToMonth();
            int year = path.ToYear();

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string? data = reader.ReadLine();

                    if (string.IsNullOrEmpty(data)) continue;

                    string firstName = data.Split('/')[1].Split(',', StringSplitOptions.TrimEntries).Last();
                    string lastName = data.Split('/')[1].Split(',', StringSplitOptions.TrimEntries).First();
                    string timeIn = data.Split('/')[2];

                    if (!string.IsNullOrEmpty(timeIn))
                    {
                        var newRecord = new Record
                        {
                            Id = id++,
                            FirstName = firstName,
                            LastName = lastName,
                            TimeIn = timeIn,
                            Month = month,
                            Year = year,
                        };

                        records.Add(newRecord);
                    }
                }
            }
        }

        List<int> idsToDelete = new List<int>();
        foreach (var record in records.GroupBy(i => i.FirstName + i.LastName))
        {
            string timeIn = string.Empty;
            foreach (var r in record.OrderBy(i => i.ApplicableDate))
            {
                if (timeIn != r.TimeIn)
                {
                    timeIn = r.TimeIn;
                }
                else
                {
                    idsToDelete.Add(r.Id);
                }
            }
        }

        records = records.Where(r => !idsToDelete.Contains(r.Id)).ToList();

        using (StreamWriter writer = new StreamWriter("time.txt"))
        {
            foreach (var record in records)
            {
                writer.WriteLine($"INSERT INTO EmployeeTimeIns (EmployeeId, Month, Year, TimeIn) SELECT Id, {record.Month}, {record.Year}, '{record.TimeIn}' FROM Employees WHERE FirstName = '{record.FirstName}' AND LastName = '{record.LastName}';");
            }
        }
    }
}
