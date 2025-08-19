namespace InferenceEngine
{
    public class ResolutionMethod
    {
        private List<string> _kb;
        private string _query;
        // private static bool DEBUG = true;

        public ResolutionMethod(List<string> kb, string query)
        {
            _kb = kb;
            _query = query;
        }

        public void PerformMethod()
        {
            // Convert KB sentences to CNF and extract clauses.
            List<Clause> clauses = new List<Clause>();
            foreach (var sentence in _kb)
            {
                if (string.IsNullOrWhiteSpace(sentence))
                    continue;
                Parser parser = new Parser(sentence);
                Expr expr = parser.ParseExpression();
                Expr cnfExpr = CNFConverter.Convert(expr);
                clauses.AddRange(Clause.ExtractClauses(cnfExpr));
            }

            Parser qParser = new Parser(_query);
            Expr queryExpr = qParser.ParseExpression();
            Expr negatedQuery = new Not(queryExpr);
            Expr negatedQueryCNF = CNFConverter.Convert(negatedQuery);

            // Console.WriteLine("Negated query CNF: " + negatedQueryCNF);

            List<Clause> queryClauses = Clause.ExtractClauses(negatedQueryCNF);
            clauses.AddRange(queryClauses);

            /*
            Console.WriteLine("Initial clauses (including negated query):");
            foreach (var c in clauses)
            {
                Console.WriteLine("  " + c);
            }
            */

            bool entails = ResolutionAlgorithm(clauses);
            Console.WriteLine(entails ? "YES" : "NO");
        }

        private bool ResolutionAlgorithm(List<Clause> clauses)
        {
            HashSet<Clause> clauseSet = new HashSet<Clause>(clauses);
            HashSet<Clause> newClauses = new HashSet<Clause>();

            while (true)
            {
                List<Clause> clauseList = clauseSet.ToList();
                int n = clauseList.Count;

                // Try resolving each pair of clauses.
                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        foreach (Clause resolvent in Clause.Resolve(clauseList[i], clauseList[j]))
                        {
                            if (resolvent.IsEmpty())
                            {
                               
                                // Console.WriteLine("Derived empty clause by resolving {0} and {1}", clauseList[i], clauseList[j]);
                                return true;
                            }
                            newClauses.Add(resolvent);
                           
                            // Console.WriteLine("Derived clause {0} by resolving {1} and {2}", resolvent, clauseList[i], clauseList[j]);
                        }
                    }
                }

                if (newClauses.IsSubsetOf(clauseSet))
                {
                   
                    // Console.WriteLine("No new clauses were derived; stopping resolution.");
                    return false;
                }
                clauseSet.UnionWith(newClauses);
            }
        }
    }
}
