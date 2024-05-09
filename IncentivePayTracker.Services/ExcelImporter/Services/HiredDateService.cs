namespace ExcelImporter.Services;

public class HiredDateService
{
    private class Record
    {
        private DateOnly _dateHired = DateOnly.FromDateTime(DateTime.UtcNow);
        private DateOnly? _dateTerminated = new DateOnly();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateHired
        {
            get => _dateHired;
            set
            {
                if (value < _dateHired)
                {
                    _dateHired = value;
                }
            }
        }

        public DateOnly? DateTerminated
        {
            get => _dateTerminated;
            set
            {
                if (value > _dateTerminated)
                {
                    _dateTerminated = value;
                }
            }
        }
    }

    public void GenerateBatch(params string[] paths)
    {
        var records = new List<Record>();

        var orderedPaths = paths.OrderBy(GetYear).ThenBy(GetMonth);

        foreach (var path in orderedPaths)
        {
            int month = GetMonth(path);
            int year = GetYear(path);

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string? data = reader.ReadLine();

                    if (string.IsNullOrEmpty(data)) continue;

                    string firstName = data.Split('/')[1].Split(',', StringSplitOptions.TrimEntries).Last();
                    string lastName = data.Split('/')[1].Split(',', StringSplitOptions.TrimEntries).First();

                    var existingRecord = records.FirstOrDefault(r => r.FirstName == firstName && r.LastName == lastName);

                    if (existingRecord is null)
                    {
                        existingRecord = new Record { FirstName = firstName, LastName = lastName };
                        records.Add(existingRecord);
                    }


                    existingRecord.DateHired = DateOnly.FromDateTime(new DateTime(year, month, 1));
                    existingRecord.DateTerminated = DateOnly.FromDateTime(new DateTime(year, month, 1));
                }
            }
        }

        using (StreamWriter writer = new StreamWriter("batch.txt"))
        {
            int id = 1;
            foreach (var record in records)
            {
                if (record.DateTerminated.Value.Year == 2024 && record.DateTerminated.Value.Month == 4)
                {
                    writer.WriteLine($"INSERT INTO EmploymentDates (Id, EmployeeId, YearHired, MonthHired, YearTerminated, MonthTerminated) SELECT {id++}, Id, {record.DateHired.Year}, {record.DateHired.Month}, NULL, NULL FROM Employees WHERE FirstName = '{record.FirstName}' AND LastName = '{record.LastName}';");
                }
                else
                {
                    writer.WriteLine($"INSERT INTO EmploymentDates (Id, EmployeeId, YearHired, MonthHired, YearTerminated, MonthTerminated) SELECT {id++}, Id, {record.DateHired.Year}, {record.DateHired.Month}, {record.DateTerminated.Value.Year}, {record.DateTerminated.Value.Month} FROM Employees WHERE FirstName = '{record.FirstName}' AND LastName = '{record.LastName}';");
                }
            }
        }
    }

    public int GetMonth(string path)
    {
        return IncentivePayTrackerReader.Months[Path.GetFileNameWithoutExtension(path).Split(' ').First()];
    }

    public int GetYear(string path)
    {
        return int.Parse(Path.GetFileNameWithoutExtension(path).Split(' ').Last());
    }
}
