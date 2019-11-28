// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="VerbForm.cs">
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

namespace Emi.SpanishVerbConjugator.Library
{
	public class VerbForm
	{
		private string conjugation = string.Empty;

		public VerbForm(string conjugation, Inflection inflection)
		{
			this.conjugation = conjugation;
			this.Inflection = inflection;
		}

		public bool IsDefective { get; set; }

		public Inflection Inflection { get; set; } = Inflection.Undetermined;

		public override string ToString()
		{
			return this.conjugation;
		}
	}
}