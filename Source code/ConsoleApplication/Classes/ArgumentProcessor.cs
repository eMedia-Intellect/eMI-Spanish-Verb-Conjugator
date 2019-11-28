// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="ArgumentProcessor.cs">
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
	using System.Linq;

	public static class ArgumentProcessor
	{
		public static bool AllowDuplicates { get; set; } = false;

		public static string[] Options { get; set; }

		public static IList<Option> Parse(string[] arguments)
		{
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}

			List<Option> options = new List<Option>();

			Option currentOption = null;

			for (int i = 0; i < arguments.Length; ++i)
			{
				if (arguments[i].StartsWith("-", StringComparison.Ordinal))
				{
					if (currentOption == null)
					{
						currentOption = new Option();
					}
					else
					{
						options.Add(currentOption);

						currentOption = new Option();
					}

					if (Options != null)
					{
						if (!Options.Contains<string>(arguments[i]))
						{
							InvalidArgumentException invalidArgumentException = new InvalidArgumentException("Invalid argument encountered: " + arguments[i]);

							invalidArgumentException.Argument = arguments[i];

							throw invalidArgumentException;
						}
					}

					if (!AllowDuplicates)
					{
						foreach (Option option in options)
						{
							if (option.Name == arguments[i])
							{
								DuplicateArgumentException duplicateArgumentException = new DuplicateArgumentException("Duplicated argument encountered: " + arguments[i]);

								duplicateArgumentException.Argument = arguments[i];

								throw duplicateArgumentException;
							}
						}
					}

					currentOption.Name = arguments[i];
				}
				else
				{
					if (currentOption == null)
					{
						currentOption = new Option();
					}

					currentOption.Add(arguments[i]);
				}

				if (i == arguments.Length - 1)
				{
					options.Add(currentOption);
				}
			}

			return options;
		}
	}
}