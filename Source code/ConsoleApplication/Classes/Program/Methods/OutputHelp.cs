// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="OutputHelp.cs">
//    Copyright © 2019 eMedia Intellect.
// </copyright>
// <licence>
//    This file is part of eMI Spanish Verb Conjugator.
//
//    eMI Spanish Verb Conjugator is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    eMI Spanish Verb Conjugator is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with eMI Spanish Verb Conjugator. If not, see http://www.gnu.org/licenses/.
// </licence>

namespace Emi.SpanishVerbConjugator.ConsoleApplication
{
	using System;
	using System.Globalization;
	using System.Linq;

	public static partial class Program
	{
		private static void OutputHelp()
		{
			string[] helpProgramModeOptions = new string[] { "--help" };

			foreach (Option option in options)
			{
				if (!helpProgramModeOptions.Contains<string>(option.Name))
				{
					Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid option with the \"--help\" option. See \"--help\" for further information.", option.Name));
					Console.WriteLine();

					Environment.Exit(1);
				}
			}

			Console.WriteLine("┌────────────────┐");
			Console.WriteLine("│ 1. Conjugation │");
			Console.WriteLine("└────────────────┘");
			Console.WriteLine();
			Console.WriteLine("svc [[--verb] VERB] [OPTION]...");
			Console.WriteLine();
			Console.WriteLine("┌────────────────────────────────────┐");
			Console.WriteLine("│ 1.1. Conjugation verb form options │");
			Console.WriteLine("└────────────────────────────────────┘");
			Console.WriteLine();
			Console.WriteLine("--conditional");
			Console.WriteLine("--future-indicative");
			Console.WriteLine("--future-subjunctive");
			Console.WriteLine("--gerund");
			Console.WriteLine("--imperative");
			Console.WriteLine("--imperative-affirmative");
			Console.WriteLine("--imperative-negative");
			Console.WriteLine("--imperfect-indicative");
			Console.WriteLine("--imperfect-ra-subjunctive");
			Console.WriteLine("--imperfect-se-subjunctive");
			Console.WriteLine("--imperfect-subjunctive");
			Console.WriteLine("--indicative");
			Console.WriteLine("--infinitive");
			Console.WriteLine("--past-participle");
			Console.WriteLine("--present-indicative");
			Console.WriteLine("--present-subjunctive");
			Console.WriteLine("--preterite-indicative");
			Console.WriteLine("--subjunctive");
			Console.WriteLine();
			Console.WriteLine("┌─────────────────────────────────┐");
			Console.WriteLine("│ 1.2. Conjugation output options │");
			Console.WriteLine("└─────────────────────────────────┘");
			Console.WriteLine();
			Console.WriteLine("--raw");
			Console.WriteLine();
			Console.WriteLine("┌────────────────┐");
			Console.WriteLine("│ 2. Information │");
			Console.WriteLine("└────────────────┘");
			Console.WriteLine();
			Console.WriteLine("svc --count [--inflection regular | irregular] [--ending ar | er | ir] [--defective]");
			Console.WriteLine("svc --generate PATH [--inflection regular | irregular] [--ending ar | er | ir] [--defective]");
			Console.WriteLine("svc --help");
			Console.WriteLine("svc --licence");
			Console.WriteLine("svc --show [--inflection regular | irregular] [--ending ar | er | ir] [--defective]");
			Console.WriteLine();
		}
	}
}