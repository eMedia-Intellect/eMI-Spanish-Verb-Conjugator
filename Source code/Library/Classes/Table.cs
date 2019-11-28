// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="Table.cs">
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
	using System.Collections.Generic;
	using System.Text;

	public class Table
	{
		private List<string[]> body = new List<string[]>();

		private string caption = null;

		public Table(string caption)
		{
			this.caption = caption;
		}

		public void AddRow(params string[] cells)
		{
			this.body.Add(cells);
		}

		public override string ToString()
		{
			StringBuilder tableStringBuilder = new StringBuilder();

			if (this.caption != null)
			{
				tableStringBuilder.AppendLine(this.caption);
			}

			int columnCount = 0;
			int longestColumn = 0;

			foreach (string[] row in this.body)
			{
				if (row.Length > columnCount)
				{
					columnCount = row.Length;
				}

				for (int i = 0; i < row.Length; ++i)
				{
					if (row[i].Length > longestColumn)
					{
						longestColumn = row[i].Length;
					}
				}
			}

			string columnSeparation = " │ ";
			string leftTableBorder = "│ ";
			string rightTableBorder = " │";

			for (int column = 0; column < columnCount; ++column)
			{
				if (column == 0 && columnCount != 1)
				{
					tableStringBuilder.Append("┌─" + string.Empty.PadLeft(longestColumn, '─') + "─┬─");
				}
				else if (column == 0)
				{
					tableStringBuilder.Append("┌─" + string.Empty.PadLeft(longestColumn, '─') + "─┐");
				}
				else if (column == columnCount - 1)
				{
					tableStringBuilder.Append(string.Empty.PadLeft(longestColumn, '─') + "─┐");
				}
				else
				{
					tableStringBuilder.Append(string.Empty.PadLeft(longestColumn, '─') + "─┬─");
				}
			}

			tableStringBuilder.AppendLine();

			for (int row = 0; row < this.body.Count; ++row)
			{
				for (int column = 0; column < this.body[row].Length; ++column)
				{
					if (column == 0)
					{
						tableStringBuilder.Append(leftTableBorder + this.body[row][column].PadRight(longestColumn) + columnSeparation);
					}
					else if (column == this.body[row].Length - 1)
					{
						tableStringBuilder.Append(this.body[row][column].PadRight(longestColumn) + rightTableBorder);
					}
					else
					{
						tableStringBuilder.Append(this.body[row][column].PadRight(longestColumn) + columnSeparation);
					}
				}

				tableStringBuilder.AppendLine();

				if (row != this.body.Count - 1)
				{
					for (int column = 0; column < columnCount; ++column)
					{
						if (column == 0 && columnCount != 1)
						{
							tableStringBuilder.Append("├─" + string.Empty.PadLeft(longestColumn, '─') + "─┼─");
						}
						else if (column == 0)
						{
							tableStringBuilder.Append("├─" + string.Empty.PadLeft(longestColumn, '─') + "─┤");
						}
						else if (column == columnCount - 1)
						{
							tableStringBuilder.Append(string.Empty.PadLeft(longestColumn, '─') + "─┤");
						}
						else
						{
							tableStringBuilder.Append(string.Empty.PadLeft(longestColumn, '─') + "─┼─");
						}
					}

					tableStringBuilder.AppendLine();
				}
				else
				{
					for (int column = 0; column < columnCount; ++column)
					{
						if (column == 0 && columnCount != 1)
						{
							tableStringBuilder.Append("└─" + string.Empty.PadLeft(longestColumn, '─') + "─┴─");
						}
						else if (column == 0)
						{
							tableStringBuilder.Append("└─" + string.Empty.PadLeft(longestColumn, '─') + "─┘");
						}
						else if (column == columnCount - 1)
						{
							tableStringBuilder.Append(string.Empty.PadLeft(longestColumn, '─') + "─┘");
						}
						else
						{
							tableStringBuilder.Append(string.Empty.PadLeft(longestColumn, '─') + "─┴─");
						}
					}
				}
			}

			return tableStringBuilder.ToString();
		}
	}
}