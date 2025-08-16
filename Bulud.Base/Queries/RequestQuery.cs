namespace Bulud.Base.Queries
{
    public class RequestQuery
    {
        public int Page { get; set; } = 1;
        public int Length { get; set; } = 10;
        public string? Search { get; set; }
        public string? Filter { get; set; }
        public string? Sort { get; set; }
        public string? Include { get; set; }
        public bool CountOnly { get; set; } = false;
        
        public void AppendInclude(string include)
        {
            if (string.IsNullOrWhiteSpace(include))
                return;

            var includes = (Include ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            if (!includes.Contains(include))
                includes.Add(include);

            Include = string.Join(",", includes);
        }
        
        public void AppendFilter(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return;

            var filters = (Filter ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            if (!filters.Contains(filter))
                filters.Add(filter);

            Filter = string.Join(",", filters);
        }
    }
}
