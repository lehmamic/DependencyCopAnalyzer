using System.Collections.Generic;
using System.Linq;
using DependencyCop.Analyzers.Settings;

namespace DependencyCop.Analyzers
{
    public class DependencyValidator
    {
        private readonly Dictionary<DependencyRuleType, IEnumerable<DependencyRule>> ruleSet;

        public DependencyValidator(DependencyCopSettings settings)
        {
            this.ruleSet = settings.Rules
                .GroupBy(r => r.Type)
                .ToDictionary(r => r.Key, r => r.Select(i => new DependencyRule(i.From, i.To)));
        }

        public bool IsAllowedDependency(Namespace fromNamespace, Namespace toNamespace)
        {
            var deniedRules = this.GetMatchingRules(DependencyRuleType.Deny, fromNamespace, toNamespace);
            if(deniedRules.Any())
            {
                return false;
            }

            var allowedRules = this.GetMatchingRules(DependencyRuleType.Allow, fromNamespace, toNamespace);
            return allowedRules.Any();
        }

        private IEnumerable<DependencyRule> GetMatchingRules(DependencyRuleType ruleType, Namespace from, Namespace to)
        {
            if(ruleSet.TryGetValue(ruleType, out var rules))
            {
                return rules.Where(i => i.From.Matches(from) && i.To.Matches(to));
            }

            return Enumerable.Empty<DependencyRule>();
        }
    }
}