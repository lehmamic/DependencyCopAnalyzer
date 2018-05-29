namespace DependencyCop.Analyzers.Settings
{
    public class DependencyRuleEntry
    {
        public DependencyRuleType Type { get; set; }

        public string From { get; set; }

        public string To { get; set; }
    }
}