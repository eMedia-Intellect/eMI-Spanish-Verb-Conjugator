// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="OutputConjugation.cs">
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
	using Library;

	public static partial class Program
	{
		private static void OutputConjugation()
		{
			string[] conjugationProgramModeOptions = new string[] { null, "--conditional", "--future-indicative", "--future-subjunctive", "--gerund", "--imperative", "--imperative-affirmative", "--imperative-negative", "--imperfect-indicative", "--imperfect-ra-subjunctive", "--imperfect-se-subjunctive", "--imperfect-subjunctive", "--indicative", "--infinitive", "--past-participle", "--present-indicative", "--present-subjunctive", "--preterite-indicative", "--raw", "--subjunctive", "--verb" };

			foreach (Option option in options)
			{
				if (!conjugationProgramModeOptions.Contains<string>(option.Name))
				{
					if (programMode == ProgramMode.Conjugation)
					{
						Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid option with the \"--verb\" option. See \"--help\" for further information.", option.Name));
					}
					else
					{
						Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not a valid option during conjugation. See \"--help\" for further information.", option.Name));
					}

					Console.WriteLine();

					Environment.Exit(1);
				}
			}

			Option unqualifiedOption = null;
			Option verbOption = null;

			foreach (Option option in options)
			{
				switch (option.Name)
				{
					case null:
						unqualifiedOption = option;

						break;
					case "--verb":
						verbOption = option;

						break;
				}
			}

			if (unqualifiedOption != null && verbOption != null)
			{
				Console.WriteLine("Both an unqualified argument and the \"--verb\" option were entered. Use one or the other. See \"--help\" for further information.");
				Console.WriteLine();

				Environment.Exit(1);
			}

			bool hasMultipleVerbs = false;

			string verb = null;

			if (unqualifiedOption != null)
			{
				if (unqualifiedOption.Count > 1)
				{
					hasMultipleVerbs = true;
				}
				else
				{
					verb = unqualifiedOption[0];
				}
			}

			if (verbOption != null)
			{
				if (verbOption.Count > 1)
				{
					hasMultipleVerbs = true;
				}
				else
				{
					if (verbOption.Count == 0)
					{
						Console.WriteLine("The \"--verb\" option must be entered with an argument. See \"--help\" for further information.");
						Console.WriteLine();

						Environment.Exit(1);
					}
					else
					{
						verb = verbOption[0];
					}
				}
			}

			if (hasMultipleVerbs)
			{
				Console.WriteLine("Multiple verbs were entered. Only one verb can be conjugated per execution. See \"--help\" for further information.");
				Console.WriteLine();

				Environment.Exit(1);
			}

			bool conjugateAll = true;
			bool conjugateConditional = false;
			bool conjugateFutureIndicative = false;
			bool conjugateFutureSubjunctive = false;
			bool conjugateGerund = false;
			bool conjugateImperativeAffirmative = false;
			bool conjugateImperativeNegative = false;
			bool conjugateImperfectIndicative = false;
			bool conjugateImperfectRaSubjunctive = false;
			bool conjugateImperfectSeSubjunctive = false;
			bool conjugateInfinitive = false;
			bool conjugatePastParticiple = false;
			bool conjugatePresentIndicative = false;
			bool conjugatePresentSubjunctive = false;
			bool conjugatePreteriteIndicative = false;

			bool isRaw = false;

			foreach (Option option in options)
			{
				switch (option.Name)
				{
					case "--conditional":
						conjugateAll = false;
						conjugateConditional = true;

						break;
					case "--future-indicative":
						conjugateAll = false;
						conjugateFutureIndicative = true;

						break;
					case "--future-subjunctive":
						conjugateAll = false;
						conjugateFutureSubjunctive = true;

						break;
					case "--gerund":
						conjugateAll = false;
						conjugateGerund = true;

						break;
					case "--imperative":
						conjugateAll = false;
						conjugateImperativeAffirmative = true;
						conjugateImperativeNegative = true;

						break;
					case "--imperative-affirmative":
						conjugateAll = false;
						conjugateImperativeAffirmative = true;

						break;
					case "--imperative-negative":
						conjugateAll = false;
						conjugateImperativeNegative = true;

						break;
					case "--imperfect-indicative":
						conjugateAll = false;
						conjugateImperfectIndicative = true;

						break;
					case "--imperfect-ra-subjunctive":
						conjugateAll = false;
						conjugateImperfectRaSubjunctive = true;

						break;
					case "--imperfect-se-subjunctive":
						conjugateAll = false;
						conjugateImperfectSeSubjunctive = true;

						break;
					case "--imperfect-subjunctive":
						conjugateAll = false;
						conjugateImperfectRaSubjunctive = true;
						conjugateImperfectSeSubjunctive = true;

						break;
					case "--indicative":
						conjugateAll = false;
						conjugateFutureIndicative = true;
						conjugateImperfectIndicative = true;
						conjugatePresentIndicative = true;
						conjugatePreteriteIndicative = true;

						break;
					case "--infinitive":
						conjugateAll = false;
						conjugateInfinitive = true;

						break;
					case "--past-participle":
						conjugateAll = false;
						conjugatePastParticiple = true;

						break;
					case "--present-indicative":
						conjugateAll = false;
						conjugatePresentIndicative = true;

						break;
					case "--present-subjunctive":
						conjugateAll = false;
						conjugatePresentSubjunctive = true;

						break;
					case "--preterite-indicative":
						conjugateAll = false;
						conjugatePreteriteIndicative = true;

						break;
					case "--raw":
						isRaw = true;

						break;
					case "--subjunctive":
						conjugateAll = false;
						conjugateFutureSubjunctive = true;
						conjugateImperfectRaSubjunctive = true;
						conjugateImperfectSeSubjunctive = true;
						conjugatePresentSubjunctive = true;

						break;
				}
			}

			if (verb == null && isRaw)
			{
				if ((conjugateInfinitive && conjugateGerund && conjugatePastParticiple) || conjugateAll)
				{
					Console.WriteLine("-ar,-er/-ir");
					Console.WriteLine("-ando,-iendo");
					Console.WriteLine("-ado,-ido");
					Console.WriteLine();
				}
				else if (conjugateInfinitive && conjugateGerund)
				{
					Console.WriteLine("-ar,-er/-ir");
					Console.WriteLine("-ando,-iendo");
					Console.WriteLine();
				}
				else if (conjugateInfinitive && conjugatePastParticiple)
				{
					Console.WriteLine("-ar,-er/-ir");
					Console.WriteLine("-ado,-ido");
					Console.WriteLine();
				}
				else if (conjugateGerund && conjugatePastParticiple)
				{
					Console.WriteLine("-ando,-iendo");
					Console.WriteLine("-ado,-ido");
					Console.WriteLine();
				}
				else if (conjugateInfinitive)
				{
					Console.WriteLine("-ar,-er/-ir");
					Console.WriteLine();
				}
				else if (conjugateGerund)
				{
					Console.WriteLine("-ando,-iendo");
					Console.WriteLine();
				}
				else if (conjugatePastParticiple)
				{
					Console.WriteLine("-ado,-ido");
					Console.WriteLine();
				}

				if (conjugatePresentIndicative || conjugateAll)
				{
					Console.WriteLine("-o,-o");
					Console.WriteLine("-as,-es");
					Console.WriteLine("-a,-e");
					Console.WriteLine("-amos,-emos/-imos");
					Console.WriteLine("-áis,-éis/-ís");
					Console.WriteLine("-an,-en");
					Console.WriteLine();
				}

				if (conjugateImperfectIndicative || conjugateAll)
				{
					Console.WriteLine("-aba,-ía");
					Console.WriteLine("-abas,-ías");
					Console.WriteLine("-aba,-ía");
					Console.WriteLine("-ábamos,-íamos");
					Console.WriteLine("-abais,-íais");
					Console.WriteLine("-aban,-ían");
					Console.WriteLine();
				}

				if (conjugatePreteriteIndicative || conjugateAll)
				{
					Console.WriteLine("-é,-í");
					Console.WriteLine("-aste,-iste");
					Console.WriteLine("-ó,-ió");
					Console.WriteLine("-amos,-imos");
					Console.WriteLine("-asteis,-isteis");
					Console.WriteLine("-aron,-ieron");
					Console.WriteLine();
				}

				if (conjugateFutureIndicative || conjugateAll)
				{
					Console.WriteLine("infinitive + é");
					Console.WriteLine("infinitive + ás");
					Console.WriteLine("infinitive + á");
					Console.WriteLine("infinitive + emos");
					Console.WriteLine("infinitive + éis");
					Console.WriteLine("infinitive + án");
					Console.WriteLine();
				}

				if (conjugatePresentSubjunctive || conjugateAll)
				{
					Console.WriteLine("-e,-a");
					Console.WriteLine("-es,-as");
					Console.WriteLine("-e,-a");
					Console.WriteLine("-emos,-amos");
					Console.WriteLine("-éis,-áis");
					Console.WriteLine("-en,-an");
					Console.WriteLine();
				}

				if (conjugateImperfectRaSubjunctive || conjugateAll)
				{
					Console.WriteLine("infinitive + a,-iera");
					Console.WriteLine("infinitive + as,-ieras");
					Console.WriteLine("infinitive + a,-iera");
					Console.WriteLine("infinitive + amos,-iéramos");
					Console.WriteLine("infinitive + ais,-ierais");
					Console.WriteLine("infinitive + an,-ieran");
					Console.WriteLine();
				}

				if (conjugateImperfectSeSubjunctive || conjugateAll)
				{
					Console.WriteLine("-ase,-iese");
					Console.WriteLine("-ases,-ieses");
					Console.WriteLine("-ase,-iese");
					Console.WriteLine("-ásemos,-iésemos");
					Console.WriteLine("-aseis,-ieseis");
					Console.WriteLine("-asen,-iesen");
					Console.WriteLine();
				}

				if (conjugateFutureSubjunctive || conjugateAll)
				{
					Console.WriteLine("infinitive + e,-iere");
					Console.WriteLine("infinitive + es,-ieres");
					Console.WriteLine("infinitive + e,-iere");
					Console.WriteLine("infinitive + emos,-iéremos");
					Console.WriteLine("infinitive + eis,-iereis");
					Console.WriteLine("infinitive + en,-ieren");
					Console.WriteLine();
				}

				if (conjugateImperativeAffirmative || conjugateAll)
				{
					Console.WriteLine("-");
					Console.WriteLine("-a,-e");
					Console.WriteLine("-e,-a");
					Console.WriteLine("-emos,-amos");
					Console.WriteLine("-ad,-ed/-id");
					Console.WriteLine("-en,-an");
					Console.WriteLine();
				}

				if (conjugateImperativeNegative || conjugateAll)
				{
					Console.WriteLine("-");
					Console.WriteLine("-es,-as");
					Console.WriteLine("-e,-a");
					Console.WriteLine("-emos,-amos");
					Console.WriteLine("-éis,-áis");
					Console.WriteLine("-en,-an");
					Console.WriteLine();
				}

				if (conjugateConditional || conjugateAll)
				{
					Console.WriteLine("infinitive + ía");
					Console.WriteLine("infinitive + ías");
					Console.WriteLine("infinitive + ía");
					Console.WriteLine("infinitive + íamos");
					Console.WriteLine("infinitive + íais");
					Console.WriteLine("infinitive + ían");
					Console.WriteLine();
				}
			}
			else if (verb == null && !isRaw)
			{
				Table formsTable = new Table("Forms");

				if ((conjugateInfinitive && conjugateGerund && conjugatePastParticiple) || conjugateAll)
				{
					formsTable.AddRow("Infinitive", "Gerund", "Past participle");
					formsTable.AddRow("-ar     -er/-ir", "-ando    -iendo", "-ado       -ido");
				}
				else if (conjugateInfinitive && conjugateGerund)
				{
					formsTable.AddRow("Infinitive", "Gerund");
					formsTable.AddRow("-ar    -er/-ir", "-ando    -iendo");
				}
				else if (conjugateInfinitive && conjugatePastParticiple)
				{
					formsTable.AddRow("Infinitive", "Past participle");
					formsTable.AddRow("-ar    -er/-ir", "-ado    -ido");
				}
				else if (conjugateGerund && conjugatePastParticiple)
				{
					formsTable.AddRow("Gerund", "Past participle");
					formsTable.AddRow("-ando    -iendo", "-ado    -ido");
				}
				else if (conjugateInfinitive)
				{
					formsTable.AddRow("Infinitive");
					formsTable.AddRow("-ar    -er/-ir");
				}
				else if (conjugateGerund)
				{
					formsTable.AddRow("Gerund");
					formsTable.AddRow("-ando    -iendo");
				}
				else if (conjugatePastParticiple)
				{
					formsTable.AddRow("Past participle");
					formsTable.AddRow("-ado    -ido");
				}

				Table indicativeTable = null;

				if (conjugatePresentIndicative || conjugateImperfectIndicative || conjugatePreteriteIndicative || conjugateFutureIndicative || conjugateAll)
				{
					indicativeTable = new Table("Indicative");

					indicativeTable.AddRow(string.Empty, "yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
				}

				if (conjugatePresentIndicative || conjugateAll)
				{
					indicativeTable.AddRow("Present:", "-o         -o", "-as        -es", "-a         -e", "-amos      -emos/-imos", "-áis       -éis/-ís", "-an        -en");
				}

				if (conjugateImperfectIndicative || conjugateAll)
				{
					indicativeTable.AddRow("Imperfect:", "-aba       -ía", "-abas      -ías", "-aba       -ía", "-ábamos    -íamos", "-abais     -íais", "-aban      -ían");
				}

				if (conjugatePreteriteIndicative || conjugateAll)
				{
					indicativeTable.AddRow("Preterite:", "-é         -í", "-aste      -iste", "-ó         -ió", "-amos      -imos", "-asteis    -isteis", "-aron      -ieron");
				}

				if (conjugateFutureIndicative || conjugateAll)
				{
					indicativeTable.AddRow("Future:", "infinitive + é", "infinitive + ás", "infinitive + á", "infinitive + emos", "infinitive + éis", "infinitive + án");
				}

				Table subjunctiveTable = null;

				if (conjugatePresentSubjunctive || conjugateImperfectRaSubjunctive || conjugateImperfectSeSubjunctive || conjugateFutureSubjunctive || conjugateAll)
				{
					subjunctiveTable = new Table("Subjunctive");

					subjunctiveTable.AddRow(string.Empty, "yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
				}

				if (conjugatePresentSubjunctive || conjugateAll)
				{
					subjunctiveTable.AddRow("Present:", "-e                   -a", "-es                  -as", "-e                   -a", "-emos                 -amos", "-éis                 -áis", "-en                  -an");
				}

				if (conjugateImperfectRaSubjunctive || conjugateAll)
				{
					subjunctiveTable.AddRow("Imperfect (ra):", "infinitive + a       -iera", "infinitive + as      -ieras", "infinitive + a       -iera", "infinitive + amos     -iéramos", "infinitive + ais     -ierais", "infinitive + an      -ieran");
				}

				if (conjugateImperfectSeSubjunctive || conjugateAll)
				{
					subjunctiveTable.AddRow("Imperfect (se):", "-ase                 -iese", "-ases                -ieses", "-ase                 -iese", "-ásemos               -iésemos", "-aseis               -ieseis", "-asen                -iesen");
				}

				if (conjugateFutureSubjunctive || conjugateAll)
				{
					subjunctiveTable.AddRow("Future:", "infinitive + e       -iere", "infinitive + es      -ieres", "infinitive + e       -iere", "infinitive + emos     -iéremos", "infinitive + eis     -iereis", "infinitive + en      -ieren");
				}

				Table imperativeTable = null;

				if (conjugateImperativeAffirmative || conjugateImperativeNegative || conjugateAll)
				{
					imperativeTable = new Table("Imperative");

					imperativeTable.AddRow(string.Empty, "yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
				}

				if (conjugateImperativeAffirmative || conjugateAll)
				{
					imperativeTable.AddRow("Affirmative", string.Empty, "-a      -e", "-e      -a", "-emos   -amos", "-ad     -ed/-id", "-en     -an");
				}

				if (conjugateImperativeNegative || conjugateAll)
				{
					imperativeTable.AddRow("Negative", string.Empty, "-es     -as", "-e      -a", "-emos   -amos", "-éis    -áis", "-en     -an");
				}

				Table conditionalTable = null;

				if (conjugateConditional || conjugateAll)
				{
					conditionalTable = new Table("Conditional");

					conditionalTable.AddRow("yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
					conditionalTable.AddRow("infinitive + ía", "infinitive + ías", "infinitive + ía", "infinitive + íamos", "infinitive + íais", "infinitive + ían");
				}

				if (conjugateInfinitive || conjugateGerund || conjugatePastParticiple || conjugateAll)
				{
					Console.WriteLine(formsTable);
					Console.WriteLine();
				}

				if (conjugatePresentIndicative || conjugateImperfectIndicative || conjugatePreteriteIndicative || conjugateFutureIndicative || conjugateAll)
				{
					Console.WriteLine(indicativeTable);
					Console.WriteLine();
				}

				if (conjugatePresentSubjunctive || conjugateImperfectRaSubjunctive || conjugateImperfectSeSubjunctive || conjugateFutureSubjunctive || conjugateAll)
				{
					Console.WriteLine(subjunctiveTable);
					Console.WriteLine();
				}

				if (conjugateImperativeAffirmative || conjugateImperativeNegative || conjugateAll)
				{
					Console.WriteLine(imperativeTable);
					Console.WriteLine();
				}

				if (conjugateConditional || conjugateAll)
				{
					Console.WriteLine(conditionalTable);
					Console.WriteLine();
				}
			}
			else if (isRaw)
			{
				try
				{
					Conjugation conjugation = Conjugator.Conjugate(new Verb(verb));

					if ((conjugateInfinitive && conjugateGerund && conjugatePastParticiple) || conjugateAll)
					{
						Console.WriteLine(conjugation.Infinitive);
						Console.WriteLine(conjugation.Gerund.ToString());
						Console.WriteLine(conjugation.PastParticiple.ToString());
						Console.WriteLine();
					}
					else if (conjugateInfinitive && conjugateGerund)
					{
						Console.WriteLine(conjugation.Infinitive);
						Console.WriteLine(conjugation.Gerund.ToString());
						Console.WriteLine();
					}
					else if (conjugateInfinitive && conjugatePastParticiple)
					{
						Console.WriteLine(conjugation.Infinitive);
						Console.WriteLine(conjugation.PastParticiple.ToString());
						Console.WriteLine();
					}
					else if (conjugateGerund && conjugatePastParticiple)
					{
						Console.WriteLine(conjugation.Gerund.ToString());
						Console.WriteLine(conjugation.PastParticiple.ToString());
						Console.WriteLine();
					}
					else if (conjugateInfinitive)
					{
						Console.WriteLine(conjugation.Infinitive);
						Console.WriteLine();
					}
					else if (conjugateGerund)
					{
						Console.WriteLine(conjugation.Gerund.ToString());
						Console.WriteLine();
					}
					else if (conjugatePastParticiple)
					{
						Console.WriteLine(conjugation.PastParticiple.ToString());
						Console.WriteLine();
					}

					if (conjugatePresentIndicative || conjugateAll)
					{
						Console.WriteLine(conjugation.PresentIndicative.FirstPersonSingular.ToString());
						Console.WriteLine(conjugation.PresentIndicative.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.PresentIndicative.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.PresentIndicative.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.PresentIndicative.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.PresentIndicative.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugateImperfectIndicative || conjugateAll)
					{
						Console.WriteLine(conjugation.ImperfectIndicative.FirstPersonSingular.ToString());
						Console.WriteLine(conjugation.ImperfectIndicative.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.ImperfectIndicative.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.ImperfectIndicative.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.ImperfectIndicative.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.ImperfectIndicative.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugatePreteriteIndicative || conjugateAll)
					{
						Console.WriteLine(conjugation.PreteriteIndicative.FirstPersonSingular.ToString());
						Console.WriteLine(conjugation.PreteriteIndicative.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.PreteriteIndicative.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.PreteriteIndicative.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.PreteriteIndicative.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.PreteriteIndicative.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugateFutureIndicative || conjugateAll)
					{
						Console.WriteLine(conjugation.FutureIndicative.FirstPersonSingular.ToString());
						Console.WriteLine(conjugation.FutureIndicative.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.FutureIndicative.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.FutureIndicative.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.FutureIndicative.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.FutureIndicative.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugatePresentSubjunctive || conjugateAll)
					{
						Console.WriteLine(conjugation.PresentSubjunctive.FirstPersonSingular.ToString());
						Console.WriteLine(conjugation.PresentSubjunctive.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.PresentSubjunctive.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.PresentSubjunctive.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.PresentSubjunctive.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.PresentSubjunctive.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugateImperfectRaSubjunctive || conjugateAll)
					{
						Console.WriteLine(conjugation.ImperfectRaSubjunctive.FirstPersonSingular.ToString());
						Console.WriteLine(conjugation.ImperfectRaSubjunctive.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.ImperfectRaSubjunctive.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.ImperfectRaSubjunctive.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.ImperfectRaSubjunctive.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.ImperfectRaSubjunctive.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugateImperfectSeSubjunctive || conjugateAll)
					{
						Console.WriteLine(conjugation.ImperfectSeSubjunctive.FirstPersonSingular.ToString());
						Console.WriteLine(conjugation.ImperfectSeSubjunctive.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.ImperfectSeSubjunctive.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.ImperfectSeSubjunctive.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.ImperfectSeSubjunctive.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.ImperfectSeSubjunctive.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugateFutureSubjunctive || conjugateAll)
					{
						Console.WriteLine(conjugation.FutureSubjunctive.FirstPersonSingular.ToString());
						Console.WriteLine(conjugation.FutureSubjunctive.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.FutureSubjunctive.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.FutureSubjunctive.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.FutureSubjunctive.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.FutureSubjunctive.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugateImperativeAffirmative || conjugateAll)
					{
						Console.WriteLine("-");
						Console.WriteLine(conjugation.AffirmativeImperative.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.AffirmativeImperative.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.AffirmativeImperative.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.AffirmativeImperative.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.AffirmativeImperative.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugateImperativeNegative || conjugateAll)
					{
						Console.WriteLine("-");
						Console.WriteLine(conjugation.NegativeImperative.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.NegativeImperative.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.NegativeImperative.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.NegativeImperative.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.NegativeImperative.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}

					if (conjugateConditional || conjugateAll)
					{
						Console.WriteLine(conjugation.Conditional.FirstPersonSingular.ToString());
						Console.WriteLine(conjugation.Conditional.SecondPersonSingular.ToString());
						Console.WriteLine(conjugation.Conditional.ThirdPersonSingular.ToString());
						Console.WriteLine(conjugation.Conditional.FirstPersonPlural.ToString());
						Console.WriteLine(conjugation.Conditional.SecondPersonPlural.ToString());
						Console.WriteLine(conjugation.Conditional.ThirdPersonPlural.ToString());
						Console.WriteLine();
					}
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
				try
				{
					Conjugation conjugation = Conjugator.Conjugate(new Verb(verb));

					Table formsTable = new Table("Forms");

					if ((conjugateInfinitive && conjugateGerund && conjugatePastParticiple) || conjugateAll)
					{
						formsTable.AddRow("Infinitive", "Gerund", "Past participle");
						formsTable.AddRow(conjugation.Infinitive, conjugation.Gerund.ToString(), conjugation.PastParticiple.ToString());
					}
					else if (conjugateInfinitive && conjugateGerund)
					{
						formsTable.AddRow("Infinitive", "Gerund");
						formsTable.AddRow(conjugation.Infinitive, conjugation.Gerund.ToString());
					}
					else if (conjugateInfinitive && conjugatePastParticiple)
					{
						formsTable.AddRow("Infinitive", "Past participle");
						formsTable.AddRow(conjugation.Infinitive, conjugation.PastParticiple.ToString());
					}
					else if (conjugateGerund && conjugatePastParticiple)
					{
						formsTable.AddRow("Gerund", "Past participle");
						formsTable.AddRow(conjugation.Gerund.ToString(), conjugation.PastParticiple.ToString());
					}
					else if (conjugateInfinitive)
					{
						formsTable.AddRow("Infinitive");
						formsTable.AddRow(conjugation.Infinitive);
					}
					else if (conjugateGerund)
					{
						formsTable.AddRow("Gerund");
						formsTable.AddRow(conjugation.Gerund.ToString());
					}
					else if (conjugatePastParticiple)
					{
						formsTable.AddRow("Past participle");
						formsTable.AddRow(conjugation.PastParticiple.ToString());
					}

					Table indicativeTable = null;

					if (conjugatePresentIndicative || conjugateImperfectIndicative || conjugatePreteriteIndicative || conjugateFutureIndicative || conjugateAll)
					{
						indicativeTable = new Table("Indicative");

						indicativeTable.AddRow(string.Empty, "yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
					}

					if (conjugatePresentIndicative || conjugateAll)
					{
						indicativeTable.AddRow("Present:", conjugation.PresentIndicative.FirstPersonSingular.ToString(), conjugation.PresentIndicative.SecondPersonSingular.ToString(), conjugation.PresentIndicative.ThirdPersonSingular.ToString(), conjugation.PresentIndicative.FirstPersonPlural.ToString(), conjugation.PresentIndicative.SecondPersonPlural.ToString(), conjugation.PresentIndicative.ThirdPersonPlural.ToString());
					}

					if (conjugateImperfectIndicative || conjugateAll)
					{
						indicativeTable.AddRow("Imperfect:", conjugation.ImperfectIndicative.FirstPersonSingular.ToString(), conjugation.ImperfectIndicative.SecondPersonSingular.ToString(), conjugation.ImperfectIndicative.ThirdPersonSingular.ToString(), conjugation.ImperfectIndicative.FirstPersonPlural.ToString(), conjugation.ImperfectIndicative.SecondPersonPlural.ToString(), conjugation.ImperfectIndicative.ThirdPersonPlural.ToString());
					}

					if (conjugatePreteriteIndicative || conjugateAll)
					{
						indicativeTable.AddRow("Preterite:", conjugation.PreteriteIndicative.FirstPersonSingular.ToString(), conjugation.PreteriteIndicative.SecondPersonSingular.ToString(), conjugation.PreteriteIndicative.ThirdPersonSingular.ToString(), conjugation.PreteriteIndicative.FirstPersonPlural.ToString(), conjugation.PreteriteIndicative.SecondPersonPlural.ToString(), conjugation.PreteriteIndicative.ThirdPersonPlural.ToString());
					}

					if (conjugateFutureIndicative || conjugateAll)
					{
						indicativeTable.AddRow("Future:", conjugation.FutureIndicative.FirstPersonSingular.ToString(), conjugation.FutureIndicative.SecondPersonSingular.ToString(), conjugation.FutureIndicative.ThirdPersonSingular.ToString(), conjugation.FutureIndicative.FirstPersonPlural.ToString(), conjugation.FutureIndicative.SecondPersonPlural.ToString(), conjugation.FutureIndicative.ThirdPersonPlural.ToString());
					}

					Table subjunctiveTable = null;

					if (conjugatePresentSubjunctive || conjugateImperfectRaSubjunctive || conjugateImperfectSeSubjunctive || conjugateFutureSubjunctive || conjugateAll)
					{
						subjunctiveTable = new Table("Subjunctive");

						subjunctiveTable.AddRow(string.Empty, "yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
					}

					if (conjugatePresentSubjunctive || conjugateAll)
					{
						subjunctiveTable.AddRow("Present:", conjugation.PresentSubjunctive.FirstPersonSingular.ToString(), conjugation.PresentSubjunctive.SecondPersonSingular.ToString(), conjugation.PresentSubjunctive.ThirdPersonSingular.ToString(), conjugation.PresentSubjunctive.FirstPersonPlural.ToString(), conjugation.PresentSubjunctive.SecondPersonPlural.ToString(), conjugation.PresentSubjunctive.ThirdPersonPlural.ToString());
					}

					if (conjugateImperfectRaSubjunctive || conjugateAll)
					{
						subjunctiveTable.AddRow("Imperfect (ra):", conjugation.ImperfectRaSubjunctive.FirstPersonSingular.ToString(), conjugation.ImperfectRaSubjunctive.SecondPersonSingular.ToString(), conjugation.ImperfectRaSubjunctive.ThirdPersonSingular.ToString(), conjugation.ImperfectRaSubjunctive.FirstPersonPlural.ToString(), conjugation.ImperfectRaSubjunctive.SecondPersonPlural.ToString(), conjugation.ImperfectRaSubjunctive.ThirdPersonPlural.ToString());
					}

					if (conjugateImperfectSeSubjunctive || conjugateAll)
					{
						subjunctiveTable.AddRow("Imperfect (se):", conjugation.ImperfectSeSubjunctive.FirstPersonSingular.ToString(), conjugation.ImperfectSeSubjunctive.SecondPersonSingular.ToString(), conjugation.ImperfectSeSubjunctive.ThirdPersonSingular.ToString(), conjugation.ImperfectSeSubjunctive.FirstPersonPlural.ToString(), conjugation.ImperfectSeSubjunctive.SecondPersonPlural.ToString(), conjugation.ImperfectSeSubjunctive.ThirdPersonPlural.ToString());
					}

					if (conjugateFutureSubjunctive || conjugateAll)
					{
						subjunctiveTable.AddRow("Future:", conjugation.FutureSubjunctive.FirstPersonSingular.ToString(), conjugation.FutureSubjunctive.SecondPersonSingular.ToString(), conjugation.FutureSubjunctive.ThirdPersonSingular.ToString(), conjugation.FutureSubjunctive.FirstPersonPlural.ToString(), conjugation.FutureSubjunctive.SecondPersonPlural.ToString(), conjugation.FutureSubjunctive.ThirdPersonPlural.ToString());
					}

					Table imperativeTable = null;

					if (conjugateImperativeAffirmative || conjugateImperativeNegative || conjugateAll)
					{
						imperativeTable = new Table("Imperative");

						imperativeTable.AddRow(string.Empty, "yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
					}

					if (conjugateImperativeAffirmative || conjugateAll)
					{
						imperativeTable.AddRow("Affirmative", string.Empty, conjugation.AffirmativeImperative.SecondPersonSingular.ToString(), conjugation.AffirmativeImperative.ThirdPersonSingular.ToString(), conjugation.AffirmativeImperative.FirstPersonPlural.ToString(), conjugation.AffirmativeImperative.SecondPersonPlural.ToString(), conjugation.AffirmativeImperative.ThirdPersonPlural.ToString());
					}

					if (conjugateImperativeNegative || conjugateAll)
					{
						imperativeTable.AddRow("Negative", string.Empty, conjugation.NegativeImperative.SecondPersonSingular.ToString(), conjugation.NegativeImperative.ThirdPersonSingular.ToString(), conjugation.NegativeImperative.FirstPersonPlural.ToString(), conjugation.NegativeImperative.SecondPersonPlural.ToString(), conjugation.NegativeImperative.ThirdPersonPlural.ToString());
					}

					Table conditionalTable = null;

					if (conjugateConditional || conjugateAll)
					{
						conditionalTable = new Table("Conditional");

						conditionalTable.AddRow("yo", "tú", "él/ella/ello", "nosotros/-as", "vosotros/-as", "ellos/ellas");
						conditionalTable.AddRow(conjugation.Conditional.FirstPersonSingular.ToString(), conjugation.Conditional.SecondPersonSingular.ToString(), conjugation.Conditional.ThirdPersonSingular.ToString(), conjugation.Conditional.FirstPersonPlural.ToString(), conjugation.Conditional.SecondPersonPlural.ToString(), conjugation.Conditional.ThirdPersonPlural.ToString());
					}

					if (conjugateInfinitive || conjugateGerund || conjugatePastParticiple || conjugateAll)
					{
						Console.WriteLine(formsTable);
						Console.WriteLine();
					}

					if (conjugatePresentIndicative || conjugateImperfectIndicative || conjugatePreteriteIndicative || conjugateFutureIndicative || conjugateAll)
					{
						Console.WriteLine(indicativeTable);
						Console.WriteLine();
					}

					if (conjugatePresentSubjunctive || conjugateImperfectRaSubjunctive || conjugateImperfectSeSubjunctive || conjugateFutureSubjunctive || conjugateAll)
					{
						Console.WriteLine(subjunctiveTable);
						Console.WriteLine();
					}

					if (conjugateImperativeAffirmative || conjugateImperativeNegative || conjugateAll)
					{
						Console.WriteLine(imperativeTable);
						Console.WriteLine();
					}

					if (conjugateConditional || conjugateAll)
					{
						Console.WriteLine(conditionalTable);
						Console.WriteLine();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					Console.WriteLine();

					Environment.Exit(1);
				}
			}
		}
	}
}