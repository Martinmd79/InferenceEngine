namespace InferenceEngine;

public class ForwardChainingMethod
{
    private static List<string> _kb;
    private static string _query;

    public ForwardChainingMethod(List<string> kb, string query)
    {
        _kb = kb;
        _query = query;
    }

    // Implements forward chaining
    public void PerformMethod()
    {
        var inferred = new HashSet<string>();
        var agenda = new Queue<string>(_kb.Where(s => !s.Contains("=>")));

        // Extract rules from the knowledge base
        var rules = _kb.Where(s => s.Contains("=>"))
            .Select(rule => new Rule
            {
                Premises = rule.Split("=>")[0].Split('&').Select(p => p.Trim()).ToList(),
                Conclusion = rule.Split("=>")[1].Trim()
            }).ToList();

        while (agenda.Count > 0)
        {
            var fact = agenda.Dequeue();
            if (inferred.Contains(fact))
                continue;

            inferred.Add(fact);
            foreach (var rule in rules)
            {
                // Check if all premises of the rule are satisfied.
                if (rule.Premises.All(p => inferred.Contains(p)))
                {
                    if (!inferred.Contains(rule.Conclusion))
                    {
                        agenda.Enqueue(rule.Conclusion);
                        if (rule.Conclusion == _query)
                        {
                            Console.WriteLine($"YES: {string.Join(", ", inferred)} , " + _query);
                            return;
                        }
                    }
                }
            }
        }

        Console.WriteLine("NO");
    }
}