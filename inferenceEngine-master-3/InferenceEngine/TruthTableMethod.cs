using System;
using System.Collections.Generic;
using System.Linq;

namespace InferenceEngine
{
    public class TruthTableMethod
    {
        private static List<string> _knowledgeBase;
        private static string _goal;
        private static List<Dictionary<string, bool>> _allModels;
        private static int _countOfValidModels;

        public TruthTableMethod(List<string> kb, string query)
        {
            _knowledgeBase = kb;
            _goal = query;

            var symbols = CollectSymbols(_knowledgeBase);
            _allModels = GenerateAllPossibleModels(symbols);
            _countOfValidModels = 0;
        }

        public void PerformMethod()
        {
            foreach (var assignment in _allModels)
            {
                

                if (IsKnowledgeBaseTrue(_knowledgeBase, assignment))
                {
                    _countOfValidModels++;

                    bool queryResult = InterpretExpression(_goal, assignment);

                    // If any valid model makes the query false, entailment fails.
                    if (!queryResult)
                    {
                        Console.WriteLine("NO");
                        return;
                    }
                }
            }
            Console.WriteLine($"YES: {_countOfValidModels}");
        }

        private static bool IsKnowledgeBaseTrue(List<string> kb, Dictionary<string, bool> model)
        {
            foreach (var clause in kb)
            {
                if (!InterpretExpression(clause, model))
                    return false;
            }
            return true;
        }

        private static bool InterpretExpression(string expr, Dictionary<string, bool> model)
        {
            expr = expr.Trim();

            // Remove only outer parentheses that actually enclose the entire expression.
            while (IsEnclosedInParentheses(expr))
            {
                expr = expr.Substring(1, expr.Length - 2).Trim();
            }

            if (string.IsNullOrEmpty(expr))
                return false;

            // Process biconditional ("<=>")
            int index = FindTopLevelOperatorIndex(expr, "<=>");
            if (index != -1)
            {
                string left = expr.Substring(0, index);
                string right = expr.Substring(index + 3); // Length of "<=>"
                return InterpretExpression(left, model) == InterpretExpression(right, model);
            }

            // Process implication ("=>")
            index = FindTopLevelOperatorIndex(expr, "=>");
            if (index != -1)
            {
                string left = expr.Substring(0, index);
                string right = expr.Substring(index + 2); // Length of "=>"
                bool lhs = InterpretExpression(left, model);
                bool rhs = InterpretExpression(right, model);
                return (!lhs) || rhs;
            }

            // Process disjunction ("||")
            index = FindTopLevelOperatorIndex(expr, "||");
            if (index != -1)
            {
                var disjuncts = SplitByTopLevelOperator(expr, "||");
                foreach (var part in disjuncts)
                {
                    if (InterpretExpression(part, model))
                        return true;
                }
                return false;
            }

            // Process conjunction ("&")
            index = FindTopLevelOperatorIndex(expr, "&");
            if (index != -1)
            {
                var conjuncts = SplitByTopLevelOperator(expr, "&");
                foreach (var part in conjuncts)
                {
                    if (!InterpretExpression(part, model))
                        return false;
                }
                return true;
            }

            if (expr.StartsWith("~"))
            {
                string rest = expr.Substring(1).Trim();
                return !InterpretExpression(rest, model);
            }

            return model.TryGetValue(expr, out bool val) && val;
        }

        private static int FindTopLevelOperatorIndex(string expr, string op)
        {
            int parenCount = 0;
            for (int i = 0; i <= expr.Length - op.Length; i++)
            {
                if (expr[i] == '(')
                    parenCount++;
                else if (expr[i] == ')')
                    parenCount--;

                if (parenCount == 0 && expr.Substring(i, op.Length) == op)
                    return i;
            }
            return -1;
        }

      
        private static List<string> SplitByTopLevelOperator(string expr, string op)
        {
            var tokens = new List<string>();
            int parenCount = 0;
            int start = 0;
            for (int i = 0; i <= expr.Length - op.Length; i++)
            {
                if (expr[i] == '(')
                    parenCount++;
                else if (expr[i] == ')')
                    parenCount--;

                if (parenCount == 0 && expr.Substring(i, op.Length) == op)
                {
                    tokens.Add(expr.Substring(start, i - start).Trim());
                    start = i + op.Length;
                    i += op.Length - 1;
                }
            }
            tokens.Add(expr.Substring(start).Trim());
            return tokens;
        }

        private static IEnumerable<string> CollectSymbols(IEnumerable<string> clauses)
        {
            var symbolSet = new HashSet<string>();

            foreach (var statement in clauses)
            {
                string withoutParens = statement.Replace("(", "").Replace(")", "");
                var tokens = withoutParens.Split(new[] { "<=>", "=>", "||", "&" },
                                                  StringSplitOptions.RemoveEmptyEntries);

                foreach (var token in tokens)
                {
                    string clean = token.Trim();
                    if (clean.StartsWith("~"))
                        clean = clean.Substring(1).Trim();
                    if (!string.IsNullOrEmpty(clean))
                        symbolSet.Add(clean);
                }
            }
            return symbolSet;
        }

        
        private static List<Dictionary<string, bool>> GenerateAllPossibleModels(IEnumerable<string> symbols)
        {
            var symbolList = symbols.ToList();
            var allCombinations = new List<Dictionary<string, bool>>();
            int numberOfSymbols = symbolList.Count;
            int totalAssignments = 1 << numberOfSymbols; // 2^n assignments

            for (int i = 0; i < totalAssignments; i++)
            {
                var model = new Dictionary<string, bool>();
                for (int bitIndex = 0; bitIndex < numberOfSymbols; bitIndex++)
                {
                    bool value = ((i >> bitIndex) & 1) == 1;
                    model[symbolList[bitIndex]] = value;
                }
                allCombinations.Add(model);
            }
            return allCombinations;
        }

        
        private static bool IsEnclosedInParentheses(string expr)
        {
            expr = expr.Trim();
            if (expr.Length < 2 || expr[0] != '(' || expr[expr.Length - 1] != ')')
                return false;

            int count = 0;
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == '(')
                    count++;
                else if (expr[i] == ')')
                    count--;
                if (count == 0 && i < expr.Length - 1)
                    return false;
            }
            return (count == 0);
        }

        private static bool MatchingBrackets(string expression)
        {
            int count = 0;
            foreach (char c in expression)
            {
                if (c == '(') count++;
                else if (c == ')') count--;
                if (count < 0)
                    return false;
            }
            return (count == 0);
        }
    }
}

