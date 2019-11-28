// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="Program.cs">
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
	using System.Collections.Generic;
	using System.Globalization;

	public static partial class Program
	{
		private static IList<Option> options = null;

		private static ProgramMode programMode = ProgramMode.Undetermined;

		private static string[] allProgramModeOptions = new string[] { "--conditional", "--count", "--defective", "--ending", "--future-indicative", "--future-subjunctive", "--generate", "--gerund", "--help", "--imperative", "--imperative-affirmative", "--imperative-negative", "--imperfect-indicative", "--imperfect-ra-subjunctive", "--imperfect-se-subjunctive", "--imperfect-subjunctive", "--indicative", "--infinitive", "--inflection", "--licence", "--license", "--past-participle", "--present-indicative", "--present-subjunctive", "--preterite-indicative", "--raw", "--show", "--subjunctive", "--verb" };

		public static void Main(string[] arguments)
		{
			ArgumentProcessor.Options = allProgramModeOptions;

			try
			{
				options = ArgumentProcessor.Parse(arguments);
			}
			catch (DuplicateArgumentException e)
			{
				Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" was entered more than once. Duplicate options are not accepted. See \"--help\" for further information.", e.Argument));
				Console.WriteLine();

				Environment.Exit(1);
			}
			catch (InvalidArgumentException e)
			{
				Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid option. See \"--help\" for further information.", e.Argument));
				Console.WriteLine();

				Environment.Exit(1);
			}

			foreach (Option option in options)
			{
				if (programMode == ProgramMode.Undetermined)
				{
					switch (option.Name)
					{
						case "--count":
							programMode = ProgramMode.Count;

							break;
						case "--generate":
							programMode = ProgramMode.Generate;

							break;
						case "--help":
							programMode = ProgramMode.Help;

							break;
						case "--licence":
							programMode = ProgramMode.Licence;

							break;
						case "--license":
							programMode = ProgramMode.Licence;

							break;
						case "--show":
							programMode = ProgramMode.Show;

							break;
						case "--translate":
							programMode = ProgramMode.Translation;

							break;
						case "--verb":
							programMode = ProgramMode.Conjugation;

							break;
					}
				}
				else
				{
					break;
				}
			}

			switch (programMode)
			{
				case ProgramMode.Conjugation:
					OutputConjugation();

					break;
				case ProgramMode.Count:
					OutputCount();

					break;
				case ProgramMode.Generate:
					OutputGenerate();

					break;
				case ProgramMode.Help:
					OutputHelp();

					break;
				case ProgramMode.Licence:
					OutputLicence();

					break;
				case ProgramMode.Show:
					OutputShow();

					break;
				case ProgramMode.Undetermined:
					OutputConjugation();

					break;
			}
		}
	}
}