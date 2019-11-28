// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="OutputGenerate.cs">
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
	using System.IO;
	using System.Linq;
	using Library;

	public static partial class Program
	{
		private static void OutputGenerate()
		{
			bool countIrregular = true;
			bool countRegular = true;

			bool countArEndings = true;
			bool countErEndings = true;
			bool countIrEndings = true;

			bool hasDefectiveFilter = false;

			List<string> defectiveVerbs = null;
			List<string> verbs = new List<string>();

			string path = string.Empty;

			string[] generateProgramModeOptions = new string[] { "--defective", "--ending", "--generate", "--inflection" };

			foreach (Option option in options)
			{
				if (!generateProgramModeOptions.Contains<string>(option.Name))
				{
					Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid option with the \"--generate\" option. See \"--help\" for further information.", option.Name));
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
									countArEndings = true;
									countErEndings = false;
									countIrEndings = false;

									break;
								case "er":
									countArEndings = false;
									countErEndings = true;
									countIrEndings = false;

									break;
								case "ir":
									countArEndings = false;
									countErEndings = false;
									countIrEndings = true;

									break;
								default:
									Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid argument for the \"--ending\" option. See \"--help\" for further information.", option[0]));
									Console.WriteLine();

									Environment.Exit(1);
									break;
							}
						}

						break;
					case "--generate":
						if (option.Count != 0)
						{
							path = option[0];
						}
						else
						{
							Console.WriteLine("The \"--generate\" option must be entered with an argument. See \"--help\" for further information.");
							Console.WriteLine();

							Environment.Exit(1);
						}

						break;
					case "--inflection":
						if (option.Count != 0)
						{
							switch (option[0])
							{
								case "irregular":
									countIrregular = true;
									countRegular = false;

									break;
								case "regular":
									countIrregular = false;
									countRegular = true;

									break;
								default:
									Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid argument for the \"--count\" option. See \"--help\" for further information.", option[0]));
									Console.WriteLine();

									Environment.Exit(1);
									break;
							}
						}

						break;
				}
			}

			if (countIrregular)
			{
				if (countArEndings)
				{
					verbs.AddRange(VerbManager.IrregularArVerbs);
				}

				if (countErEndings)
				{
					verbs.AddRange(VerbManager.IrregularErVerbs);
				}

				if (countIrEndings)
				{
					verbs.AddRange(VerbManager.IrregularIrVerbs);
				}
			}

			if (countRegular)
			{
				if (countArEndings)
				{
					verbs.AddRange(VerbManager.RegularArVerbs);
				}

				if (countErEndings)
				{
					verbs.AddRange(VerbManager.RegularErVerbs);
				}

				if (countIrEndings)
				{
					verbs.AddRange(VerbManager.RegularIrVerbs);
				}
			}

			verbs.Sort();

			if (!hasDefectiveFilter)
			{
				if (Directory.Exists(path))
				{
					try
					{
						Console.WriteLine("Generated verbs: " + VerbManager.GenerateImages(verbs, path).ToString(CultureInfo.InvariantCulture));
						Console.WriteLine("Location: " + path);
						Console.WriteLine();
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						Console.WriteLine();

						Environment.Exit(1);
					}
				}
				else
				{
					Console.WriteLine("The directory path entered with the \"--generate\" option must exist. See \"--help\" for further information.");
					Console.WriteLine();

					Environment.Exit(1);
				}
			}
			else
			{
				defectiveVerbs = new List<string>();

				foreach (string verb in verbs)
				{
					foreach (string defectiveVerb in VerbManager.DefectiveVerbs)
					{
						if (verb == defectiveVerb)
						{
							defectiveVerbs.Add(verb);
						}
					}
				}

				if (Directory.Exists(path))
				{
					try
					{
						Console.WriteLine("Generated verbs: " + VerbManager.GenerateImages(defectiveVerbs, path).ToString(CultureInfo.InvariantCulture));
						Console.WriteLine("Location: " + path);
						Console.WriteLine();
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						Console.WriteLine();

						Environment.Exit(1);
					}
				}
				else
				{
					Console.WriteLine("The directory path entered with the \"--generate\" option must exist. See \"--help\" for further information.");
					Console.WriteLine();

					Environment.Exit(1);
				}
			}
		}
	}
}