using Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab3
{
	public class GrammarOps
	{
        private Dictionary<Nonterminal, HashSet<Terminal>> firstSets = new Dictionary<Nonterminal, HashSet<Terminal>>();
        private Dictionary<Nonterminal, HashSet<Terminal>> followSets = new Dictionary<Nonterminal, HashSet<Terminal>>();


        public GrammarOps(IGrammar g)
		{
			this.g = g;
			compute_empty();
            ComputeFirstSets();
            ComputeFollowSets();
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

        public void ComputeFirstSets()
        {
            // Initialize firstSets for each nonterminal with an empty set
            foreach (var nt in g.Nonterminals)
            {
                firstSets[nt] = new HashSet<Terminal>();
            }

            bool changed;
            do
            {
                changed = false;
                foreach (var rule in g.Rules)
                {
                    var lhs = rule.LHS;
                    bool canLeadToEpsilon = true;

                    foreach (var symbol in rule.RHS)
                    {
                        if (symbol is Terminal terminal)
                        {
                            // If a terminal is found, add it to the FIRST set and break, as it cannot lead to epsilon directly
                            if (firstSets[lhs].Add(terminal)) changed = true;
                            canLeadToEpsilon = false;
                            break;
                        }
                        else if (symbol is Nonterminal nt)
                        {
                            // Attempt to add all FIRST symbols from nt to lhs, except epsilon initially
                            foreach (var firstSymbol in firstSets[nt].Where(t => t.Name != "{e}"))
                            {
                                if (firstSets[lhs].Add(firstSymbol)) changed = true;
                            }
                            // If nt can lead to epsilon, continue to check the next symbol
                            if (!firstSets[nt].Contains(new Terminal("{e}")))
                            {
                                canLeadToEpsilon = false;
                                break; // nt cannot lead to epsilon, stop processing this rule
                            }
                        }
                    }

                    // If all symbols can lead to epsilon or the RHS is empty, add epsilon to lhs
                    if (canLeadToEpsilon && rule.RHS.Any())
                    {
                        if (firstSets[lhs].Add(new Terminal("{e}"))) changed = true;
                    }
                }
            } while (changed);
        }



        public void ComputeFollowSets()
        {
            // Initialize followSets for each nonterminal with an empty set
            foreach (var nt in g.Nonterminals)
            {
                followSets[nt] = new HashSet<Terminal>();
            }

            // Assume a designated method or property to identify the start symbol if needed
            // For simplicity, we'll add '$' to the follow set of the first nonterminal found
            // This may need adjustment based on your grammar's start symbol identification
            if (g.Nonterminals.Any())
            {
                followSets[g.Nonterminals.First()].Add(new Terminal("$"));
            }

            bool changed;
            do
            {
                changed = false;
                foreach (var rule in g.Rules)
                {
                    var lhs = rule.LHS;
                    for (int i = 0; i < rule.RHS.Count; i++)
                    {
                        var symbol = rule.RHS[i];
                        if (symbol is Nonterminal nt)
                        {
                            var beforeFollowSetSize = followSets[nt].Count;

                            // If it's the last symbol, add FOLLOW(lhs) to FOLLOW(nt)
                            if (i == rule.RHS.Count - 1)
                            {
                                foreach (var follow in followSets[lhs])
                                {
                                    if (followSets[nt].Add(follow))
                                    {
                                        changed = true;
                                    }
                                }
                            }
                            else
                            {
                                var nextSymbols = rule.RHS.Skip(i + 1).ToList();
                                bool nextLeadsToEpsilon = true;

                                foreach (var nextSymbol in nextSymbols)
                                {
                                    if (nextSymbol is Terminal terminal)
                                    {
                                        nextLeadsToEpsilon = false;
                                        if (followSets[nt].Add(terminal))
                                        {
                                            changed = true;
                                        }
                                        break; // Stop at the first terminal
                                    }
                                    else if (nextSymbol is Nonterminal nextNt)
                                    {
                                        // Add FIRST(nextNt) to FOLLOW(nt), excluding epsilon (implicitly handled)
                                        foreach (var first in firstSets[nextNt])
                                        {
                                            if (followSets[nt].Add(first))
                                            {
                                                changed = true;
                                            }
                                        }
                                        nextLeadsToEpsilon = firstSets[nextNt].Contains(new Terminal("")); // Assuming "" represents epsilon
                                        if (!nextLeadsToEpsilon) break;
                                    }
                                }

                                // If all symbols after nt can lead to epsilon, add FOLLOW(lhs) to FOLLOW(nt)
                                if (nextLeadsToEpsilon)
                                {
                                    foreach (var follow in followSets[lhs])
                                    {
                                        if (followSets[nt].Add(follow))
                                        {
                                            changed = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } while (changed);
        }

        public void DisplayFirstSets()
        {
            Console.WriteLine("FIRST:");
            foreach (var rule in g.Rules)
            {
                var firstSetDisplay = new HashSet<string>(); // To store unique FIRST symbols for this rule
                bool canLeadToEpsilon = true;

                foreach (var symbol in rule.RHS)
                {
                    if (symbol is Terminal terminal)
                    {
                        // Directly add terminal and break, as terminals do not lead to epsilon
                        firstSetDisplay.Add(terminal.Name);
                        canLeadToEpsilon = false;
                        break;
                    }
                    else if (symbol is Nonterminal nt)
                    {
                        // Add all FIRST symbols from nt to the display set, except epsilon initially
                        foreach (var firstSymbol in firstSets[nt])
                        {
                            if (firstSymbol.Name != "{e}")
                            {
                                firstSetDisplay.Add(firstSymbol.Name);
                            }
                            else
                            {
                                // Only add epsilon if it's the last symbol or if every symbol can lead to epsilon
                                if (rule.RHS.Last().Equals(symbol))
                                {
                                    firstSetDisplay.Add("{e}");
                                }
                            }
                        }

                        // If the current nt cannot lead to epsilon, stop processing further symbols
                        if (!firstSets[nt].Contains(new Terminal("{e}")))
                        {
                            canLeadToEpsilon = false;
                            break;
                        }
                    }
                }

                // Construct the rule's RHS as a string for display
                var rhsDisplay = string.Join(" ", rule.RHS.Select(s => s.Name.Replace(" ", "")));

                // If canLeadToEpsilon is still true by the end, it means all symbols can lead to epsilon or it's an epsilon rule directly
                if (canLeadToEpsilon && !firstSetDisplay.Contains("{e}"))
                {
                    firstSetDisplay.Add("{e}");
                }

                // Display the FIRST set for this rule
                Console.WriteLine($"first[{rule.LHS.Name}:{rhsDisplay}] = {string.Join(" ", firstSetDisplay)}");
            }
        }


        // Method to display the FOLLOW sets
        public void DisplayFollowSets()
        {
            Console.WriteLine("FOLLOW sets:");
            foreach (var pair in followSets)
            {
                string followSetString = string.Join(" ", pair.Value.Select(t => t.Name));
                Console.WriteLine($"FOLLOW[{pair.Key.Name}] = { followSetString }");
            }
        }
    }
}
