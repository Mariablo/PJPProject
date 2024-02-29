using Grammar;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Lab3
{

	class Program
	{
		static void Main(string[] args)
		{
            //LAB3
            try
			{
				StreamReader r = new StreamReader(new FileStream("G1.TXT", FileMode.Open));

				GrammarReader inp = new GrammarReader(r);
				var grammar = inp.Read();
				grammar.dump();

				GrammarOps gr = new GrammarOps(grammar);

				// First step, computes nonterminals that can be rewritten as empty word
				foreach (Nonterminal nt in gr.EmptyNonterminals)
				{
					Console.Write(nt.Name + " ");
				}
				Console.WriteLine();
			}
			catch (GrammarException e)
			{
				Console.WriteLine($"{e.LineNumber}: Error -  {e.Message}");
			}
			catch (IOException e)
			{
				Console.WriteLine(e);
			}

            // LAB1
            /*string input = Console.ReadLine();
            if (!int.TryParse(input, out int count_of_operation))
            {
                Console.WriteLine("You must enter a number.");
                return;
            }

            List<string> results = new List<string>(); // Use a list to store results or errors

            for (int i = 0; i < count_of_operation; i++)
            {
                string expression = Console.ReadLine();
                try
                {
                    // Evaluate the expression using the Evaluation class
                    int result = Evaluation.evaluate(expression);
                    results.Add(result.ToString()); // Store the result as a string
                }
                catch (Exception ex)
                {
                    results.Add("ERROR"); // Store "ERROR" in case of an exception
                }
            }

            // Print all results or errors
            foreach (string result in results)
            {
                Console.WriteLine(result);
            }*/

            //LAB2
            /*string input = @"
            -2 + (245 div 3);  // note
            2 mod 3 * hello";

            var tokens = Lexical.Analyze(input);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }

            Console.ReadLine(); // Wait for user input before closing*/
        }
	}
}