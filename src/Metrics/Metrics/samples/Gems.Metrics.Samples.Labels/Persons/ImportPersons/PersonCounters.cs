namespace Gems.Metrics.Samples.Labels.Persons.ImportPersons
{
    public record PersonCounters
    {
        public int Added { get; init; }

        public int Updated { get; init; }

        public int Deleted { get; init; }
    }
}
