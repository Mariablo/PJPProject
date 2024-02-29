using Grammar;
using System.Collections.Generic;
using System.Linq;

namespace Lab3
{
	public class GrammarOps
	{
		public GrammarOps(IGrammar g)
		{
			this.g = g;
			compute_empty();
		}

		public ISet<Nonterminal> EmptyNonterminals { get; } = new HashSet<Nonterminal>();
        private void compute_empty()
        {
            ///TODO: Add your code here...
            foreach (var rule in g.Rules)
            {
                if (rule.RHS.Count == 0)
                {
                    EmptyNonterminals.Add(rule.LHS);

                }
            }

            int sizeBefore = EmptyNonterminals.Count;
            do
            {
                sizeBefore = EmptyNonterminals.Count;
                foreach (var rule in g.Rules)
                {
                    if (rule.RHS.All(x => x is Nonterminal && EmptyNonterminals.Contains((Nonterminal)x)))
                    {
                        EmptyNonterminals.Add((Nonterminal)rule.LHS);
                    }
                }
            }
            while (sizeBefore != EmptyNonterminals.Count);
        }

        private IGrammar g;
	}
}
