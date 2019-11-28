// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="Verb.cs">
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
	using System;
	using System.Globalization;
	using System.Linq;

	public class Verb
	{
		public Verb(string verb)
		{
			if (verb == null)
			{
				throw new ArgumentNullException("verb");
			}

			string sanitisedVerb = verb.ToLower(new CultureInfo("es"));

			string alphabet = "abcdefghiíjklmnñopqrstuvwxyz";

			foreach (char letter in sanitisedVerb)
			{
				if (!alphabet.Contains(letter))
				{
					throw new Exception("The entered verb comprised one or more invalid letters. The Spanish alphabet comprises the following 27 letters: " + Environment.NewLine + "a b c d e f g h i j k l m n ñ o p q r s t u v w x y z");
				}
			}

			if (sanitisedVerb.Length == 1)
			{
				throw new Exception("The entered verb comprised fewer than two letters. The shortest verb in Spanish comprises two letters (ir).");
			}

			if (sanitisedVerb.Substring(sanitisedVerb.Length - 2) != "ar" && sanitisedVerb.Substring(sanitisedVerb.Length - 2) != "er" && sanitisedVerb.Substring(sanitisedVerb.Length - 2) != "ir" && sanitisedVerb.Substring(sanitisedVerb.Length - 2) != "ír")
			{
				throw new Exception("The entered verb did not end with -ar, -er nor -ir. All Spanish verbs end with one of those endings in the infinitive. The verb must be entered in the infinitive.");
			}

			this.Infinitive = sanitisedVerb;

			if (sanitisedVerb.Substring(verb.Length - 2) != "ír")
			{
				this.Ending = sanitisedVerb.Substring(verb.Length - 2);
			}
			else
			{
				this.Ending = "ir";
			}

			this.Stem = sanitisedVerb.Substring(0, verb.Length - 2);
		}

		public string Ending { get; }

		public string Infinitive { get; }

		public string Stem { get; }

		public override string ToString()
		{
			return this.Infinitive;
		}
	}
}