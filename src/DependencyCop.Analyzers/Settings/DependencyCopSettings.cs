namespace DependencyCop.Analyzers.Settings
{
    public class DependencyCopSettings
    {
        public static DependencyCopSettings Default => new DependencyCopSettings
            {
                Rules = new[] {
                    new DependencyRuleEntry { Type = DependencyRuleType.Allow, From = "*", To="*" }
                }
            };

        public DependencyRuleEntry[] Rules { get; set; }
    }
}