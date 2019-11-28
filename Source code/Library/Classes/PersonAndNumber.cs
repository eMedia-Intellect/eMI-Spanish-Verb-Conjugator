// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="PersonAndNumber.cs">
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
	public class PersonAndNumber
	{
		public bool IsDefective { get; set; }

		public Inflection Inflection
		{
			get
			{
				Inflection inflection = Inflection.Undetermined;

				if (this.FirstPersonSingular.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.FirstPersonSingular.Inflection;
					}
				}

				if (this.FirstPersonPlural.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.FirstPersonPlural.Inflection;
					}
				}

				if (this.SecondPersonSingular.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.SecondPersonSingular.Inflection;
					}
				}

				if (this.SecondPersonPlural.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.SecondPersonPlural.Inflection;
					}
				}

				if (this.ThirdPersonSingular.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.ThirdPersonSingular.Inflection;
					}
				}

				if (this.ThirdPersonPlural.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.ThirdPersonPlural.Inflection;
					}
				}

				return inflection;
			}

			set
			{
				this.FirstPersonSingular.Inflection = value;
				this.FirstPersonPlural.Inflection = value;

				this.SecondPersonSingular.Inflection = value;
				this.SecondPersonPlural.Inflection = value;

				this.ThirdPersonSingular.Inflection = value;
				this.ThirdPersonPlural.Inflection = value;
			}
		}

		public VerbForm FirstPersonSingular { get; set; } = null;

		public VerbForm FirstPersonPlural { get; set; } = null;

		public VerbForm SecondPersonSingular { get; set; } = null;

		public VerbForm SecondPersonPlural { get; set; } = null;

		public VerbForm ThirdPersonSingular { get; set; } = null;

		public VerbForm ThirdPersonPlural { get; set; } = null;
	}
}