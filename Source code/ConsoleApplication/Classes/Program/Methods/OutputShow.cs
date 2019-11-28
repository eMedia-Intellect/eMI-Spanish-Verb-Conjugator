// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="OutputShow.cs">
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
	using System.Linq;
	using Library;

	public static partial class Program
	{
		private static void OutputShow()
		{
			bool showIrregular = true;
			bool showRegular = true;

			bool showArEndings = true;
			bool showErEndings = true;
			bool showIrEndings = true;

			bool hasDefectiveFilter = false;

			List<string> verbs = new List<string>();

			string[] showProgramModeOptions = new string[] { "--defective", "--ending", "--inflection", "--show" };

			foreach (Option option in options)
			{
				if (!showProgramModeOptions.Contains<string>(option.Name))
				{
					Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid option with the \"--show\" option. See \"--help\" for further information.", option.Name));
					Console.WriteLine();

					Environment.Exit(1);
				}

				switch (option.Name)
				{
					case "--defective":
						hasDefectiveFilter = true;

						break;
					case "--ending":
						if (option.Count != 0)
						{
							switch (option[0])
							{
								case "ar":
									showArEndings = true;
									showErEndings = false;
									showIrEndings = false;

									break;
								case "er":
									showArEndings = false;
									showErEndings = true;
									showIrEndings = false;

									break;
								case "ir":
									showArEndings = false;
									showErEndings = false;
									showIrEndings = true;

									break;
								default:
									Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid argument for the \"--ending\" option. See \"--help\" for further information.", option[0]));
									Console.WriteLine();

									Environment.Exit(1);
									break;
							}
						}

						break;
					case "--inflection":
						if (option.Count != 0)
						{
							switch (option[0])
							{
								case "irregular":
									showIrregular = true;
									showRegular = false;

									break;
								case "regular":
									showIrregular = false;
									showRegular = true;

									break;
								default:
									Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid argument for the \"--show\" option. See \"--help\" for further information.", option[0]));
									Console.WriteLine();

									Environment.Exit(1);
									break;
							}
						}

						break;
				}
			}

			if (showIrregular)
			{
				if (showArEndings)
				{
					verbs.AddRange(VerbManager.IrregularArVerbs);
				}

				if (showErEndings)
				{
					verbs.AddRange(VerbManager.IrregularErVerbs);
				}

				if (showIrEndings)
				{
					verbs.AddRange(VerbManager.IrregularIrVerbs);
				}
			}

			if (showRegular)
			{
				if (showArEndings)
				{
					verbs.AddRange(VerbManager.RegularArVerbs);
				}

				if (showErEndings)
				{
					verbs.AddRange(VerbManager.RegularErVerbs);
				}

				if (showIrEndings)
				{
					verbs.AddRange(VerbManager.RegularIrVerbs);
				}
			}

			verbs.Sort();

			if (!hasDefectiveFilter)
			{
				foreach (string verb in verbs)
				{
					Console.WriteLine(verb);
				}
			}
			else
			{
				foreach (string verb in verbs)
				{
					foreach (string defectiveVerb in VerbManager.DefectiveVerbs)
					{
						if (verb == defectiveVerb)
						{
							Console.WriteLine(verb);
						}
					}
				}
			}

			Console.WriteLine();
		}
	}
}