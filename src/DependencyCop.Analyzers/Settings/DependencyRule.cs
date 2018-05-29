namespace DependencyCop.Analyzers.Settings
{
    public class DependencyRule
    {
        public DependencyRuleType Type { get; set; }

        public string From { get; set; }

        public string To { get; set; }
    }
}