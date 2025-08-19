namespace InferenceEngine;

public class BackwardChainingMethod
{
    private static List<string> _kb;
    private static string _query;
    
    public BackwardChainingMethod(List<string> kb, string query)
    {
        _kb = kb;
        _query = query;
    }
    // Implements backward chaining
    public void PerformMethod()
    {
        //Extract rules from the knowledge base
        var rules = _kb.Where(s => s.Contains("=>"))
            .Select(rule => new Rule
            {
                Premises = rule.Split("=>")[0].Split('&').Select(p => p.Trim()).ToList(),
                Conclusion = rule.Split("=>")[1].Trim()
            }).ToList();

        var inferred = new HashSet<string>();
        if (BCRecursive(_query, rules, inferred))
        {
            Console.WriteLine($"YES: {string.Join(", ", inferred)}");
        }
        else
        {
            Console.WriteLine("NO");
        }
    }

    // Recursive function for backward chaining 
    private static bool BCRecursive(string query, List<Rule> rules, HashSet<string> inferred)
    {
        if (inferred.Contains(query))
        {
            return true;
        }

        if (!rules.Any(r => r.Conclusion == query))
        {
            inferred.Add(query);
            return true;
        }

        foreach (var rule in rules.Where(r => r.Conclusion == query))
        {
            // Recursively verify all premises
            if (rule.Premises.All(p => BCRecursive(p, rules, inferred)))
            {
                inferred.Add(query);
                return true;
            }
        }

        return false;
    }
}