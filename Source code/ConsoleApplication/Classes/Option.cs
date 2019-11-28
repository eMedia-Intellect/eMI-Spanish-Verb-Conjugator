﻿// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="Option.cs">
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
	using System.Collections.Generic;
	using System.Globalization;

	public class Option
	{
		private List<string> arguments = new List<string>();

		public int Count
		{
			get
			{
				return this.arguments.Count;
			}
		}

		public string Name { get; set; }

		public string this[int index]
		{
			get
			{
				return this.arguments[index];
			}
		}

		public void Add(string argument)
		{
			this.arguments.Add(argument);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.Name, string.Join(" ", this.arguments)).Trim();
		}
	}
}