// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="Conjugator.cs">
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
	using System.IO;
	using System.Xml;

	public static class Conjugator
	{
		public static Conjugation Conjugate(Verb verb)
		{
			if (verb == null)
			{
				throw new ArgumentNullException("verb");
			}

			try
			{
				using (FileStream verbFileStream = new FileStream(string.Format(CultureInfo.InvariantCulture, "Conjugations\\Regular\\{0}.txt", verb.Ending), FileMode.Open, FileAccess.Read))
				{
					using (StreamReader verbStreamReader = new StreamReader(verbFileStream))
					{
						string verbInFile = verbStreamReader.ReadLine();

						while (verbInFile != null)
						{
							if (verb.Infinitive == verbInFile)
							{
								return ConjugateRegular(verb);
							}

							verbInFile = verbStreamReader.ReadLine();
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception("The software could not access or locate one or more of the verb files on the system.");
			}

			return ConjugateIrregular(verb);
		}

		private static Conjugation ConjugateIrregular(Verb verb)
		{
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(string.Format(CultureInfo.InvariantCulture, "Conjugations\\Irregular\\{0}\\{1}.xml", verb.Ending, verb.Infinitive)))
				{
					Conjugation conjugation = null;

					conjugation = ConjugateRegular(verb);

					string currentMood = null;
					string currentTense = null;
					string currentVariant = null;

					while (xmlReader.Read())
					{
						switch (xmlReader.Name)
						{
							case "form":
								switch (xmlReader["type"])
								{
									case "infinitive":
										conjugation.Infinitive = xmlReader.ReadElementContentAsString();

										break;
									case "gerund":
										if (xmlReader["defective"] == "true")
										{
											conjugation.Gerund.IsDefective = true;
										}
										else
										{
											conjugation.Gerund = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);
										}

										break;
									case "past-participle":
										if (xmlReader["defective"] == "true")
										{
											conjugation.PastParticiple.IsDefective = true;
										}
										else
										{
											conjugation.PastParticiple = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);
										}

										break;
								}

								break;
							case "mood":
								if (xmlReader.IsStartElement())
								{
									currentMood = xmlReader["type"];
								}

								break;
							case "tense":
								if (xmlReader.IsStartElement())
								{
									currentTense = xmlReader["type"];
									currentVariant = xmlReader["variant"];

									if (xmlReader["defective"] == "true")
									{
										switch (currentMood)
										{
											case "indicative":
												switch (currentTense)
												{
													case "present":
														conjugation.PresentIndicative.IsDefective = true;

														break;
													case "imperfect":
														conjugation.ImperfectIndicative.IsDefective = true;

														break;
													case "preterite":
														conjugation.PreteriteIndicative.IsDefective = true;

														break;
													case "future":
														conjugation.FutureIndicative.IsDefective = true;

														break;
												}

												break;
											case "subjunctive":
												switch (currentTense)
												{
													case "present":
														conjugation.PresentSubjunctive.IsDefective = true;

														break;
													case "imperfect":
														switch (currentVariant)
														{
															case "ra":
																conjugation.ImperfectRaSubjunctive.IsDefective = true;

																break;
															case "se":
																conjugation.ImperfectSeSubjunctive.IsDefective = true;

																break;
														}

														break;
													case "future":
														conjugation.FutureSubjunctive.IsDefective = true;

														break;
												}

												break;
											case "imperative":
												switch (currentVariant)
												{
													case "affirmative":
														conjugation.AffirmativeImperative.IsDefective = true;

														break;
													case "negative":
														conjugation.NegativeImperative.IsDefective = true;

														break;
												}

												break;
											case "conditional":
												conjugation.Conditional.IsDefective = true;

												break;
										}
									}
								}

								break;
							case "conjugation":
								switch (currentMood)
								{
									case "indicative":
										switch (currentTense)
										{
											case "present":
												switch (xmlReader["number"])
												{
													case "singular":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.PresentIndicative.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.PresentIndicative.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.PresentIndicative.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
													case "plural":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.PresentIndicative.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.PresentIndicative.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.PresentIndicative.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
												}

												break;
											case "imperfect":
												switch (xmlReader["number"])
												{
													case "singular":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.ImperfectIndicative.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.ImperfectIndicative.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.ImperfectIndicative.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
													case "plural":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.ImperfectIndicative.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.ImperfectIndicative.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.ImperfectIndicative.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
												}

												break;
											case "preterite":
												switch (xmlReader["number"])
												{
													case "singular":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.PreteriteIndicative.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.PreteriteIndicative.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.PreteriteIndicative.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
													case "plural":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.PreteriteIndicative.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.PreteriteIndicative.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.PreteriteIndicative.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
												}

												break;
											case "future":
												switch (xmlReader["number"])
												{
													case "singular":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.FutureIndicative.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.FutureIndicative.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.FutureIndicative.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
													case "plural":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.FutureIndicative.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.FutureIndicative.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.FutureIndicative.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
												}

												break;
										}

										break;
									case "subjunctive":
										switch (currentTense)
										{
											case "present":
												switch (xmlReader["number"])
												{
													case "singular":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.PresentSubjunctive.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.PresentSubjunctive.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.PresentSubjunctive.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
													case "plural":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.PresentSubjunctive.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.PresentSubjunctive.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.PresentSubjunctive.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
												}

												break;
											case "imperfect":
												switch (currentVariant)
												{
													case "ra":
														switch (xmlReader["number"])
														{
															case "singular":
																switch (xmlReader["person"])
																{
																	case "1":
																		conjugation.ImperfectRaSubjunctive.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																	case "2":
																		conjugation.ImperfectRaSubjunctive.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																	case "3":
																		conjugation.ImperfectRaSubjunctive.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																}

																break;
															case "plural":
																switch (xmlReader["person"])
																{
																	case "1":
																		conjugation.ImperfectRaSubjunctive.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																	case "2":
																		conjugation.ImperfectRaSubjunctive.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																	case "3":
																		conjugation.ImperfectRaSubjunctive.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																}

																break;
														}

														break;
													case "se":
														switch (xmlReader["number"])
														{
															case "singular":
																switch (xmlReader["person"])
																{
																	case "1":
																		conjugation.ImperfectSeSubjunctive.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																	case "2":
																		conjugation.ImperfectSeSubjunctive.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																	case "3":
																		conjugation.ImperfectSeSubjunctive.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																}

																break;
															case "plural":
																switch (xmlReader["person"])
																{
																	case "1":
																		conjugation.ImperfectSeSubjunctive.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																	case "2":
																		conjugation.ImperfectSeSubjunctive.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																	case "3":
																		conjugation.ImperfectSeSubjunctive.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																		break;
																}

																break;
														}

														break;
												}

												break;
											case "future":
												switch (xmlReader["number"])
												{
													case "singular":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.FutureSubjunctive.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.FutureSubjunctive.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.FutureSubjunctive.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
													case "plural":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.FutureSubjunctive.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.FutureSubjunctive.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.FutureSubjunctive.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
												}

												break;
										}

										break;
									case "imperative":
										switch (currentVariant)
										{
											case "affirmative":
												switch (xmlReader["number"])
												{
													case "singular":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.AffirmativeImperative.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.AffirmativeImperative.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.AffirmativeImperative.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
													case "plural":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.AffirmativeImperative.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.AffirmativeImperative.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.AffirmativeImperative.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
												}

												break;
											case "negative":
												switch (xmlReader["number"])
												{
													case "singular":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.NegativeImperative.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.NegativeImperative.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.NegativeImperative.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
													case "plural":
														switch (xmlReader["person"])
														{
															case "1":
																conjugation.NegativeImperative.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "2":
																conjugation.NegativeImperative.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
															case "3":
																conjugation.NegativeImperative.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

																break;
														}

														break;
												}

												break;
										}

										break;
									case "conditional":
										switch (xmlReader["number"])
										{
											case "singular":
												switch (xmlReader["person"])
												{
													case "1":
														conjugation.Conditional.FirstPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

														break;
													case "2":
														conjugation.Conditional.SecondPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

														break;
													case "3":
														conjugation.Conditional.ThirdPersonSingular = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

														break;
												}

												break;
											case "plural":
												switch (xmlReader["person"])
												{
													case "1":
														conjugation.Conditional.FirstPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

														break;
													case "2":
														conjugation.Conditional.SecondPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

														break;
													case "3":
														conjugation.Conditional.ThirdPersonPlural = new VerbForm(xmlReader.ReadElementContentAsString(), Inflection.Irregular);

														break;
												}

												break;
										}

										break;
								}

								break;
						}
					}

					return conjugation;
				}
			}
			catch (Exception)
			{
				throw new Exception("The software could not access or locate one or more of the verb files on the system. It is possible that the entered verb has not been added to the library.");
			}
		}

		private static Conjugation ConjugateRegular(Verb verb)
		{
			Conjugation conjugation = new Conjugation();

			conjugation.Infinitive = verb.Infinitive;

			if (verb.Ending == "ar")
			{
				conjugation.Gerund = new VerbForm(verb.Stem + "ando", Inflection.Regular);
				conjugation.PastParticiple = new VerbForm(verb.Stem + "ado", Inflection.Regular);

				conjugation.PresentIndicative.FirstPersonSingular = new VerbForm(verb.Stem + "o", Inflection.Regular);
				conjugation.PresentIndicative.SecondPersonSingular = new VerbForm(verb.Stem + "as", Inflection.Regular);
				conjugation.PresentIndicative.ThirdPersonSingular = new VerbForm(verb.Stem + "a", Inflection.Regular);
				conjugation.PresentIndicative.FirstPersonPlural = new VerbForm(verb.Stem + "amos", Inflection.Regular);
				conjugation.PresentIndicative.SecondPersonPlural = new VerbForm(verb.Stem + "áis", Inflection.Regular);
				conjugation.PresentIndicative.ThirdPersonPlural = new VerbForm(verb.Stem + "an", Inflection.Regular);

				conjugation.ImperfectIndicative.FirstPersonSingular = new VerbForm(verb.Stem + "aba", Inflection.Regular);
				conjugation.ImperfectIndicative.SecondPersonSingular = new VerbForm(verb.Stem + "abas", Inflection.Regular);
				conjugation.ImperfectIndicative.ThirdPersonSingular = new VerbForm(verb.Stem + "aba", Inflection.Regular);
				conjugation.ImperfectIndicative.FirstPersonPlural = new VerbForm(verb.Stem + "ábamos", Inflection.Regular);
				conjugation.ImperfectIndicative.SecondPersonPlural = new VerbForm(verb.Stem + "abais", Inflection.Regular);
				conjugation.ImperfectIndicative.ThirdPersonPlural = new VerbForm(verb.Stem + "aban", Inflection.Regular);

				conjugation.PreteriteIndicative.FirstPersonSingular = new VerbForm(verb.Stem + "é", Inflection.Regular);
				conjugation.PreteriteIndicative.SecondPersonSingular = new VerbForm(verb.Stem + "aste", Inflection.Regular);
				conjugation.PreteriteIndicative.ThirdPersonSingular = new VerbForm(verb.Stem + "ó", Inflection.Regular);
				conjugation.PreteriteIndicative.FirstPersonPlural = new VerbForm(verb.Stem + "amos", Inflection.Regular);
				conjugation.PreteriteIndicative.SecondPersonPlural = new VerbForm(verb.Stem + "asteis", Inflection.Regular);
				conjugation.PreteriteIndicative.ThirdPersonPlural = new VerbForm(verb.Stem + "aron", Inflection.Regular);

				conjugation.PresentSubjunctive.FirstPersonSingular = new VerbForm(verb.Stem + "e", Inflection.Regular);
				conjugation.PresentSubjunctive.SecondPersonSingular = new VerbForm(verb.Stem + "es", Inflection.Regular);
				conjugation.PresentSubjunctive.ThirdPersonSingular = new VerbForm(verb.Stem + "e", Inflection.Regular);
				conjugation.PresentSubjunctive.FirstPersonPlural = new VerbForm(verb.Stem + "emos", Inflection.Regular);
				conjugation.PresentSubjunctive.SecondPersonPlural = new VerbForm(verb.Stem + "éis", Inflection.Regular);
				conjugation.PresentSubjunctive.ThirdPersonPlural = new VerbForm(verb.Stem + "en", Inflection.Regular);

				conjugation.ImperfectRaSubjunctive.FirstPersonSingular = new VerbForm(verb.Infinitive + "a", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.SecondPersonSingular = new VerbForm(verb.Infinitive + "as", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.ThirdPersonSingular = new VerbForm(verb.Infinitive + "a", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.FirstPersonPlural = new VerbForm(verb.Infinitive + "amos", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.SecondPersonPlural = new VerbForm(verb.Infinitive + "ais", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.ThirdPersonPlural = new VerbForm(verb.Infinitive + "an", Inflection.Regular);

				conjugation.ImperfectSeSubjunctive.FirstPersonSingular = new VerbForm(verb.Stem + "ase", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.SecondPersonSingular = new VerbForm(verb.Stem + "ases", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.ThirdPersonSingular = new VerbForm(verb.Stem + "ase", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.FirstPersonPlural = new VerbForm(verb.Stem + "ásemos", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.SecondPersonPlural = new VerbForm(verb.Stem + "aseis", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.ThirdPersonPlural = new VerbForm(verb.Stem + "asen", Inflection.Regular);

				conjugation.FutureSubjunctive.FirstPersonSingular = new VerbForm(verb.Infinitive + "e", Inflection.Regular);
				conjugation.FutureSubjunctive.SecondPersonSingular = new VerbForm(verb.Infinitive + "es", Inflection.Regular);
				conjugation.FutureSubjunctive.ThirdPersonSingular = new VerbForm(verb.Infinitive + "e", Inflection.Regular);
				conjugation.FutureSubjunctive.FirstPersonPlural = new VerbForm(verb.Infinitive + "emos", Inflection.Regular);
				conjugation.FutureSubjunctive.SecondPersonPlural = new VerbForm(verb.Infinitive + "eis", Inflection.Regular);
				conjugation.FutureSubjunctive.ThirdPersonPlural = new VerbForm(verb.Infinitive + "en", Inflection.Regular);

				conjugation.AffirmativeImperative.SecondPersonSingular = new VerbForm(verb.Stem + "a", Inflection.Regular);
				conjugation.AffirmativeImperative.ThirdPersonSingular = new VerbForm(verb.Stem + "e", Inflection.Regular);
				conjugation.AffirmativeImperative.FirstPersonPlural = new VerbForm(verb.Stem + "emos", Inflection.Regular);
				conjugation.AffirmativeImperative.SecondPersonPlural = new VerbForm(verb.Stem + "ad", Inflection.Regular);
				conjugation.AffirmativeImperative.ThirdPersonPlural = new VerbForm(verb.Stem + "en", Inflection.Regular);

				conjugation.NegativeImperative.SecondPersonSingular = new VerbForm(verb.Stem + "es", Inflection.Regular);
				conjugation.NegativeImperative.ThirdPersonSingular = new VerbForm(verb.Stem + "e", Inflection.Regular);
				conjugation.NegativeImperative.FirstPersonPlural = new VerbForm(verb.Stem + "emos", Inflection.Regular);
				conjugation.NegativeImperative.SecondPersonPlural = new VerbForm(verb.Stem + "éis", Inflection.Regular);
				conjugation.NegativeImperative.ThirdPersonPlural = new VerbForm(verb.Stem + "en", Inflection.Regular);
			}
			else if (verb.Ending == "er" || verb.Ending == "ir")
			{
				conjugation.Gerund = new VerbForm(verb.Stem + "iendo", Inflection.Regular);
				conjugation.PastParticiple = new VerbForm(verb.Stem + "ido", Inflection.Regular);

				conjugation.PresentIndicative.FirstPersonSingular = new VerbForm(verb.Stem + "o", Inflection.Regular);
				conjugation.PresentIndicative.SecondPersonSingular = new VerbForm(verb.Stem + "es", Inflection.Regular);
				conjugation.PresentIndicative.ThirdPersonSingular = new VerbForm(verb.Stem + "e", Inflection.Regular);

				switch (verb.Ending)
				{
					case "er":
						conjugation.PresentIndicative.FirstPersonPlural = new VerbForm(verb.Stem + "emos", Inflection.Regular);
						conjugation.PresentIndicative.SecondPersonPlural = new VerbForm(verb.Stem + "éis", Inflection.Regular);

						break;
					case "ir":
						conjugation.PresentIndicative.FirstPersonPlural = new VerbForm(verb.Stem + "imos", Inflection.Regular);
						conjugation.PresentIndicative.SecondPersonPlural = new VerbForm(verb.Stem + "ís", Inflection.Regular);

						break;
				}

				conjugation.PresentIndicative.ThirdPersonPlural = new VerbForm(verb.Stem + "en", Inflection.Regular);

				conjugation.ImperfectIndicative.FirstPersonSingular = new VerbForm(verb.Stem + "ía", Inflection.Regular);
				conjugation.ImperfectIndicative.SecondPersonSingular = new VerbForm(verb.Stem + "ías", Inflection.Regular);
				conjugation.ImperfectIndicative.ThirdPersonSingular = new VerbForm(verb.Stem + "ía", Inflection.Regular);
				conjugation.ImperfectIndicative.FirstPersonPlural = new VerbForm(verb.Stem + "íamos", Inflection.Regular);
				conjugation.ImperfectIndicative.SecondPersonPlural = new VerbForm(verb.Stem + "íais", Inflection.Regular);
				conjugation.ImperfectIndicative.ThirdPersonPlural = new VerbForm(verb.Stem + "ían", Inflection.Regular);

				conjugation.PreteriteIndicative.FirstPersonSingular = new VerbForm(verb.Stem + "í", Inflection.Regular);
				conjugation.PreteriteIndicative.SecondPersonSingular = new VerbForm(verb.Stem + "iste", Inflection.Regular);
				conjugation.PreteriteIndicative.ThirdPersonSingular = new VerbForm(verb.Stem + "ió", Inflection.Regular);
				conjugation.PreteriteIndicative.FirstPersonPlural = new VerbForm(verb.Stem + "imos", Inflection.Regular);
				conjugation.PreteriteIndicative.SecondPersonPlural = new VerbForm(verb.Stem + "isteis", Inflection.Regular);
				conjugation.PreteriteIndicative.ThirdPersonPlural = new VerbForm(verb.Stem + "ieron", Inflection.Regular);

				conjugation.PresentSubjunctive.FirstPersonSingular = new VerbForm(verb.Stem + "a", Inflection.Regular);
				conjugation.PresentSubjunctive.SecondPersonSingular = new VerbForm(verb.Stem + "as", Inflection.Regular);
				conjugation.PresentSubjunctive.ThirdPersonSingular = new VerbForm(verb.Stem + "a", Inflection.Regular);
				conjugation.PresentSubjunctive.FirstPersonPlural = new VerbForm(verb.Stem + "amos", Inflection.Regular);
				conjugation.PresentSubjunctive.SecondPersonPlural = new VerbForm(verb.Stem + "áis", Inflection.Regular);
				conjugation.PresentSubjunctive.ThirdPersonPlural = new VerbForm(verb.Stem + "an", Inflection.Regular);

				conjugation.ImperfectRaSubjunctive.FirstPersonSingular = new VerbForm(verb.Stem + "iera", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.SecondPersonSingular = new VerbForm(verb.Stem + "ieras", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.ThirdPersonSingular = new VerbForm(verb.Stem + "iera", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.FirstPersonPlural = new VerbForm(verb.Stem + "iéramos", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.SecondPersonPlural = new VerbForm(verb.Stem + "ierais", Inflection.Regular);
				conjugation.ImperfectRaSubjunctive.ThirdPersonPlural = new VerbForm(verb.Stem + "ieran", Inflection.Regular);

				conjugation.ImperfectSeSubjunctive.FirstPersonSingular = new VerbForm(verb.Stem + "iese", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.SecondPersonSingular = new VerbForm(verb.Stem + "ieses", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.ThirdPersonSingular = new VerbForm(verb.Stem + "iese", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.FirstPersonPlural = new VerbForm(verb.Stem + "iésemos", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.SecondPersonPlural = new VerbForm(verb.Stem + "ieseis", Inflection.Regular);
				conjugation.ImperfectSeSubjunctive.ThirdPersonPlural = new VerbForm(verb.Stem + "iesen", Inflection.Regular);

				conjugation.FutureSubjunctive.FirstPersonSingular = new VerbForm(verb.Stem + "iere", Inflection.Regular);
				conjugation.FutureSubjunctive.SecondPersonSingular = new VerbForm(verb.Stem + "ieres", Inflection.Regular);
				conjugation.FutureSubjunctive.ThirdPersonSingular = new VerbForm(verb.Stem + "iere", Inflection.Regular);
				conjugation.FutureSubjunctive.FirstPersonPlural = new VerbForm(verb.Stem + "iéremos", Inflection.Regular);
				conjugation.FutureSubjunctive.SecondPersonPlural = new VerbForm(verb.Stem + "iereis", Inflection.Regular);
				conjugation.FutureSubjunctive.ThirdPersonPlural = new VerbForm(verb.Stem + "ieren", Inflection.Regular);

				conjugation.AffirmativeImperative.SecondPersonSingular = new VerbForm(verb.Stem + "e", Inflection.Regular);
				conjugation.AffirmativeImperative.ThirdPersonSingular = new VerbForm(verb.Stem + "a", Inflection.Regular);
				conjugation.AffirmativeImperative.FirstPersonPlural = new VerbForm(verb.Stem + "amos", Inflection.Regular);

				switch (verb.Ending)
				{
					case "er":
						conjugation.AffirmativeImperative.SecondPersonPlural = new VerbForm(verb.Stem + "ed", Inflection.Regular);

						break;
					case "ir":
						conjugation.AffirmativeImperative.SecondPersonPlural = new VerbForm(verb.Stem + "id", Inflection.Regular);

						break;
				}

				conjugation.AffirmativeImperative.ThirdPersonPlural = new VerbForm(verb.Stem + "an", Inflection.Regular);

				conjugation.NegativeImperative.SecondPersonSingular = new VerbForm(verb.Stem + "as", Inflection.Regular);
				conjugation.NegativeImperative.ThirdPersonSingular = new VerbForm(verb.Stem + "a", Inflection.Regular);
				conjugation.NegativeImperative.FirstPersonPlural = new VerbForm(verb.Stem + "amos", Inflection.Regular);
				conjugation.NegativeImperative.SecondPersonPlural = new VerbForm(verb.Stem + "áis", Inflection.Regular);
				conjugation.NegativeImperative.ThirdPersonPlural = new VerbForm(verb.Stem + "an", Inflection.Regular);
			}

			if (verb.Ending == "ar" || verb.Ending == "er" || verb.Ending == "ir")
			{
				conjugation.FutureIndicative.FirstPersonSingular = new VerbForm(verb.Infinitive + "é", Inflection.Regular);
				conjugation.FutureIndicative.SecondPersonSingular = new VerbForm(verb.Infinitive + "ás", Inflection.Regular);
				conjugation.FutureIndicative.ThirdPersonSingular = new VerbForm(verb.Infinitive + "á", Inflection.Regular);
				conjugation.FutureIndicative.FirstPersonPlural = new VerbForm(verb.Infinitive + "emos", Inflection.Regular);
				conjugation.FutureIndicative.SecondPersonPlural = new VerbForm(verb.Infinitive + "éis", Inflection.Regular);
				conjugation.FutureIndicative.ThirdPersonPlural = new VerbForm(verb.Infinitive + "án", Inflection.Regular);

				conjugation.Conditional.FirstPersonSingular = new VerbForm(verb.Infinitive + "ía", Inflection.Regular);
				conjugation.Conditional.SecondPersonSingular = new VerbForm(verb.Infinitive + "ías", Inflection.Regular);
				conjugation.Conditional.ThirdPersonSingular = new VerbForm(verb.Infinitive + "ía", Inflection.Regular);
				conjugation.Conditional.FirstPersonPlural = new VerbForm(verb.Infinitive + "íamos", Inflection.Regular);
				conjugation.Conditional.SecondPersonPlural = new VerbForm(verb.Infinitive + "íais", Inflection.Regular);
				conjugation.Conditional.ThirdPersonPlural = new VerbForm(verb.Infinitive + "ían", Inflection.Regular);
			}

			return conjugation;
		}
	}
}