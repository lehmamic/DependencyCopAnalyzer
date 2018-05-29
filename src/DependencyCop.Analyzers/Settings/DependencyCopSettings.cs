namespace DependencyCop.Analyzers.Settings
{
    public class DependencyCopSettings
    {
        public static DependencyCopSettings Default => new DependencyCopSettings
            {
                Rules = new[] {
                    new DependencyRule { Type = DependencyRuleType.Allow, From = "*", To="*" }
                }
            };

        public DependencyRule[] Rules { get; set; }
    }
}