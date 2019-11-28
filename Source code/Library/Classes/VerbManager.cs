// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="VerbManager.cs">
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
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Drawing.Text;
	using System.IO;
	using System.Linq;

	public static class VerbManager
	{
		public static IList<string> DefectiveVerbs
		{
			get
			{
				List<string> verbs = new List<string>();

				try
				{
					verbs.AddRange(File.ReadAllLines(@"Conjugations\Defective.txt"));
				}
				catch (Exception)
				{
					// The exception does not matter. An exception should not be thrown in a property.
				}

				verbs.Sort();

				return verbs;
			}
		}

		public static IList<string> IrregularArVerbs
		{
			get
			{
				List<string> verbs = new List<string>();

				try
				{
					IEnumerable<string> verbFiles = Directory.EnumerateFiles(@"Conjugations\Irregular\ar", "*.xml", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension);

					verbs = new List<string>(verbFiles);

					verbs.Sort();
				}
				catch (Exception)
				{
					// The exception does not matter. An exception should not be thrown in a property.
				}

				return verbs;
			}
		}

		public static IList<string> IrregularErVerbs
		{
			get
			{
				List<string> verbs = new List<string>();

				try
				{
					IEnumerable<string> verbFiles = Directory.EnumerateFiles(@"Conjugations\Irregular\er", "*.xml", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension);

					verbs = new List<string>(verbFiles);

					verbs.Sort();
				}
				catch (Exception)
				{
					// The exception does not matter. An exception should not be thrown in a property.
				}

				return verbs;
			}
		}

		public static IList<string> IrregularIrVerbs
		{
			get
			{
				List<string> verbs = new List<string>();

				try
				{
					IEnumerable<string> verbFiles = Directory.EnumerateFiles(@"Conjugations\Irregular\ir", "*.xml", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension);

					verbs = new List<string>(verbFiles);

					verbs.Sort();
				}
				catch (Exception)
				{
					// The exception does not matter. An exception should not be thrown in a property.
				}

				return verbs;
			}
		}

		public static IList<string> IrregularVerbs
		{
			get
			{
				List<string> verbs = new List<string>();

				verbs.AddRange(IrregularArVerbs);
				verbs.AddRange(IrregularErVerbs);
				verbs.AddRange(IrregularIrVerbs);

				verbs.Sort();

				return verbs;
			}
		}

		public static IList<string> RegularArVerbs
		{
			get
			{
				string[] arVerbs = new string[] { };

				try
				{
					arVerbs = File.ReadAllLines(@"Conjugations\Regular\ar.txt");
				}
				catch (Exception)
				{
					// The exception does not matter. An exception should not be thrown in a property.
				}

				List<string> verbs = new List<string>(arVerbs);

				verbs.Sort();

				return verbs;
			}
		}

		public static IList<string> RegularErVerbs
		{
			get
			{
				string[] erVerbs = new string[] { };

				try
				{
					erVerbs = File.ReadAllLines(@"Conjugations\Regular\er.txt");
				}
				catch (Exception)
				{
					// The exception does not matter. An exception should not be thrown in a property.
				}

				List<string> verbs = new List<string>(erVerbs);

				verbs.Sort();

				return verbs;
			}
		}

		public static IList<string> RegularIrVerbs
		{
			get
			{
				string[] irVerbs = new string[] { };

				try
				{
					irVerbs = File.ReadAllLines(@"Conjugations\Regular\ir.txt");
				}
				catch (Exception)
				{
					// The exception does not matter. An exception should not be thrown in a property.
				}

				List<string> verbs = new List<string>(irVerbs);

				verbs.Sort();

				return verbs;
			}
		}

		public static IList<string> RegularVerbs
		{
			get
			{
				List<string> verbs = new List<string>();

				verbs.AddRange(RegularArVerbs);
				verbs.AddRange(RegularErVerbs);
				verbs.AddRange(RegularIrVerbs);

				verbs.Sort();

				return verbs;
			}
		}

		public static IList<string> Verbs
		{
			get
			{
				List<string> verbs = new List<string>();

				verbs.AddRange(RegularVerbs);
				verbs.AddRange(IrregularVerbs);

				verbs.Sort();

				return verbs;
			}
		}

		public static int GenerateImages(IList<string> verbs, string path)
		{
			if (verbs == null)
			{
				throw new ArgumentNullException("verbs");
			}

			int verbCounter = 0;

			foreach (string verb in verbs)
			{
				using (Bitmap dummyBitmap = new Bitmap(1, 1))
				{
					int conjugationHeight = 0;
					int conjugationWidth = 0;

					using (Font font = new Font("Liberation Mono", 18F))
					{
						Graphics graphics = Graphics.FromImage(dummyBitmap);

						string conjugation = Conjugator.Conjugate(new Verb(verb)).ToString();

						conjugationWidth = (int)graphics.MeasureString(conjugation, font).Width;
						conjugationHeight = (int)graphics.MeasureString(conjugation, font).Height;

						using (Bitmap bitmap = new Bitmap(dummyBitmap, new Size(conjugationWidth, conjugationHeight)))
						{
							graphics = Graphics.FromImage(bitmap);

							graphics.Clear(Color.White);
							graphics.SmoothingMode = SmoothingMode.AntiAlias;
							graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

							using (SolidBrush solidBrush = new SolidBrush(Color.Black))
							{
								graphics.DrawString(conjugation, font, solidBrush, 0F, 0F);
							}

							graphics.Flush();

							try
							{
								bitmap.Save(Path.Combine(path, verb + ".jpg"));
							}
							catch (Exception)
							{
								throw new Exception("The software could not save the generated conjugation images. Make sure that the directory exists and that you have permission to create files inside it.");
							}
						}
					}
				}

				++verbCounter;
			}

			return verbCounter;
		}
	}
}