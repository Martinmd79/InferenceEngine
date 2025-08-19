namespace InferenceEngine
{
    class InferenceEngine
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: iengine <method> <filename>");
                return;
            }

            string method = args[0].ToUpper();
            string filename = args[1];

            var (kb, query) = ParseInputFile(filename);
            switch (method)
            {
                case "TT":
                    var tt = new TruthTableMethod(kb, query);
                    tt.PerformMethod();
                    break;
                case "FC":
                    var fc = new ForwardChainingMethod(kb, query);
                    fc.PerformMethod();
                    break;
                case "BC":
                    var bc = new BackwardChainingMethod(kb, query);
                    bc.PerformMethod();
                    break;
                case "RES":
                    var res = new ResolutionMethod(kb, query);
                    res.PerformMethod();
                    break;
                default:
                    Console.WriteLine("Invalid method. Use TT, FC, or BC.");
                    break;
            }
        }

        private static (List<string>, string) ParseInputFile(string filename)
        {
            var lines = File.ReadAllLines(filename);
            var kb = new List<string>();
            string query = "";
            bool inTellSection = false;


            foreach (var line in lines.Select(l => l.Trim()))
            {
                if (line.StartsWith("TELL"))
                {
                    inTellSection = true;
                }
                else if (inTellSection)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        kb.AddRange(line.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));
                        inTellSection = false;
                    }
                }
                else if (line.StartsWith("ASK"))
                {
                    inTellSection = false;
                }
                else if (!inTellSection)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        query = line;
                    }
                }
            }

            if (!kb.Any())
            {
                Console.WriteLine("Error: Knowledge base is missing in the input file.");
            }

            if (string.IsNullOrEmpty(query))
            {
                Console.WriteLine("Error: Query is missing in the input file.");
            }

            return (kb, query);
        }
    }
}