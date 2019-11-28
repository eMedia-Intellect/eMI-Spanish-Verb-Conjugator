// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="OutputLicence.cs">
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
		private static void OutputLicence()
		{
			string[] licenceProgramModeOptions = new string[] { "--licence", "--license" };

			foreach (Option option in options)
			{
				if (!licenceProgramModeOptions.Contains<string>(option.Name))
				{
					Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid option with the \"--licence\" option. See \"--help\" for further information.", option.Name));
					Console.WriteLine();

					Environment.Exit(1);
				}
			}

			Console.WriteLine("Spanish Verb Conjugator");
			Console.WriteLine("Copyright (c) 2018 eMedia Intellect.");
			Console.WriteLine("GNU General Public License version 3");
			Console.WriteLine();
		}
	}
}