// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="Conjugation.cs">
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
	using System.Text;

	public class Conjugation
	{
		public Inflection Inflection
		{
			get
			{
				Inflection inflection = Inflection.Undetermined;

				if (this.Gerund.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.Gerund.Inflection;
					}
				}

				if (this.PastParticiple.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.PastParticiple.Inflection;
					}
				}

				if (this.PresentIndicative.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.PresentIndicative.Inflection;
					}
				}

				if (this.ImperfectIndicative.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.ImperfectIndicative.Inflection;
					}
				}

				if (this.PreteriteIndicative.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.PreteriteIndicative.Inflection;
					}
				}

				if (this.FutureIndicative.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.FutureIndicative.Inflection;
					}
				}

				if (this.PresentSubjunctive.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.PresentSubjunctive.Inflection;
					}
				}

				if (this.ImperfectRaSubjunctive.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.ImperfectRaSubjunctive.Inflection;
					}
				}

				if (this.ImperfectSeSubjunctive.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.ImperfectSeSubjunctive.Inflection;
					}
				}

				if (this.FutureSubjunctive.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.FutureSubjunctive.Inflection;
					}
				}

				if (this.AffirmativeImperative.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.AffirmativeImperative.Inflection;
					}
				}

				if (this.NegativeImperative.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.NegativeImperative.Inflection;
					}
				}

				if (this.Conditional.Inflection != Inflection.Undetermined)
				{
					if (inflection == Inflection.Regular || inflection == Inflection.Undetermined)
					{
						inflection = this.Conditional.Inflection;
					}
				}

				return inflection;
			}

			set
			{
				this.Gerund.Inflection = value;
				this.PastParticiple.Inflection = value;

				this.PresentIndicative.Inflection = value;
				this.ImperfectIndicative.Inflection = value;
				this.PreteriteIndicative.Inflection = value;
				this.FutureIndicative.Inflection = value;

				this.PresentSubjunctive.Inflection = value;
				this.ImperfectRaSubjunctive.Inflection = value;
				this.ImperfectSeSubjunctive.Inflection = value;
				this.FutureSubjunctive.Inflection = value;

				this.AffirmativeImperative.Inflection = value;
				this.NegativeImperative.Inflection = value;

				this.Conditional.Inflection = value;
			}
		}

		public PersonAndNumber PresentIndicative { get; set; } = new PersonAndNumber();

		public PersonAndNumber ImperfectIndicative { get; set; } = new PersonAndNumber();

		public PersonAndNumber PreteriteIndicative { get; set; } = new PersonAndNumber();

		public PersonAndNumber FutureIndicative { get; set; } = new PersonAndNumber();

		public PersonAndNumber PresentSubjunctive { get; set; } = new PersonAndNumber();

		public PersonAndNumber ImperfectRaSubjunctive { get; set; } = new PersonAndNumber();

		public PersonAndNumber ImperfectSeSubjunctive { get; set; } = new PersonAndNumber();

		public PersonAndNumber FutureSubjunctive { get; set; } = new PersonAndNumber();

		public PersonAndNumber AffirmativeImperative { get; set; } = new PersonAndNumber();

		public PersonAndNumber NegativeImperative { get; set; } = new PersonAndNumber();

		public PersonAndNumber Conditional { get; set; } = new PersonAndNumber();

		public string Infinitive { get; set; } = string.Empty;

		public VerbForm Gerund { get; set; } = null;

		public VerbForm PastParticiple { get; set; } = null;

		public override string ToString()
		{
			string infinitiveColumn = "Infinitive";

			string gerundColumn = "Gerund";

			if (this.Gerund.IsDefective)
			{
				gerundColumn += " (defective)";
			}

			string pastParticipleColumn = "Past participle";

			if (this.PastParticiple.IsDefective)
			{
				pastParticipleColumn += " (defective)";
			}

			string indicativeColumn = "Indicative";

			string presentIndicativeColumn = "Present";

			if (this.PresentIndicative.IsDefective)
			{
				presentIndicativeColumn += " (defective):";
			}
			else
			{
				presentIndicativeColumn += ":";
			}

			string imperfectIndicativeColumn = "Imperfect";

			if (this.ImperfectIndicative.IsDefective)
			{
				imperfectIndicativeColumn += " (defective):";
			}
			else
			{
				imperfectIndicativeColumn += ":";
			}

			string preteriteIndicativeColumn = "Preterite";

			if (this.PreteriteIndicative.IsDefective)
			{
				preteriteIndicativeColumn += " (defective):";
			}
			else
			{
				preteriteIndicativeColumn += ":";
			}

			string futureIndicativeColumn = "Future";

			if (this.FutureIndicative.IsDefective)
			{
				futureIndicativeColumn += " (defective):";
			}
			else
			{
				futureIndicativeColumn += ":";
			}

			string subjunctiveColumn = "Subjunctive";

			string presentSubjunctiveColumn = "Present";

			if (this.PresentSubjunctive.IsDefective)
			{
				presentSubjunctiveColumn += " (defective):";
			}
			else
			{
				presentSubjunctiveColumn += ":";
			}

			string imperfectRaSubjunctiveColumn = "Imperfect (ra)";

			if (this.ImperfectRaSubjunctive.IsDefective)
			{
				imperfectRaSubjunctiveColumn += " (defective):";
			}
			else
			{
				imperfectRaSubjunctiveColumn += ":";
			}

			string imperfectSeSubjunctiveColumn = "Imperfect (se)";

			if (this.ImperfectSeSubjunctive.IsDefective)
			{
				imperfectSeSubjunctiveColumn += " (defective):";
			}
			else
			{
				imperfectSeSubjunctiveColumn += ":";
			}

			string futureSubjunctiveColumn = "Future";

			if (this.FutureSubjunctive.IsDefective)
			{
				futureSubjunctiveColumn += " (defective):";
			}
			else
			{
				futureSubjunctiveColumn += ":";
			}

			string imperativeColumn = "Imperative";

			string affirmativeImperativeColumn = "Affirmative";

			if (this.AffirmativeImperative.IsDefective)
			{
				affirmativeImperativeColumn += " (defective):";
			}
			else
			{
				affirmativeImperativeColumn += ":";
			}

			string negativeImperativeColumn = "Negative";

			if (this.NegativeImperative.IsDefective)
			{
				negativeImperativeColumn += " (defective):";
			}
			else
			{
				negativeImperativeColumn += ":";
			}

			string conditionalColumn = "Conditional";

			if (this.Conditional.IsDefective)
			{
				conditionalColumn += " (defective)";
			}

			Table formsTable = new Table("Forms");

			formsTable.AddRow(infinitiveColumn, gerundColumn, pastParticipleColumn);
			formsTable.AddRow(this.Infinitive, this.Gerund.ToString(), this.PastParticiple.ToString());

			Table indicativeTable = new Table(indicativeColumn);

			indicativeTable.AddRow(string.Empty, "yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
			indicativeTable.AddRow(presentIndicativeColumn, this.PresentIndicative.FirstPersonSingular.ToString(), this.PresentIndicative.SecondPersonSingular.ToString(), this.PresentIndicative.ThirdPersonSingular.ToString(), this.PresentIndicative.FirstPersonPlural.ToString(), this.PresentIndicative.SecondPersonPlural.ToString(), this.PresentIndicative.ThirdPersonPlural.ToString());
			indicativeTable.AddRow(imperfectIndicativeColumn, this.ImperfectIndicative.FirstPersonSingular.ToString(), this.ImperfectIndicative.SecondPersonSingular.ToString(), this.ImperfectIndicative.ThirdPersonSingular.ToString(), this.ImperfectIndicative.FirstPersonPlural.ToString(), this.ImperfectIndicative.SecondPersonPlural.ToString(), this.ImperfectIndicative.ThirdPersonPlural.ToString());
			indicativeTable.AddRow(preteriteIndicativeColumn, this.PreteriteIndicative.FirstPersonSingular.ToString(), this.PreteriteIndicative.SecondPersonSingular.ToString(), this.PreteriteIndicative.ThirdPersonSingular.ToString(), this.PreteriteIndicative.FirstPersonPlural.ToString(), this.PreteriteIndicative.SecondPersonPlural.ToString(), this.PreteriteIndicative.ThirdPersonPlural.ToString());
			indicativeTable.AddRow(futureIndicativeColumn, this.FutureIndicative.FirstPersonSingular.ToString(), this.FutureIndicative.SecondPersonSingular.ToString(), this.FutureIndicative.ThirdPersonSingular.ToString(), this.FutureIndicative.FirstPersonPlural.ToString(), this.FutureIndicative.SecondPersonPlural.ToString(), this.FutureIndicative.ThirdPersonPlural.ToString());

			Table subjunctiveTable = new Table(subjunctiveColumn);

			subjunctiveTable.AddRow(string.Empty, "yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
			subjunctiveTable.AddRow(presentSubjunctiveColumn, this.PresentSubjunctive.FirstPersonSingular.ToString(), this.PresentSubjunctive.SecondPersonSingular.ToString(), this.PresentSubjunctive.ThirdPersonSingular.ToString(), this.PresentSubjunctive.FirstPersonPlural.ToString(), this.PresentSubjunctive.SecondPersonPlural.ToString(), this.PresentSubjunctive.ThirdPersonPlural.ToString());
			subjunctiveTable.AddRow(imperfectRaSubjunctiveColumn, this.ImperfectRaSubjunctive.FirstPersonSingular.ToString(), this.ImperfectRaSubjunctive.SecondPersonSingular.ToString(), this.ImperfectRaSubjunctive.ThirdPersonSingular.ToString(), this.ImperfectRaSubjunctive.FirstPersonPlural.ToString(), this.ImperfectRaSubjunctive.SecondPersonPlural.ToString(), this.ImperfectRaSubjunctive.ThirdPersonPlural.ToString());
			subjunctiveTable.AddRow(imperfectSeSubjunctiveColumn, this.ImperfectSeSubjunctive.FirstPersonSingular.ToString(), this.ImperfectSeSubjunctive.SecondPersonSingular.ToString(), this.ImperfectSeSubjunctive.ThirdPersonSingular.ToString(), this.ImperfectSeSubjunctive.FirstPersonPlural.ToString(), this.ImperfectSeSubjunctive.SecondPersonPlural.ToString(), this.ImperfectSeSubjunctive.ThirdPersonPlural.ToString());
			subjunctiveTable.AddRow(futureSubjunctiveColumn, this.FutureSubjunctive.FirstPersonSingular.ToString(), this.FutureSubjunctive.SecondPersonSingular.ToString(), this.FutureSubjunctive.ThirdPersonSingular.ToString(), this.FutureSubjunctive.FirstPersonPlural.ToString(), this.FutureSubjunctive.SecondPersonPlural.ToString(), this.FutureSubjunctive.ThirdPersonPlural.ToString());

			Table imperativeTable = new Table(imperativeColumn);

			imperativeTable.AddRow(string.Empty, "yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
			imperativeTable.AddRow(affirmativeImperativeColumn, string.Empty, this.AffirmativeImperative.SecondPersonSingular.ToString(), this.AffirmativeImperative.ThirdPersonSingular.ToString(), this.AffirmativeImperative.FirstPersonPlural.ToString(), this.AffirmativeImperative.SecondPersonPlural.ToString(), this.AffirmativeImperative.ThirdPersonPlural.ToString());
			imperativeTable.AddRow(negativeImperativeColumn, string.Empty, this.NegativeImperative.SecondPersonSingular.ToString(), this.NegativeImperative.ThirdPersonSingular.ToString(), this.NegativeImperative.FirstPersonPlural.ToString(), this.NegativeImperative.SecondPersonPlural.ToString(), this.NegativeImperative.ThirdPersonPlural.ToString());

			Table conditionalTable = new Table(conditionalColumn);

			conditionalTable.AddRow("yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
			conditionalTable.AddRow(this.Conditional.FirstPersonSingular.ToString(), this.Conditional.SecondPersonSingular.ToString(), this.Conditional.ThirdPersonSingular.ToString(), this.Conditional.FirstPersonPlural.ToString(), this.Conditional.SecondPersonPlural.ToString(), this.Conditional.ThirdPersonPlural.ToString());

			StringBuilder tablesStringBuilder = new StringBuilder();

			tablesStringBuilder.Append(formsTable);
			tablesStringBuilder.AppendLine();
			tablesStringBuilder.AppendLine();
			tablesStringBuilder.Append(indicativeTable);
			tablesStringBuilder.AppendLine();
			tablesStringBuilder.AppendLine();
			tablesStringBuilder.Append(subjunctiveTable);
			tablesStringBuilder.AppendLine();
			tablesStringBuilder.AppendLine();
			tablesStringBuilder.Append(imperativeTable);
			tablesStringBuilder.AppendLine();
			tablesStringBuilder.AppendLine();
			tablesStringBuilder.Append(conditionalTable);

			return tablesStringBuilder.ToString();
		}
	}
}