// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="MainWindow.xaml.cs">
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

namespace Emi.SpanishVerbConjugator.WindowsApplication
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Documents;
	using System.Windows.Media;
	using Library;
	using UserControls;

	using Table = System.Windows.Documents.Table;

	public partial class MainWindow : Window
	{
		private bool showIrregular = true;
		private bool showRegular = true;

		private bool showArEndings = true;
		private bool showErEndings = true;
		private bool showIrEndings = true;

		private bool hasDefectiveFilter = false;

		private bool isResetting = false;

		private List<string> verbsList = new List<string>();

		public MainWindow()
		{
			this.InitializeComponent();

			this.isResetting = true;

			this.PopulateVerbsListBox();

			this.isResetting = false;

			this.GenerateConjugationTable();

			this.Show();

			if (this.verbsListBox.Items.Count == 0)
			{
				this.verbsListBox.Visibility = Visibility.Collapsed;

				MessageBox.Show("The software could not access or locate one or more of the verb files on the system. Ensure that the verb files are in their proper location and that you have permission to access them, then restart the program.", "Error");
			}
		}

		private static async Task<int> GenerateImagesAsync(IList<string> verbs, string path)
		{
			return await Task.Run(() => VerbManager.GenerateImages(verbs, path));
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			BrowserSettings browserSetings = new BrowserSettings();

			browserSetings.MinimumFiles = 0;
			browserSetings.MaximumFiles = 0;
			browserSetings.MinimumDirectories = 1;
			browserSetings.MaximumDirectories = 1;

			FileSystemBrowserWindow fileSystemBrowserWindow = new FileSystemBrowserWindow(browserSetings);

			fileSystemBrowserWindow.Owner = this;

			if (fileSystemBrowserWindow.ShowDialog() ?? true)
			{
				this.PathTextBox.Text = fileSystemBrowserWindow.SelectedDirectories[0];
			}
		}

		private void DefectiveCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			this.hasDefectiveFilter = !this.hasDefectiveFilter;

			this.isResetting = true;

			this.PopulateVerbsListBox();

			this.isResetting = false;
		}

		private void EndingGroupRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;

			switch (radioButton.Content.ToString())
			{
				case "ar":
					this.showArEndings = true;
					this.showErEndings = false;
					this.showIrEndings = false;

					break;
				case "er":
					this.showArEndings = false;
					this.showErEndings = true;
					this.showIrEndings = false;

					break;
				case "ir":
					this.showArEndings = false;
					this.showErEndings = false;
					this.showIrEndings = true;

					break;
			}

			this.isResetting = true;

			this.PopulateVerbsListBox();

			this.isResetting = false;
		}

		private async void ExportButton_Click(object sender, RoutedEventArgs e)
		{
			int imageCount = 0;

			List<string> verbs = null;

			this.PathTextBox.IsEnabled = false;
			this.BrowseButton.IsEnabled = false;
			this.YesFilterRadioButton.IsEnabled = false;
			this.NoFilterRadioButton.IsEnabled = false;
			this.ExportButton.IsEnabled = false;

			if (this.YesFilterRadioButton.IsChecked == true)
			{
				verbs = this.verbsListBox.Items.OfType<string>().ToList();
			}
			else
			{
				verbs = new List<string>();

				verbs.AddRange(VerbManager.Verbs);
			}

			try
			{
				imageCount = await GenerateImagesAsync(verbs, this.PathTextBox.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error");
			}

			MessageBox.Show("Generated verbs: " + imageCount + Environment.NewLine + "Location: " + this.PathTextBox.Text, "Image generation");

			this.PathTextBox.IsEnabled = true;
			this.BrowseButton.IsEnabled = true;
			this.YesFilterRadioButton.IsEnabled = true;
			this.NoFilterRadioButton.IsEnabled = true;
			this.ExportButton.IsEnabled = true;
		}

		private void GenerateConjugationTable()
		{
			this.tableFlowDocument.Blocks.Clear();

			Thickness tableBorderThickness = new Thickness(1D);
			Thickness tableTableCellPaddingThickness = new Thickness(5D);

			if (this.verbsListBox.SelectedItem == null)
			{
				/*
				 * Forms
				 */

				Paragraph formsParagraph = new Paragraph();

				formsParagraph.Inlines.Add(new Bold(new Run("Forms")));

				this.tableFlowDocument.Blocks.Add(formsParagraph);

				Table formsTable = new Table();

				formsTable.BorderBrush = Brushes.Black;
				formsTable.BorderThickness = tableBorderThickness;
				formsTable.CellSpacing = 0D;

				TableRowGroup formsTableTableRowGroup = new TableRowGroup();

				/*
				 * Forms | Row: header
				 */

				TableRow formsHeaderTableRow = new TableRow();

				formsHeaderTableRow.Background = Brushes.LightGray;

				TableCell infinitiveTableCell = new TableCell();

				infinitiveTableCell.BorderBrush = Brushes.Black;
				infinitiveTableCell.BorderThickness = tableBorderThickness;
				infinitiveTableCell.ColumnSpan = 2;
				infinitiveTableCell.Padding = tableTableCellPaddingThickness;
				infinitiveTableCell.TextAlignment = TextAlignment.Center;

				Run infinitiveRun = new Run("Infinitive");

				infinitiveRun.FontSize = 12D;

				infinitiveTableCell.Blocks.Add(new Paragraph(new Bold(infinitiveRun)));

				formsHeaderTableRow.Cells.Add(infinitiveTableCell);

				TableCell gerundTableCell = new TableCell();

				gerundTableCell.BorderBrush = Brushes.Black;
				gerundTableCell.BorderThickness = tableBorderThickness;
				gerundTableCell.ColumnSpan = 2;
				gerundTableCell.Padding = tableTableCellPaddingThickness;
				gerundTableCell.TextAlignment = TextAlignment.Center;

				Run gerundRun = new Run("Gerund");

				gerundRun.FontSize = 12D;

				gerundTableCell.Blocks.Add(new Paragraph(new Bold(gerundRun)));

				formsHeaderTableRow.Cells.Add(gerundTableCell);

				TableCell pastParticipleTableCell = new TableCell();

				pastParticipleTableCell.BorderBrush = Brushes.Black;
				pastParticipleTableCell.BorderThickness = tableBorderThickness;
				pastParticipleTableCell.ColumnSpan = 2;
				pastParticipleTableCell.Padding = tableTableCellPaddingThickness;
				pastParticipleTableCell.TextAlignment = TextAlignment.Center;

				Run pastParticipleRun = new Run("Past participle");

				pastParticipleRun.FontSize = 12D;

				pastParticipleTableCell.Blocks.Add(new Paragraph(new Bold(pastParticipleRun)));

				formsHeaderTableRow.Cells.Add(pastParticipleTableCell);

				formsTableTableRowGroup.Rows.Add(formsHeaderTableRow);

				/*
				 * Forms | Row: forms
				 */

				TableRow formsFormsTableRow = new TableRow();

				TableCell forms1TableCell = new TableCell();

				forms1TableCell.BorderBrush = Brushes.Black;
				forms1TableCell.BorderThickness = tableBorderThickness;
				forms1TableCell.Padding = tableTableCellPaddingThickness;

				Run forms1Run = new Run("-ar");

				forms1Run.FontSize = 12D;

				forms1TableCell.Blocks.Add(new Paragraph(forms1Run));

				formsFormsTableRow.Cells.Add(forms1TableCell);

				TableCell forms2TableCell = new TableCell();

				forms2TableCell.BorderBrush = Brushes.Black;
				forms2TableCell.BorderThickness = tableBorderThickness;
				forms2TableCell.Padding = tableTableCellPaddingThickness;

				Run forms2Run = new Run("-er/-ir");

				forms2Run.FontSize = 12D;

				forms2TableCell.Blocks.Add(new Paragraph(forms2Run));

				formsFormsTableRow.Cells.Add(forms2TableCell);

				formsTableTableRowGroup.Rows.Add(formsFormsTableRow);

				TableCell forms3TableCell = new TableCell();

				forms3TableCell.BorderBrush = Brushes.Black;
				forms3TableCell.BorderThickness = tableBorderThickness;
				forms3TableCell.Padding = tableTableCellPaddingThickness;

				Run forms3Run = new Run("-ando");

				forms3Run.FontSize = 12D;

				forms3TableCell.Blocks.Add(new Paragraph(forms3Run));

				formsFormsTableRow.Cells.Add(forms3TableCell);

				TableCell forms4TableCell = new TableCell();

				forms4TableCell.BorderBrush = Brushes.Black;
				forms4TableCell.BorderThickness = tableBorderThickness;
				forms4TableCell.Padding = tableTableCellPaddingThickness;

				Run forms4Run = new Run("-iendo");

				forms4Run.FontSize = 12D;

				forms4TableCell.Blocks.Add(new Paragraph(forms4Run));

				formsFormsTableRow.Cells.Add(forms4TableCell);

				TableCell forms5TableCell = new TableCell();

				forms5TableCell.BorderBrush = Brushes.Black;
				forms5TableCell.BorderThickness = tableBorderThickness;
				forms5TableCell.Padding = tableTableCellPaddingThickness;

				Run forms5Run = new Run("-ado");

				forms5Run.FontSize = 12D;

				forms5TableCell.Blocks.Add(new Paragraph(forms5Run));

				formsFormsTableRow.Cells.Add(forms5TableCell);

				TableCell forms6TableCell = new TableCell();

				forms6TableCell.BorderBrush = Brushes.Black;
				forms6TableCell.BorderThickness = tableBorderThickness;
				forms6TableCell.Padding = tableTableCellPaddingThickness;

				Run forms6Run = new Run("-ido");

				forms6Run.FontSize = 12D;

				forms6TableCell.Blocks.Add(new Paragraph(forms6Run));

				formsFormsTableRow.Cells.Add(forms6TableCell);

				/*
				 * Forms | RowGroup to Table and Table to FlowDocument.
				 */

				formsTable.RowGroups.Add(formsTableTableRowGroup);

				this.tableFlowDocument.Blocks.Add(formsTable);

				/*
				 * Indicative
				 */

				Paragraph indicativeParagraph = new Paragraph();

				indicativeParagraph.Inlines.Add(new Bold(new Run("Indicative")));

				this.tableFlowDocument.Blocks.Add(indicativeParagraph);

				Table indicativeTable = new Table();

				indicativeTable.BorderBrush = Brushes.Black;
				indicativeTable.BorderThickness = tableBorderThickness;
				indicativeTable.CellSpacing = 0D;

				TableRowGroup indicativeTableTableRowGroup = new TableRowGroup();

				/*
				 * Indicative | Row: header
				 */

				TableRow indicativeHeaderTableRow = new TableRow();

				indicativeHeaderTableRow.Background = Brushes.LightGray;

				TableCell indicativeHeader1TableCell = new TableCell();

				indicativeHeader1TableCell.BorderBrush = Brushes.Black;
				indicativeHeader1TableCell.BorderThickness = tableBorderThickness;
				indicativeHeader1TableCell.Padding = tableTableCellPaddingThickness;
				indicativeHeader1TableCell.TextAlignment = TextAlignment.Center;

				Run indicativeHeader1Run = new Run(string.Empty);

				indicativeHeader1Run.FontSize = 12D;

				indicativeHeader1TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader1Run)));

				indicativeHeaderTableRow.Cells.Add(indicativeHeader1TableCell);

				TableCell indicativeHeader2TableCell = new TableCell();

				indicativeHeader2TableCell.BorderBrush = Brushes.Black;
				indicativeHeader2TableCell.BorderThickness = tableBorderThickness;
				indicativeHeader2TableCell.ColumnSpan = 2;
				indicativeHeader2TableCell.Padding = tableTableCellPaddingThickness;
				indicativeHeader2TableCell.TextAlignment = TextAlignment.Center;

				Run indicativeHeader2Run = new Run("yo");

				indicativeHeader2Run.FontSize = 12D;

				indicativeHeader2TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader2Run)));

				indicativeHeaderTableRow.Cells.Add(indicativeHeader2TableCell);

				TableCell indicativeHeader3TableCell = new TableCell();

				indicativeHeader3TableCell.BorderBrush = Brushes.Black;
				indicativeHeader3TableCell.BorderThickness = tableBorderThickness;
				indicativeHeader3TableCell.ColumnSpan = 2;
				indicativeHeader3TableCell.Padding = tableTableCellPaddingThickness;
				indicativeHeader3TableCell.TextAlignment = TextAlignment.Center;

				Run indicativeHeader3Run = new Run("tú");

				indicativeHeader3Run.FontSize = 12D;

				indicativeHeader3TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader3Run)));

				indicativeHeaderTableRow.Cells.Add(indicativeHeader3TableCell);

				TableCell indicativeHeader4TableCell = new TableCell();

				indicativeHeader4TableCell.BorderBrush = Brushes.Black;
				indicativeHeader4TableCell.BorderThickness = tableBorderThickness;
				indicativeHeader4TableCell.ColumnSpan = 2;
				indicativeHeader4TableCell.Padding = tableTableCellPaddingThickness;
				indicativeHeader4TableCell.TextAlignment = TextAlignment.Center;

				Run indicativeHeader4Run = new Run("él/ella/ello");

				indicativeHeader4Run.FontSize = 12D;

				indicativeHeader4TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader4Run)));

				indicativeHeaderTableRow.Cells.Add(indicativeHeader4TableCell);

				TableCell indicativeHeader5TableCell = new TableCell();

				indicativeHeader5TableCell.BorderBrush = Brushes.Black;
				indicativeHeader5TableCell.BorderThickness = tableBorderThickness;
				indicativeHeader5TableCell.ColumnSpan = 2;
				indicativeHeader5TableCell.Padding = tableTableCellPaddingThickness;
				indicativeHeader5TableCell.TextAlignment = TextAlignment.Center;

				Run indicativeHeader5Run = new Run("nosotros/nosotras");

				indicativeHeader5Run.FontSize = 12D;

				indicativeHeader5TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader5Run)));

				indicativeHeaderTableRow.Cells.Add(indicativeHeader5TableCell);

				TableCell indicativeHeader6TableCell = new TableCell();

				indicativeHeader6TableCell.BorderBrush = Brushes.Black;
				indicativeHeader6TableCell.BorderThickness = tableBorderThickness;
				indicativeHeader6TableCell.ColumnSpan = 2;
				indicativeHeader6TableCell.Padding = tableTableCellPaddingThickness;
				indicativeHeader6TableCell.TextAlignment = TextAlignment.Center;

				Run indicativeHeader6Run = new Run("vosotros/vosotras");

				indicativeHeader6Run.FontSize = 12D;

				indicativeHeader6TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader6Run)));

				indicativeHeaderTableRow.Cells.Add(indicativeHeader6TableCell);

				TableCell indicativeHeader7TableCell = new TableCell();

				indicativeHeader7TableCell.BorderBrush = Brushes.Black;
				indicativeHeader7TableCell.BorderThickness = tableBorderThickness;
				indicativeHeader7TableCell.ColumnSpan = 2;
				indicativeHeader7TableCell.Padding = tableTableCellPaddingThickness;
				indicativeHeader7TableCell.TextAlignment = TextAlignment.Center;

				Run indicativeHeader7Run = new Run("ellos/ellas");

				indicativeHeader7Run.FontSize = 12D;

				indicativeHeader7TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader7Run)));

				indicativeHeaderTableRow.Cells.Add(indicativeHeader7TableCell);

				indicativeTableTableRowGroup.Rows.Add(indicativeHeaderTableRow);

				/*
				 * Indicative | Row: present
				 */

				TableRow indicativePresentTableRow = new TableRow();

				TableCell indicativePresent1TableCell = new TableCell();

				indicativePresent1TableCell.Background = Brushes.LightGray;
				indicativePresent1TableCell.BorderBrush = Brushes.Black;
				indicativePresent1TableCell.BorderThickness = tableBorderThickness;
				indicativePresent1TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent1Run = new Run("Present:");

				indicativePresent1Run.FontSize = 12D;

				indicativePresent1TableCell.Blocks.Add(new Paragraph(new Bold(indicativePresent1Run)));

				indicativePresentTableRow.Cells.Add(indicativePresent1TableCell);

				TableCell indicativePresent2TableCell = new TableCell();

				indicativePresent2TableCell.BorderBrush = Brushes.Black;
				indicativePresent2TableCell.BorderThickness = tableBorderThickness;
				indicativePresent2TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent2Run = new Run("-o");

				indicativePresent2Run.FontSize = 12D;

				indicativePresent2TableCell.Blocks.Add(new Paragraph(indicativePresent2Run));

				indicativePresentTableRow.Cells.Add(indicativePresent2TableCell);

				TableCell indicativePresent3TableCell = new TableCell();

				indicativePresent3TableCell.BorderBrush = Brushes.Black;
				indicativePresent3TableCell.BorderThickness = tableBorderThickness;
				indicativePresent3TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent3Run = new Run("-o");

				indicativePresent3Run.FontSize = 12D;

				indicativePresent3TableCell.Blocks.Add(new Paragraph(indicativePresent3Run));

				indicativePresentTableRow.Cells.Add(indicativePresent3TableCell);

				TableCell indicativePresent4TableCell = new TableCell();

				indicativePresent4TableCell.BorderBrush = Brushes.Black;
				indicativePresent4TableCell.BorderThickness = tableBorderThickness;
				indicativePresent4TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent4Run = new Run("-as");

				indicativePresent4Run.FontSize = 12D;

				indicativePresent4TableCell.Blocks.Add(new Paragraph(indicativePresent4Run));

				indicativePresentTableRow.Cells.Add(indicativePresent4TableCell);

				TableCell indicativePresent5TableCell = new TableCell();

				indicativePresent5TableCell.BorderBrush = Brushes.Black;
				indicativePresent5TableCell.BorderThickness = tableBorderThickness;
				indicativePresent5TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent5Run = new Run("-es");

				indicativePresent5Run.FontSize = 12D;

				indicativePresent5TableCell.Blocks.Add(new Paragraph(indicativePresent5Run));

				indicativePresentTableRow.Cells.Add(indicativePresent5TableCell);

				TableCell indicativePresent6TableCell = new TableCell();

				indicativePresent6TableCell.BorderBrush = Brushes.Black;
				indicativePresent6TableCell.BorderThickness = tableBorderThickness;
				indicativePresent6TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent6Run = new Run("-a");

				indicativePresent6Run.FontSize = 12D;

				indicativePresent6TableCell.Blocks.Add(new Paragraph(indicativePresent6Run));

				indicativePresentTableRow.Cells.Add(indicativePresent6TableCell);

				TableCell indicativePresent7TableCell = new TableCell();

				indicativePresent7TableCell.BorderBrush = Brushes.Black;
				indicativePresent7TableCell.BorderThickness = tableBorderThickness;
				indicativePresent7TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent7Run = new Run("-e");

				indicativePresent7Run.FontSize = 12D;

				indicativePresent7TableCell.Blocks.Add(new Paragraph(indicativePresent7Run));

				indicativePresentTableRow.Cells.Add(indicativePresent7TableCell);

				TableCell indicativePresent8TableCell = new TableCell();

				indicativePresent8TableCell.BorderBrush = Brushes.Black;
				indicativePresent8TableCell.BorderThickness = tableBorderThickness;
				indicativePresent8TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent8Run = new Run("-amos");

				indicativePresent8Run.FontSize = 12D;

				indicativePresent8TableCell.Blocks.Add(new Paragraph(indicativePresent8Run));

				indicativePresentTableRow.Cells.Add(indicativePresent8TableCell);

				TableCell indicativePresent9TableCell = new TableCell();

				indicativePresent9TableCell.BorderBrush = Brushes.Black;
				indicativePresent9TableCell.BorderThickness = tableBorderThickness;
				indicativePresent9TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent9Run = new Run("-emos/-imos");

				indicativePresent9Run.FontSize = 12D;

				indicativePresent9TableCell.Blocks.Add(new Paragraph(indicativePresent9Run));

				indicativePresentTableRow.Cells.Add(indicativePresent9TableCell);

				TableCell indicativePresent10TableCell = new TableCell();

				indicativePresent10TableCell.BorderBrush = Brushes.Black;
				indicativePresent10TableCell.BorderThickness = tableBorderThickness;
				indicativePresent10TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent10Run = new Run("-áis");

				indicativePresent10Run.FontSize = 12D;

				indicativePresent10TableCell.Blocks.Add(new Paragraph(indicativePresent10Run));

				indicativePresentTableRow.Cells.Add(indicativePresent10TableCell);

				TableCell indicativePresent11TableCell = new TableCell();

				indicativePresent11TableCell.BorderBrush = Brushes.Black;
				indicativePresent11TableCell.BorderThickness = tableBorderThickness;
				indicativePresent11TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent11Run = new Run("-éis/-ís");

				indicativePresent11Run.FontSize = 12D;

				indicativePresent11TableCell.Blocks.Add(new Paragraph(indicativePresent11Run));

				indicativePresentTableRow.Cells.Add(indicativePresent11TableCell);

				TableCell indicativePresent12TableCell = new TableCell();

				indicativePresent12TableCell.BorderBrush = Brushes.Black;
				indicativePresent12TableCell.BorderThickness = tableBorderThickness;
				indicativePresent12TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent12Run = new Run("-an");

				indicativePresent12Run.FontSize = 12D;

				indicativePresent12TableCell.Blocks.Add(new Paragraph(indicativePresent12Run));

				indicativePresentTableRow.Cells.Add(indicativePresent12TableCell);

				TableCell indicativePresent13TableCell = new TableCell();

				indicativePresent13TableCell.BorderBrush = Brushes.Black;
				indicativePresent13TableCell.BorderThickness = tableBorderThickness;
				indicativePresent13TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePresent13Run = new Run("-en");

				indicativePresent13Run.FontSize = 12D;

				indicativePresent13TableCell.Blocks.Add(new Paragraph(indicativePresent13Run));

				indicativePresentTableRow.Cells.Add(indicativePresent13TableCell);

				indicativeTableTableRowGroup.Rows.Add(indicativePresentTableRow);

				/*
				 * Indicative | Row: imperfect
				 */

				TableRow indicativeImperfectTableRow = new TableRow();

				TableCell indicativeImperfect1TableCell = new TableCell();

				indicativeImperfect1TableCell.Background = Brushes.LightGray;
				indicativeImperfect1TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect1TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect1TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect1Run = new Run("Imperfect:");

				indicativeImperfect1Run.FontSize = 12D;

				indicativeImperfect1TableCell.Blocks.Add(new Paragraph(new Bold(indicativeImperfect1Run)));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect1TableCell);

				TableCell indicativeImperfect2TableCell = new TableCell();

				indicativeImperfect2TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect2TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect2TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect2Run = new Run("-aba");

				indicativeImperfect2Run.FontSize = 12D;

				indicativeImperfect2TableCell.Blocks.Add(new Paragraph(indicativeImperfect2Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect2TableCell);

				TableCell indicativeImperfect3TableCell = new TableCell();

				indicativeImperfect3TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect3TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect3TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect3Run = new Run("-ía");

				indicativeImperfect3Run.FontSize = 12D;

				indicativeImperfect3TableCell.Blocks.Add(new Paragraph(indicativeImperfect3Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect3TableCell);

				TableCell indicativeImperfect4TableCell = new TableCell();

				indicativeImperfect4TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect4TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect4TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect4Run = new Run("-abas");

				indicativeImperfect4Run.FontSize = 12D;

				indicativeImperfect4TableCell.Blocks.Add(new Paragraph(indicativeImperfect4Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect4TableCell);

				TableCell indicativeImperfect5TableCell = new TableCell();

				indicativeImperfect5TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect5TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect5TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect5Run = new Run("-ías");

				indicativeImperfect5Run.FontSize = 12D;

				indicativeImperfect5TableCell.Blocks.Add(new Paragraph(indicativeImperfect5Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect5TableCell);

				TableCell indicativeImperfect6TableCell = new TableCell();

				indicativeImperfect6TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect6TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect6TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect6Run = new Run("-aba");

				indicativeImperfect6Run.FontSize = 12D;

				indicativeImperfect6TableCell.Blocks.Add(new Paragraph(indicativeImperfect6Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect6TableCell);

				TableCell indicativeImperfect7TableCell = new TableCell();

				indicativeImperfect7TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect7TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect7TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect7Run = new Run("-ía");

				indicativeImperfect7Run.FontSize = 12D;

				indicativeImperfect7TableCell.Blocks.Add(new Paragraph(indicativeImperfect7Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect7TableCell);

				TableCell indicativeImperfect8TableCell = new TableCell();

				indicativeImperfect8TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect8TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect8TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect8Run = new Run("-ábamos");

				indicativeImperfect8Run.FontSize = 12D;

				indicativeImperfect8TableCell.Blocks.Add(new Paragraph(indicativeImperfect8Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect8TableCell);

				TableCell indicativeImperfect9TableCell = new TableCell();

				indicativeImperfect9TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect9TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect9TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect9Run = new Run("-íamos");

				indicativeImperfect9Run.FontSize = 12D;

				indicativeImperfect9TableCell.Blocks.Add(new Paragraph(indicativeImperfect9Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect9TableCell);

				TableCell indicativeImperfect10TableCell = new TableCell();

				indicativeImperfect10TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect10TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect10TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect10Run = new Run("-abais");

				indicativeImperfect10Run.FontSize = 12D;

				indicativeImperfect10TableCell.Blocks.Add(new Paragraph(indicativeImperfect10Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect10TableCell);

				TableCell indicativeImperfect11TableCell = new TableCell();

				indicativeImperfect11TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect11TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect11TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect11Run = new Run("-íais");

				indicativeImperfect11Run.FontSize = 12D;

				indicativeImperfect11TableCell.Blocks.Add(new Paragraph(indicativeImperfect11Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect11TableCell);

				TableCell indicativeImperfect12TableCell = new TableCell();

				indicativeImperfect12TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect12TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect12TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect12Run = new Run("-aban");

				indicativeImperfect12Run.FontSize = 12D;

				indicativeImperfect12TableCell.Blocks.Add(new Paragraph(indicativeImperfect12Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect12TableCell);

				TableCell indicativeImperfect13TableCell = new TableCell();

				indicativeImperfect13TableCell.BorderBrush = Brushes.Black;
				indicativeImperfect13TableCell.BorderThickness = tableBorderThickness;
				indicativeImperfect13TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeImperfect13Run = new Run("-ían");

				indicativeImperfect13Run.FontSize = 12D;

				indicativeImperfect13TableCell.Blocks.Add(new Paragraph(indicativeImperfect13Run));

				indicativeImperfectTableRow.Cells.Add(indicativeImperfect13TableCell);

				indicativeTableTableRowGroup.Rows.Add(indicativeImperfectTableRow);

				/*
				 * Indicative | Row: preterite
				 */

				TableRow indicativePreteriteTableRow = new TableRow();

				TableCell indicativePreterite1TableCell = new TableCell();

				indicativePreterite1TableCell.Background = Brushes.LightGray;
				indicativePreterite1TableCell.BorderBrush = Brushes.Black;
				indicativePreterite1TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite1TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite1Run = new Run("Preterite:");

				indicativePreterite1Run.FontSize = 12D;

				indicativePreterite1TableCell.Blocks.Add(new Paragraph(new Bold(indicativePreterite1Run)));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite1TableCell);

				TableCell indicativePreterite2TableCell = new TableCell();

				indicativePreterite2TableCell.BorderBrush = Brushes.Black;
				indicativePreterite2TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite2TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite2Run = new Run("-é");

				indicativePreterite2Run.FontSize = 12D;

				indicativePreterite2TableCell.Blocks.Add(new Paragraph(indicativePreterite2Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite2TableCell);

				TableCell indicativePreterite3TableCell = new TableCell();

				indicativePreterite3TableCell.BorderBrush = Brushes.Black;
				indicativePreterite3TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite3TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite3Run = new Run("-í");

				indicativePreterite3Run.FontSize = 12D;

				indicativePreterite3TableCell.Blocks.Add(new Paragraph(indicativePreterite3Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite3TableCell);

				TableCell indicativePreterite4TableCell = new TableCell();

				indicativePreterite4TableCell.BorderBrush = Brushes.Black;
				indicativePreterite4TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite4TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite4Run = new Run("-aste");

				indicativePreterite4Run.FontSize = 12D;

				indicativePreterite4TableCell.Blocks.Add(new Paragraph(indicativePreterite4Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite4TableCell);

				TableCell indicativePreterite5TableCell = new TableCell();

				indicativePreterite5TableCell.BorderBrush = Brushes.Black;
				indicativePreterite5TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite5TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite5Run = new Run("-iste");

				indicativePreterite5Run.FontSize = 12D;

				indicativePreterite5TableCell.Blocks.Add(new Paragraph(indicativePreterite5Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite5TableCell);

				TableCell indicativePreterite6TableCell = new TableCell();

				indicativePreterite6TableCell.BorderBrush = Brushes.Black;
				indicativePreterite6TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite6TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite6Run = new Run("-ó");

				indicativePreterite6Run.FontSize = 12D;

				indicativePreterite6TableCell.Blocks.Add(new Paragraph(indicativePreterite6Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite6TableCell);

				TableCell indicativePreterite7TableCell = new TableCell();

				indicativePreterite7TableCell.BorderBrush = Brushes.Black;
				indicativePreterite7TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite7TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite7Run = new Run("-ió");

				indicativePreterite7Run.FontSize = 12D;

				indicativePreterite7TableCell.Blocks.Add(new Paragraph(indicativePreterite7Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite7TableCell);

				TableCell indicativePreterite8TableCell = new TableCell();

				indicativePreterite8TableCell.BorderBrush = Brushes.Black;
				indicativePreterite8TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite8TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite8Run = new Run("-amos");

				indicativePreterite8Run.FontSize = 12D;

				indicativePreterite8TableCell.Blocks.Add(new Paragraph(indicativePreterite8Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite8TableCell);

				TableCell indicativePreterite9TableCell = new TableCell();

				indicativePreterite9TableCell.BorderBrush = Brushes.Black;
				indicativePreterite9TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite9TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite9Run = new Run("-imos");

				indicativePreterite9Run.FontSize = 12D;

				indicativePreterite9TableCell.Blocks.Add(new Paragraph(indicativePreterite9Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite9TableCell);

				TableCell indicativePreterite10TableCell = new TableCell();

				indicativePreterite10TableCell.BorderBrush = Brushes.Black;
				indicativePreterite10TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite10TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite10Run = new Run("-asteis");

				indicativePreterite10Run.FontSize = 12D;

				indicativePreterite10TableCell.Blocks.Add(new Paragraph(indicativePreterite10Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite10TableCell);

				TableCell indicativePreterite11TableCell = new TableCell();

				indicativePreterite11TableCell.BorderBrush = Brushes.Black;
				indicativePreterite11TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite11TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite11Run = new Run("-isteis");

				indicativePreterite11Run.FontSize = 12D;

				indicativePreterite11TableCell.Blocks.Add(new Paragraph(indicativePreterite11Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite11TableCell);

				TableCell indicativePreterite12TableCell = new TableCell();

				indicativePreterite12TableCell.BorderBrush = Brushes.Black;
				indicativePreterite12TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite12TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite12Run = new Run("-aron");

				indicativePreterite12Run.FontSize = 12D;

				indicativePreterite12TableCell.Blocks.Add(new Paragraph(indicativePreterite12Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite12TableCell);

				TableCell indicativePreterite13TableCell = new TableCell();

				indicativePreterite13TableCell.BorderBrush = Brushes.Black;
				indicativePreterite13TableCell.BorderThickness = tableBorderThickness;
				indicativePreterite13TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativePreterite13Run = new Run("-ieron");

				indicativePreterite13Run.FontSize = 12D;

				indicativePreterite13TableCell.Blocks.Add(new Paragraph(indicativePreterite13Run));

				indicativePreteriteTableRow.Cells.Add(indicativePreterite13TableCell);

				indicativeTableTableRowGroup.Rows.Add(indicativePreteriteTableRow);

				/*
				 * Indicative | Row: future
				 */

				TableRow indicativeFutureTableRow = new TableRow();

				TableCell indicativeFuture1TableCell = new TableCell();

				indicativeFuture1TableCell.Background = Brushes.LightGray;
				indicativeFuture1TableCell.BorderBrush = Brushes.Black;
				indicativeFuture1TableCell.BorderThickness = tableBorderThickness;
				indicativeFuture1TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeFuture1Run = new Run("Future:");

				indicativeFuture1Run.FontSize = 12D;

				indicativeFuture1TableCell.Blocks.Add(new Paragraph(new Bold(indicativeFuture1Run)));

				indicativeFutureTableRow.Cells.Add(indicativeFuture1TableCell);

				TableCell indicativeFuture2TableCell = new TableCell();

				indicativeFuture2TableCell.BorderBrush = Brushes.Black;
				indicativeFuture2TableCell.BorderThickness = tableBorderThickness;
				indicativeFuture2TableCell.ColumnSpan = 2;
				indicativeFuture2TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeFuture2Run = new Run("infinitive + é");

				indicativeFuture2Run.FontSize = 12D;

				indicativeFuture2TableCell.Blocks.Add(new Paragraph(indicativeFuture2Run));

				indicativeFutureTableRow.Cells.Add(indicativeFuture2TableCell);

				TableCell indicativeFuture3TableCell = new TableCell();

				indicativeFuture3TableCell.BorderBrush = Brushes.Black;
				indicativeFuture3TableCell.BorderThickness = tableBorderThickness;
				indicativeFuture3TableCell.ColumnSpan = 2;
				indicativeFuture3TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeFuture3Run = new Run("infinitive + ás");

				indicativeFuture3Run.FontSize = 12D;

				indicativeFuture3TableCell.Blocks.Add(new Paragraph(indicativeFuture3Run));

				indicativeFutureTableRow.Cells.Add(indicativeFuture3TableCell);

				TableCell indicativeFuture4TableCell = new TableCell();

				indicativeFuture4TableCell.BorderBrush = Brushes.Black;
				indicativeFuture4TableCell.BorderThickness = tableBorderThickness;
				indicativeFuture4TableCell.ColumnSpan = 2;
				indicativeFuture4TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeFuture4Run = new Run("infinitive + á");

				indicativeFuture4Run.FontSize = 12D;

				indicativeFuture4TableCell.Blocks.Add(new Paragraph(indicativeFuture4Run));

				indicativeFutureTableRow.Cells.Add(indicativeFuture4TableCell);

				TableCell indicativeFuture5TableCell = new TableCell();

				indicativeFuture5TableCell.BorderBrush = Brushes.Black;
				indicativeFuture5TableCell.BorderThickness = tableBorderThickness;
				indicativeFuture5TableCell.ColumnSpan = 2;
				indicativeFuture5TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeFuture5Run = new Run("infinitive + emos");

				indicativeFuture5Run.FontSize = 12D;

				indicativeFuture5TableCell.Blocks.Add(new Paragraph(indicativeFuture5Run));

				indicativeFutureTableRow.Cells.Add(indicativeFuture5TableCell);

				TableCell indicativeFuture6TableCell = new TableCell();

				indicativeFuture6TableCell.BorderBrush = Brushes.Black;
				indicativeFuture6TableCell.BorderThickness = tableBorderThickness;
				indicativeFuture6TableCell.ColumnSpan = 2;
				indicativeFuture6TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeFuture6Run = new Run("infinitive + éis");

				indicativeFuture6Run.FontSize = 12D;

				indicativeFuture6TableCell.Blocks.Add(new Paragraph(indicativeFuture6Run));

				indicativeFutureTableRow.Cells.Add(indicativeFuture6TableCell);

				TableCell indicativeFuture7TableCell = new TableCell();

				indicativeFuture7TableCell.BorderBrush = Brushes.Black;
				indicativeFuture7TableCell.BorderThickness = tableBorderThickness;
				indicativeFuture7TableCell.ColumnSpan = 2;
				indicativeFuture7TableCell.Padding = tableTableCellPaddingThickness;

				Run indicativeFuture7Run = new Run("infinitive + án");

				indicativeFuture7Run.FontSize = 12D;

				indicativeFuture7TableCell.Blocks.Add(new Paragraph(indicativeFuture7Run));

				indicativeFutureTableRow.Cells.Add(indicativeFuture7TableCell);

				indicativeTableTableRowGroup.Rows.Add(indicativeFutureTableRow);

				/*
				 * Indicative | RowGroup to Table and Table to FlowDocument.
				 */

				indicativeTable.RowGroups.Add(indicativeTableTableRowGroup);

				this.tableFlowDocument.Blocks.Add(indicativeTable);

				/*
				 * Subjunctive
				 */

				Paragraph subjunctiveParagraph = new Paragraph();

				subjunctiveParagraph.Inlines.Add(new Bold(new Run("Subjunctive")));

				this.tableFlowDocument.Blocks.Add(subjunctiveParagraph);

				Table subjunctiveTable = new Table();

				subjunctiveTable.BorderBrush = Brushes.Black;
				subjunctiveTable.BorderThickness = tableBorderThickness;
				subjunctiveTable.CellSpacing = 0D;

				TableRowGroup subjunctiveTableTableRowGroup = new TableRowGroup();

				/*
				 * Subjunctive | Row: header
				 */

				TableRow subjunctiveHeaderTableRow = new TableRow();

				subjunctiveHeaderTableRow.Background = Brushes.LightGray;

				TableCell subjunctiveHeader1TableCell = new TableCell();

				subjunctiveHeader1TableCell.BorderBrush = Brushes.Black;
				subjunctiveHeader1TableCell.BorderThickness = tableBorderThickness;
				subjunctiveHeader1TableCell.Padding = tableTableCellPaddingThickness;
				subjunctiveHeader1TableCell.TextAlignment = TextAlignment.Center;

				Run subjunctiveHeader1Run = new Run(string.Empty);

				subjunctiveHeader1Run.FontSize = 12D;

				subjunctiveHeader1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader1Run)));

				subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader1TableCell);

				TableCell subjunctiveHeader2TableCell = new TableCell();

				subjunctiveHeader2TableCell.BorderBrush = Brushes.Black;
				subjunctiveHeader2TableCell.BorderThickness = tableBorderThickness;
				subjunctiveHeader2TableCell.ColumnSpan = 2;
				subjunctiveHeader2TableCell.Padding = tableTableCellPaddingThickness;
				subjunctiveHeader2TableCell.TextAlignment = TextAlignment.Center;

				Run subjunctiveHeader2Run = new Run("yo");

				subjunctiveHeader2Run.FontSize = 12D;

				subjunctiveHeader2TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader2Run)));

				subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader2TableCell);

				TableCell subjunctiveHeader3TableCell = new TableCell();

				subjunctiveHeader3TableCell.BorderBrush = Brushes.Black;
				subjunctiveHeader3TableCell.BorderThickness = tableBorderThickness;
				subjunctiveHeader3TableCell.ColumnSpan = 2;
				subjunctiveHeader3TableCell.Padding = tableTableCellPaddingThickness;
				subjunctiveHeader3TableCell.TextAlignment = TextAlignment.Center;

				Run subjunctiveHeader3Run = new Run("tú");

				subjunctiveHeader3Run.FontSize = 12D;

				subjunctiveHeader3TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader3Run)));

				subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader3TableCell);

				TableCell subjunctiveHeader4TableCell = new TableCell();

				subjunctiveHeader4TableCell.BorderBrush = Brushes.Black;
				subjunctiveHeader4TableCell.BorderThickness = tableBorderThickness;
				subjunctiveHeader4TableCell.ColumnSpan = 2;
				subjunctiveHeader4TableCell.Padding = tableTableCellPaddingThickness;
				subjunctiveHeader4TableCell.TextAlignment = TextAlignment.Center;

				Run subjunctiveHeader4Run = new Run("él/ella/ello");

				subjunctiveHeader4Run.FontSize = 12D;

				subjunctiveHeader4TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader4Run)));

				subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader4TableCell);

				TableCell subjunctiveHeader5TableCell = new TableCell();

				subjunctiveHeader5TableCell.BorderBrush = Brushes.Black;
				subjunctiveHeader5TableCell.BorderThickness = tableBorderThickness;
				subjunctiveHeader5TableCell.ColumnSpan = 2;
				subjunctiveHeader5TableCell.Padding = tableTableCellPaddingThickness;
				subjunctiveHeader5TableCell.TextAlignment = TextAlignment.Center;

				Run subjunctiveHeader5Run = new Run("nosotros/nosotras");

				subjunctiveHeader5Run.FontSize = 12D;

				subjunctiveHeader5TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader5Run)));

				subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader5TableCell);

				TableCell subjunctiveHeader6TableCell = new TableCell();

				subjunctiveHeader6TableCell.BorderBrush = Brushes.Black;
				subjunctiveHeader6TableCell.BorderThickness = tableBorderThickness;
				subjunctiveHeader6TableCell.ColumnSpan = 2;
				subjunctiveHeader6TableCell.Padding = tableTableCellPaddingThickness;
				subjunctiveHeader6TableCell.TextAlignment = TextAlignment.Center;

				Run subjunctiveHeader6Run = new Run("vosotros/vosotras");

				subjunctiveHeader6Run.FontSize = 12D;

				subjunctiveHeader6TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader6Run)));

				subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader6TableCell);

				TableCell subjunctiveHeader7TableCell = new TableCell();

				subjunctiveHeader7TableCell.BorderBrush = Brushes.Black;
				subjunctiveHeader7TableCell.BorderThickness = tableBorderThickness;
				subjunctiveHeader7TableCell.ColumnSpan = 2;
				subjunctiveHeader7TableCell.Padding = tableTableCellPaddingThickness;
				subjunctiveHeader7TableCell.TextAlignment = TextAlignment.Center;

				Run subjunctiveHeader7Run = new Run("ellos/ellas");

				subjunctiveHeader7Run.FontSize = 12D;

				subjunctiveHeader7TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader7Run)));

				subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader7TableCell);

				subjunctiveTableTableRowGroup.Rows.Add(subjunctiveHeaderTableRow);

				/*
				 * Subjunctive | Row: present
				 */

				TableRow subjunctivePresentTableRow = new TableRow();

				TableCell subjunctivePresent1TableCell = new TableCell();

				subjunctivePresent1TableCell.Background = Brushes.LightGray;
				subjunctivePresent1TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent1TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent1TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent1Run = new Run("Present:");

				subjunctivePresent1Run.FontSize = 12D;

				subjunctivePresent1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctivePresent1Run)));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent1TableCell);

				TableCell subjunctivePresent2TableCell = new TableCell();

				subjunctivePresent2TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent2TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent2TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent2Run = new Run("-e");

				subjunctivePresent2Run.FontSize = 12D;

				subjunctivePresent2TableCell.Blocks.Add(new Paragraph(subjunctivePresent2Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent2TableCell);

				TableCell subjunctivePresent3TableCell = new TableCell();

				subjunctivePresent3TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent3TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent3TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent3Run = new Run("-a");

				subjunctivePresent3Run.FontSize = 12D;

				subjunctivePresent3TableCell.Blocks.Add(new Paragraph(subjunctivePresent3Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent3TableCell);

				TableCell subjunctivePresent4TableCell = new TableCell();

				subjunctivePresent4TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent4TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent4TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent4Run = new Run("-es");

				subjunctivePresent4Run.FontSize = 12D;

				subjunctivePresent4TableCell.Blocks.Add(new Paragraph(subjunctivePresent4Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent4TableCell);

				TableCell subjunctivePresent5TableCell = new TableCell();

				subjunctivePresent5TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent5TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent5TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent5Run = new Run("-as");

				subjunctivePresent5Run.FontSize = 12D;

				subjunctivePresent5TableCell.Blocks.Add(new Paragraph(subjunctivePresent5Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent5TableCell);

				TableCell subjunctivePresent6TableCell = new TableCell();

				subjunctivePresent6TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent6TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent6TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent6Run = new Run("-e");

				subjunctivePresent6Run.FontSize = 12D;

				subjunctivePresent6TableCell.Blocks.Add(new Paragraph(subjunctivePresent6Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent6TableCell);

				TableCell subjunctivePresent7TableCell = new TableCell();

				subjunctivePresent7TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent7TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent7TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent7Run = new Run("-a");

				subjunctivePresent7Run.FontSize = 12D;

				subjunctivePresent7TableCell.Blocks.Add(new Paragraph(subjunctivePresent7Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent7TableCell);

				TableCell subjunctivePresent8TableCell = new TableCell();

				subjunctivePresent8TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent8TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent8TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent8Run = new Run("-emos");

				subjunctivePresent8Run.FontSize = 12D;

				subjunctivePresent8TableCell.Blocks.Add(new Paragraph(subjunctivePresent8Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent8TableCell);

				TableCell subjunctivePresent9TableCell = new TableCell();

				subjunctivePresent9TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent9TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent9TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent9Run = new Run("-amos");

				subjunctivePresent9Run.FontSize = 12D;

				subjunctivePresent9TableCell.Blocks.Add(new Paragraph(subjunctivePresent9Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent9TableCell);

				TableCell subjunctivePresent10TableCell = new TableCell();

				subjunctivePresent10TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent10TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent10TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent10Run = new Run("-éis");

				subjunctivePresent10Run.FontSize = 12D;

				subjunctivePresent10TableCell.Blocks.Add(new Paragraph(subjunctivePresent10Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent10TableCell);

				TableCell subjunctivePresent11TableCell = new TableCell();

				subjunctivePresent11TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent11TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent11TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent11Run = new Run("-áis");

				subjunctivePresent11Run.FontSize = 12D;

				subjunctivePresent11TableCell.Blocks.Add(new Paragraph(subjunctivePresent11Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent11TableCell);

				TableCell subjunctivePresent12TableCell = new TableCell();

				subjunctivePresent12TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent12TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent12TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent12Run = new Run("-en");

				subjunctivePresent12Run.FontSize = 12D;

				subjunctivePresent12TableCell.Blocks.Add(new Paragraph(subjunctivePresent12Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent12TableCell);

				TableCell subjunctivePresent13TableCell = new TableCell();

				subjunctivePresent13TableCell.BorderBrush = Brushes.Black;
				subjunctivePresent13TableCell.BorderThickness = tableBorderThickness;
				subjunctivePresent13TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctivePresent13Run = new Run("-an");

				subjunctivePresent13Run.FontSize = 12D;

				subjunctivePresent13TableCell.Blocks.Add(new Paragraph(subjunctivePresent13Run));

				subjunctivePresentTableRow.Cells.Add(subjunctivePresent13TableCell);

				subjunctiveTableTableRowGroup.Rows.Add(subjunctivePresentTableRow);

				/*
				 * Subjunctive | Row: imperfect (ra)
				 */

				TableRow subjunctiveImperfectRaTableRow = new TableRow();

				TableCell subjunctiveImperfectRa1TableCell = new TableCell();

				subjunctiveImperfectRa1TableCell.Background = Brushes.LightGray;
				subjunctiveImperfectRa1TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa1TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa1TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa1Run = new Run("Imperfect (ra):");

				subjunctiveImperfectRa1Run.FontSize = 12D;

				subjunctiveImperfectRa1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveImperfectRa1Run)));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa1TableCell);

				TableCell subjunctiveImperfectRa2TableCell = new TableCell();

				subjunctiveImperfectRa2TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa2TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa2TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa2Run = new Run("infinitive + a");

				subjunctiveImperfectRa2Run.FontSize = 12D;

				subjunctiveImperfectRa2TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa2Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa2TableCell);

				TableCell subjunctiveImperfectRa3TableCell = new TableCell();

				subjunctiveImperfectRa3TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa3TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa3TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa3Run = new Run("-iera");

				subjunctiveImperfectRa3Run.FontSize = 12D;

				subjunctiveImperfectRa3TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa3Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa3TableCell);

				TableCell subjunctiveImperfectRa4TableCell = new TableCell();

				subjunctiveImperfectRa4TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa4TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa4TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa4Run = new Run("infinitive + as");

				subjunctiveImperfectRa4Run.FontSize = 12D;

				subjunctiveImperfectRa4TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa4Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa4TableCell);

				TableCell subjunctiveImperfectRa5TableCell = new TableCell();

				subjunctiveImperfectRa5TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa5TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa5TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa5Run = new Run("-ieras");

				subjunctiveImperfectRa5Run.FontSize = 12D;

				subjunctiveImperfectRa5TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa5Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa5TableCell);

				TableCell subjunctiveImperfectRa6TableCell = new TableCell();

				subjunctiveImperfectRa6TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa6TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa6TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa6Run = new Run("infinitive + a");

				subjunctiveImperfectRa6Run.FontSize = 12D;

				subjunctiveImperfectRa6TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa6Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa6TableCell);

				TableCell subjunctiveImperfectRa7TableCell = new TableCell();

				subjunctiveImperfectRa7TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa7TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa7TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa7Run = new Run("-iera");

				subjunctiveImperfectRa7Run.FontSize = 12D;

				subjunctiveImperfectRa7TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa7Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa7TableCell);

				TableCell subjunctiveImperfectRa8TableCell = new TableCell();

				subjunctiveImperfectRa8TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa8TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa8TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa8Run = new Run("-infinitive + amos");

				subjunctiveImperfectRa8Run.FontSize = 12D;

				subjunctiveImperfectRa8TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa8Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa8TableCell);

				TableCell subjunctiveImperfectRa9TableCell = new TableCell();

				subjunctiveImperfectRa9TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa9TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa9TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa9Run = new Run("-iéramos");

				subjunctiveImperfectRa9Run.FontSize = 12D;

				subjunctiveImperfectRa9TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa9Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa9TableCell);

				TableCell subjunctiveImperfectRa10TableCell = new TableCell();

				subjunctiveImperfectRa10TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa10TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa10TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa10Run = new Run("infinitive + ais");

				subjunctiveImperfectRa10Run.FontSize = 12D;

				subjunctiveImperfectRa10TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa10Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa10TableCell);

				TableCell subjunctiveImperfectRa11TableCell = new TableCell();

				subjunctiveImperfectRa11TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa11TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa11TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa11Run = new Run("-ierais");

				subjunctiveImperfectRa11Run.FontSize = 12D;

				subjunctiveImperfectRa11TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa11Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa11TableCell);

				TableCell subjunctiveImperfectRa12TableCell = new TableCell();

				subjunctiveImperfectRa12TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa12TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa12TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa12Run = new Run("infinitive + an");

				subjunctiveImperfectRa12Run.FontSize = 12D;

				subjunctiveImperfectRa12TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa12Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa12TableCell);

				TableCell subjunctiveImperfectRa13TableCell = new TableCell();

				subjunctiveImperfectRa13TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectRa13TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectRa13TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectRa13Run = new Run("-ieran");

				subjunctiveImperfectRa13Run.FontSize = 12D;

				subjunctiveImperfectRa13TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa13Run));

				subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa13TableCell);

				subjunctiveTableTableRowGroup.Rows.Add(subjunctiveImperfectRaTableRow);

				/*
				 * Subjunctive | Row: imperfect (se)
				 */

				TableRow subjunctiveImperfectSeTableRow = new TableRow();

				TableCell subjunctiveImperfectSe1TableCell = new TableCell();

				subjunctiveImperfectSe1TableCell.Background = Brushes.LightGray;
				subjunctiveImperfectSe1TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe1TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe1TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe1Run = new Run("Imperfect (se):");

				subjunctiveImperfectSe1Run.FontSize = 12D;

				subjunctiveImperfectSe1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveImperfectSe1Run)));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe1TableCell);

				TableCell subjunctiveImperfectSe2TableCell = new TableCell();

				subjunctiveImperfectSe2TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe2TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe2TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe2Run = new Run("-ase");

				subjunctiveImperfectSe2Run.FontSize = 12D;

				subjunctiveImperfectSe2TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe2Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe2TableCell);

				TableCell subjunctiveImperfectSe3TableCell = new TableCell();

				subjunctiveImperfectSe3TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe3TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe3TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe3Run = new Run("-iese");

				subjunctiveImperfectSe3Run.FontSize = 12D;

				subjunctiveImperfectSe3TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe3Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe3TableCell);

				TableCell subjunctiveImperfectSe4TableCell = new TableCell();

				subjunctiveImperfectSe4TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe4TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe4TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe4Run = new Run("-ases");

				subjunctiveImperfectSe4Run.FontSize = 12D;

				subjunctiveImperfectSe4TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe4Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe4TableCell);

				TableCell subjunctiveImperfectSe5TableCell = new TableCell();

				subjunctiveImperfectSe5TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe5TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe5TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe5Run = new Run("-ieses");

				subjunctiveImperfectSe5Run.FontSize = 12D;

				subjunctiveImperfectSe5TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe5Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe5TableCell);

				TableCell subjunctiveImperfectSe6TableCell = new TableCell();

				subjunctiveImperfectSe6TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe6TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe6TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe6Run = new Run("-ase");

				subjunctiveImperfectSe6Run.FontSize = 12D;

				subjunctiveImperfectSe6TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe6Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe6TableCell);

				TableCell subjunctiveImperfectSe7TableCell = new TableCell();

				subjunctiveImperfectSe7TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe7TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe7TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe7Run = new Run("-iese");

				subjunctiveImperfectSe7Run.FontSize = 12D;

				subjunctiveImperfectSe7TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe7Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe7TableCell);

				TableCell subjunctiveImperfectSe8TableCell = new TableCell();

				subjunctiveImperfectSe8TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe8TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe8TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe8Run = new Run("-ásemos");

				subjunctiveImperfectSe8Run.FontSize = 12D;

				subjunctiveImperfectSe8TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe8Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe8TableCell);

				TableCell subjunctiveImperfectSe9TableCell = new TableCell();

				subjunctiveImperfectSe9TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe9TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe9TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe9Run = new Run("-iésemos");

				subjunctiveImperfectSe9Run.FontSize = 12D;

				subjunctiveImperfectSe9TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe9Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe9TableCell);

				TableCell subjunctiveImperfectSe10TableCell = new TableCell();

				subjunctiveImperfectSe10TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe10TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe10TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe10Run = new Run("-aseis");

				subjunctiveImperfectSe10Run.FontSize = 12D;

				subjunctiveImperfectSe10TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe10Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe10TableCell);

				TableCell subjunctiveImperfectSe11TableCell = new TableCell();

				subjunctiveImperfectSe11TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe11TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe11TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe11Run = new Run("-ieseis");

				subjunctiveImperfectSe11Run.FontSize = 12D;

				subjunctiveImperfectSe11TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe11Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe11TableCell);

				TableCell subjunctiveImperfectSe12TableCell = new TableCell();

				subjunctiveImperfectSe12TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe12TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe12TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe12Run = new Run("-asen");

				subjunctiveImperfectSe12Run.FontSize = 12D;

				subjunctiveImperfectSe12TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe12Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe12TableCell);

				TableCell subjunctiveImperfectSe13TableCell = new TableCell();

				subjunctiveImperfectSe13TableCell.BorderBrush = Brushes.Black;
				subjunctiveImperfectSe13TableCell.BorderThickness = tableBorderThickness;
				subjunctiveImperfectSe13TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveImperfectSe13Run = new Run("-iesen");

				subjunctiveImperfectSe13Run.FontSize = 12D;

				subjunctiveImperfectSe13TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe13Run));

				subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe13TableCell);

				subjunctiveTableTableRowGroup.Rows.Add(subjunctiveImperfectSeTableRow);

				/*
				 * Subjunctive | Row: future
				 */

				TableRow subjunctiveFutureTableRow = new TableRow();

				TableCell subjunctiveFuture1TableCell = new TableCell();

				subjunctiveFuture1TableCell.Background = Brushes.LightGray;
				subjunctiveFuture1TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture1TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture1TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture1Run = new Run("Future:");

				subjunctiveFuture1Run.FontSize = 12D;

				subjunctiveFuture1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveFuture1Run)));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture1TableCell);

				TableCell subjunctiveFuture2TableCell = new TableCell();

				subjunctiveFuture2TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture2TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture2TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture2Run = new Run("infinitive + e");

				subjunctiveFuture2Run.FontSize = 12D;

				subjunctiveFuture2TableCell.Blocks.Add(new Paragraph(subjunctiveFuture2Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture2TableCell);

				TableCell subjunctiveFuture3TableCell = new TableCell();

				subjunctiveFuture3TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture3TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture3TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture3Run = new Run("-iere");

				subjunctiveFuture3Run.FontSize = 12D;

				subjunctiveFuture3TableCell.Blocks.Add(new Paragraph(subjunctiveFuture3Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture3TableCell);

				TableCell subjunctiveFuture4TableCell = new TableCell();

				subjunctiveFuture4TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture4TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture4TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture4Run = new Run("infinitive + es");

				subjunctiveFuture4Run.FontSize = 12D;

				subjunctiveFuture4TableCell.Blocks.Add(new Paragraph(subjunctiveFuture4Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture4TableCell);

				TableCell subjunctiveFuture5TableCell = new TableCell();

				subjunctiveFuture5TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture5TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture5TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture5Run = new Run("-ieres");

				subjunctiveFuture5Run.FontSize = 12D;

				subjunctiveFuture5TableCell.Blocks.Add(new Paragraph(subjunctiveFuture5Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture5TableCell);

				TableCell subjunctiveFuture6TableCell = new TableCell();

				subjunctiveFuture6TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture6TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture6TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture6Run = new Run("infinitive + e");

				subjunctiveFuture6Run.FontSize = 12D;

				subjunctiveFuture6TableCell.Blocks.Add(new Paragraph(subjunctiveFuture6Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture6TableCell);

				TableCell subjunctiveFuture7TableCell = new TableCell();

				subjunctiveFuture7TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture7TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture7TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture7Run = new Run("-iere");

				subjunctiveFuture7Run.FontSize = 12D;

				subjunctiveFuture7TableCell.Blocks.Add(new Paragraph(subjunctiveFuture7Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture7TableCell);

				TableCell subjunctiveFuture8TableCell = new TableCell();

				subjunctiveFuture8TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture8TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture8TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture8Run = new Run("infinitive + emos");

				subjunctiveFuture8Run.FontSize = 12D;

				subjunctiveFuture8TableCell.Blocks.Add(new Paragraph(subjunctiveFuture8Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture8TableCell);

				TableCell subjunctiveFuture9TableCell = new TableCell();

				subjunctiveFuture9TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture9TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture9TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture9Run = new Run("-iéremos");

				subjunctiveFuture9Run.FontSize = 12D;

				subjunctiveFuture9TableCell.Blocks.Add(new Paragraph(subjunctiveFuture9Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture9TableCell);

				TableCell subjunctiveFuture10TableCell = new TableCell();

				subjunctiveFuture10TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture10TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture10TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture10Run = new Run("infinitive + eis");

				subjunctiveFuture10Run.FontSize = 12D;

				subjunctiveFuture10TableCell.Blocks.Add(new Paragraph(subjunctiveFuture10Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture10TableCell);

				TableCell subjunctiveFuture11TableCell = new TableCell();

				subjunctiveFuture11TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture11TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture11TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture11Run = new Run("-iereis");

				subjunctiveFuture11Run.FontSize = 12D;

				subjunctiveFuture11TableCell.Blocks.Add(new Paragraph(subjunctiveFuture11Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture11TableCell);

				TableCell subjunctiveFuture12TableCell = new TableCell();

				subjunctiveFuture12TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture12TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture12TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture12Run = new Run("infinitive + en");

				subjunctiveFuture12Run.FontSize = 12D;

				subjunctiveFuture12TableCell.Blocks.Add(new Paragraph(subjunctiveFuture12Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture12TableCell);

				TableCell subjunctiveFuture13TableCell = new TableCell();

				subjunctiveFuture13TableCell.BorderBrush = Brushes.Black;
				subjunctiveFuture13TableCell.BorderThickness = tableBorderThickness;
				subjunctiveFuture13TableCell.Padding = tableTableCellPaddingThickness;

				Run subjunctiveFuture13Run = new Run("-ieren");

				subjunctiveFuture13Run.FontSize = 12D;

				subjunctiveFuture13TableCell.Blocks.Add(new Paragraph(subjunctiveFuture13Run));

				subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture13TableCell);

				subjunctiveTableTableRowGroup.Rows.Add(subjunctiveFutureTableRow);

				/*
				 * Subjunctive | RowGroup to Table and Table to FlowDocument.
				 */

				subjunctiveTable.RowGroups.Add(subjunctiveTableTableRowGroup);

				this.tableFlowDocument.Blocks.Add(subjunctiveTable);

				/*
				 * Imperative
				 */

				Paragraph imperativeParagraph = new Paragraph();

				imperativeParagraph.Inlines.Add(new Bold(new Run("Imperative")));

				this.tableFlowDocument.Blocks.Add(imperativeParagraph);

				Table imperativeTable = new Table();

				imperativeTable.BorderBrush = Brushes.Black;
				imperativeTable.BorderThickness = tableBorderThickness;
				imperativeTable.CellSpacing = 0D;

				TableRowGroup imperativeTableTableRowGroup = new TableRowGroup();

				/*
				 * Imperative | Row: header
				 */

				TableRow imperativeHeaderTableRow = new TableRow();

				imperativeHeaderTableRow.Background = Brushes.LightGray;

				TableCell imperativeHeader1TableCell = new TableCell();

				imperativeHeader1TableCell.BorderBrush = Brushes.Black;
				imperativeHeader1TableCell.BorderThickness = tableBorderThickness;
				imperativeHeader1TableCell.Padding = tableTableCellPaddingThickness;
				imperativeHeader1TableCell.TextAlignment = TextAlignment.Center;

				Run imperativeHeader1Run = new Run(string.Empty);

				imperativeHeader1Run.FontSize = 12D;

				imperativeHeader1TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader1Run)));

				imperativeHeaderTableRow.Cells.Add(imperativeHeader1TableCell);

				TableCell imperativeHeader2TableCell = new TableCell();

				imperativeHeader2TableCell.BorderBrush = Brushes.Black;
				imperativeHeader2TableCell.BorderThickness = tableBorderThickness;
				imperativeHeader2TableCell.Padding = tableTableCellPaddingThickness;
				imperativeHeader2TableCell.TextAlignment = TextAlignment.Center;

				Run imperativeHeader2Run = new Run("yo");

				imperativeHeader2Run.FontSize = 12D;

				imperativeHeader2TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader2Run)));

				imperativeHeaderTableRow.Cells.Add(imperativeHeader2TableCell);

				TableCell imperativeHeader3TableCell = new TableCell();

				imperativeHeader3TableCell.BorderBrush = Brushes.Black;
				imperativeHeader3TableCell.BorderThickness = tableBorderThickness;
				imperativeHeader3TableCell.ColumnSpan = 2;
				imperativeHeader3TableCell.Padding = tableTableCellPaddingThickness;
				imperativeHeader3TableCell.TextAlignment = TextAlignment.Center;

				Run imperativeHeader3Run = new Run("tú");

				imperativeHeader3Run.FontSize = 12D;

				imperativeHeader3TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader3Run)));

				imperativeHeaderTableRow.Cells.Add(imperativeHeader3TableCell);

				TableCell imperativeHeader4TableCell = new TableCell();

				imperativeHeader4TableCell.BorderBrush = Brushes.Black;
				imperativeHeader4TableCell.BorderThickness = tableBorderThickness;
				imperativeHeader4TableCell.ColumnSpan = 2;
				imperativeHeader4TableCell.Padding = tableTableCellPaddingThickness;
				imperativeHeader4TableCell.TextAlignment = TextAlignment.Center;

				Run imperativeHeader4Run = new Run("él/ella/ello");

				imperativeHeader4Run.FontSize = 12D;

				imperativeHeader4TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader4Run)));

				imperativeHeaderTableRow.Cells.Add(imperativeHeader4TableCell);

				TableCell imperativeHeader5TableCell = new TableCell();

				imperativeHeader5TableCell.BorderBrush = Brushes.Black;
				imperativeHeader5TableCell.BorderThickness = tableBorderThickness;
				imperativeHeader5TableCell.ColumnSpan = 2;
				imperativeHeader5TableCell.Padding = tableTableCellPaddingThickness;
				imperativeHeader5TableCell.TextAlignment = TextAlignment.Center;

				Run imperativeHeader5Run = new Run("nosotros/nosotras");

				imperativeHeader5Run.FontSize = 12D;

				imperativeHeader5TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader5Run)));

				imperativeHeaderTableRow.Cells.Add(imperativeHeader5TableCell);

				TableCell imperativeHeader6TableCell = new TableCell();

				imperativeHeader6TableCell.BorderBrush = Brushes.Black;
				imperativeHeader6TableCell.BorderThickness = tableBorderThickness;
				imperativeHeader6TableCell.ColumnSpan = 2;
				imperativeHeader6TableCell.Padding = tableTableCellPaddingThickness;
				imperativeHeader6TableCell.TextAlignment = TextAlignment.Center;

				Run imperativeHeader6Run = new Run("vosotros/vosotras");

				imperativeHeader6Run.FontSize = 12D;

				imperativeHeader6TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader6Run)));

				imperativeHeaderTableRow.Cells.Add(imperativeHeader6TableCell);

				TableCell imperativeHeader7TableCell = new TableCell();

				imperativeHeader7TableCell.BorderBrush = Brushes.Black;
				imperativeHeader7TableCell.BorderThickness = tableBorderThickness;
				imperativeHeader7TableCell.ColumnSpan = 2;
				imperativeHeader7TableCell.Padding = tableTableCellPaddingThickness;
				imperativeHeader7TableCell.TextAlignment = TextAlignment.Center;

				Run imperativeHeader7Run = new Run("ellos/ellas");

				imperativeHeader7Run.FontSize = 12D;

				imperativeHeader7TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader7Run)));

				imperativeHeaderTableRow.Cells.Add(imperativeHeader7TableCell);

				imperativeTableTableRowGroup.Rows.Add(imperativeHeaderTableRow);

				/*
				 * Imperative | Row: affirmative
				 */

				TableRow imperativeAffirmativeTableRow = new TableRow();

				TableCell imperativeAffirmative1TableCell = new TableCell();

				imperativeAffirmative1TableCell.Background = Brushes.LightGray;
				imperativeAffirmative1TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative1TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative1TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative1Run = new Run("Affirmative:");

				imperativeAffirmative1Run.FontSize = 12D;

				imperativeAffirmative1TableCell.Blocks.Add(new Paragraph(new Bold(imperativeAffirmative1Run)));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative1TableCell);

				TableCell imperativeAffirmative2TableCell = new TableCell();

				imperativeAffirmative2TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative2TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative2TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative2Run = new Run("—");

				imperativeAffirmative2Run.FontSize = 12D;

				imperativeAffirmative2TableCell.Blocks.Add(new Paragraph(imperativeAffirmative2Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative2TableCell);

				TableCell imperativeAffirmative3TableCell = new TableCell();

				imperativeAffirmative3TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative3TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative3TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative3Run = new Run("-a");

				imperativeAffirmative3Run.FontSize = 12D;

				imperativeAffirmative3TableCell.Blocks.Add(new Paragraph(imperativeAffirmative3Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative3TableCell);

				TableCell imperativeAffirmative4TableCell = new TableCell();

				imperativeAffirmative4TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative4TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative4TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative4Run = new Run("-e");

				imperativeAffirmative4Run.FontSize = 12D;

				imperativeAffirmative4TableCell.Blocks.Add(new Paragraph(imperativeAffirmative4Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative4TableCell);

				TableCell imperativeAffirmative5TableCell = new TableCell();

				imperativeAffirmative5TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative5TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative5TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative5Run = new Run("-e");

				imperativeAffirmative5Run.FontSize = 12D;

				imperativeAffirmative5TableCell.Blocks.Add(new Paragraph(imperativeAffirmative5Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative5TableCell);

				TableCell imperativeAffirmative6TableCell = new TableCell();

				imperativeAffirmative6TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative6TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative6TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative6Run = new Run("-a");

				imperativeAffirmative6Run.FontSize = 12D;

				imperativeAffirmative6TableCell.Blocks.Add(new Paragraph(imperativeAffirmative6Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative6TableCell);

				TableCell imperativeAffirmative7TableCell = new TableCell();

				imperativeAffirmative7TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative7TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative7TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative7Run = new Run("-emos");

				imperativeAffirmative7Run.FontSize = 12D;

				imperativeAffirmative7TableCell.Blocks.Add(new Paragraph(imperativeAffirmative7Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative7TableCell);

				TableCell imperativeAffirmative8TableCell = new TableCell();

				imperativeAffirmative8TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative8TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative8TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative8Run = new Run("-amos");

				imperativeAffirmative8Run.FontSize = 12D;

				imperativeAffirmative8TableCell.Blocks.Add(new Paragraph(imperativeAffirmative8Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative8TableCell);

				TableCell imperativeAffirmative9TableCell = new TableCell();

				imperativeAffirmative9TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative9TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative9TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative9Run = new Run("-ad");

				imperativeAffirmative9Run.FontSize = 12D;

				imperativeAffirmative9TableCell.Blocks.Add(new Paragraph(imperativeAffirmative9Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative9TableCell);

				TableCell imperativeAffirmative10TableCell = new TableCell();

				imperativeAffirmative10TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative10TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative10TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative10Run = new Run("-ed/-id");

				imperativeAffirmative10Run.FontSize = 12D;

				imperativeAffirmative10TableCell.Blocks.Add(new Paragraph(imperativeAffirmative10Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative10TableCell);

				TableCell imperativeAffirmative11TableCell = new TableCell();

				imperativeAffirmative11TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative11TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative11TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative11Run = new Run("-en");

				imperativeAffirmative11Run.FontSize = 12D;

				imperativeAffirmative11TableCell.Blocks.Add(new Paragraph(imperativeAffirmative11Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative11TableCell);

				TableCell imperativeAffirmative12TableCell = new TableCell();

				imperativeAffirmative12TableCell.BorderBrush = Brushes.Black;
				imperativeAffirmative12TableCell.BorderThickness = tableBorderThickness;
				imperativeAffirmative12TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeAffirmative12Run = new Run("-an");

				imperativeAffirmative12Run.FontSize = 12D;

				imperativeAffirmative12TableCell.Blocks.Add(new Paragraph(imperativeAffirmative12Run));

				imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative12TableCell);

				imperativeTableTableRowGroup.Rows.Add(imperativeAffirmativeTableRow);

				/*
				 * Imperative | Row: negative
				 */

				TableRow imperativeNegativeTableRow = new TableRow();

				TableCell imperativeNegative1TableCell = new TableCell();

				imperativeNegative1TableCell.Background = Brushes.LightGray;
				imperativeNegative1TableCell.BorderBrush = Brushes.Black;
				imperativeNegative1TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative1TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative1Run = new Run("Negative:");

				imperativeNegative1Run.FontSize = 12D;

				imperativeNegative1TableCell.Blocks.Add(new Paragraph(new Bold(imperativeNegative1Run)));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative1TableCell);

				TableCell imperativeNegative2TableCell = new TableCell();

				imperativeNegative2TableCell.BorderBrush = Brushes.Black;
				imperativeNegative2TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative2TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative2Run = new Run("—");

				imperativeNegative2Run.FontSize = 12D;

				imperativeNegative2TableCell.Blocks.Add(new Paragraph(imperativeNegative2Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative2TableCell);

				TableCell imperativeNegative3TableCell = new TableCell();

				imperativeNegative3TableCell.BorderBrush = Brushes.Black;
				imperativeNegative3TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative3TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative3Run = new Run("-es");

				imperativeNegative3Run.FontSize = 12D;

				imperativeNegative3TableCell.Blocks.Add(new Paragraph(imperativeNegative3Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative3TableCell);

				TableCell imperativeNegative4TableCell = new TableCell();

				imperativeNegative4TableCell.BorderBrush = Brushes.Black;
				imperativeNegative4TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative4TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative4Run = new Run("-as");

				imperativeNegative4Run.FontSize = 12D;

				imperativeNegative4TableCell.Blocks.Add(new Paragraph(imperativeNegative4Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative4TableCell);

				TableCell imperativeNegative5TableCell = new TableCell();

				imperativeNegative5TableCell.BorderBrush = Brushes.Black;
				imperativeNegative5TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative5TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative5Run = new Run("-e");

				imperativeNegative5Run.FontSize = 12D;

				imperativeNegative5TableCell.Blocks.Add(new Paragraph(imperativeNegative5Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative5TableCell);

				TableCell imperativeNegative6TableCell = new TableCell();

				imperativeNegative6TableCell.BorderBrush = Brushes.Black;
				imperativeNegative6TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative6TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative6Run = new Run("-a");

				imperativeNegative6Run.FontSize = 12D;

				imperativeNegative6TableCell.Blocks.Add(new Paragraph(imperativeNegative6Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative6TableCell);

				TableCell imperativeNegative7TableCell = new TableCell();

				imperativeNegative7TableCell.BorderBrush = Brushes.Black;
				imperativeNegative7TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative7TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative7Run = new Run("-emos");

				imperativeNegative7Run.FontSize = 12D;

				imperativeNegative7TableCell.Blocks.Add(new Paragraph(imperativeNegative7Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative7TableCell);

				TableCell imperativeNegative8TableCell = new TableCell();

				imperativeNegative8TableCell.BorderBrush = Brushes.Black;
				imperativeNegative8TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative8TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative8Run = new Run("-amos");

				imperativeNegative8Run.FontSize = 12D;

				imperativeNegative8TableCell.Blocks.Add(new Paragraph(imperativeNegative8Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative8TableCell);

				TableCell imperativeNegative9TableCell = new TableCell();

				imperativeNegative9TableCell.BorderBrush = Brushes.Black;
				imperativeNegative9TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative9TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative9Run = new Run("-éis");

				imperativeNegative9Run.FontSize = 12D;

				imperativeNegative9TableCell.Blocks.Add(new Paragraph(imperativeNegative9Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative9TableCell);

				TableCell imperativeNegative10TableCell = new TableCell();

				imperativeNegative10TableCell.BorderBrush = Brushes.Black;
				imperativeNegative10TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative10TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative10Run = new Run("-áis");

				imperativeNegative10Run.FontSize = 12D;

				imperativeNegative10TableCell.Blocks.Add(new Paragraph(imperativeNegative10Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative10TableCell);

				TableCell imperativeNegative11TableCell = new TableCell();

				imperativeNegative11TableCell.BorderBrush = Brushes.Black;
				imperativeNegative11TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative11TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative11Run = new Run("-en");

				imperativeNegative11Run.FontSize = 12D;

				imperativeNegative11TableCell.Blocks.Add(new Paragraph(imperativeNegative11Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative11TableCell);

				TableCell imperativeNegative12TableCell = new TableCell();

				imperativeNegative12TableCell.BorderBrush = Brushes.Black;
				imperativeNegative12TableCell.BorderThickness = tableBorderThickness;
				imperativeNegative12TableCell.Padding = tableTableCellPaddingThickness;

				Run imperativeNegative12Run = new Run("-an");

				imperativeNegative12Run.FontSize = 12D;

				imperativeNegative12TableCell.Blocks.Add(new Paragraph(imperativeNegative12Run));

				imperativeNegativeTableRow.Cells.Add(imperativeNegative12TableCell);

				imperativeTableTableRowGroup.Rows.Add(imperativeNegativeTableRow);

				/*
				 * Imperative | RowGroup to Table and Table to FlowDocument.
				 */

				imperativeTable.RowGroups.Add(imperativeTableTableRowGroup);

				this.tableFlowDocument.Blocks.Add(imperativeTable);

				/*
				 * Conditional
				 */

				Paragraph conditionalParagraph = new Paragraph();

				conditionalParagraph.Inlines.Add(new Bold(new Run("Conditional")));

				this.tableFlowDocument.Blocks.Add(conditionalParagraph);

				Table conditionalTable = new Table();

				conditionalTable.BorderBrush = Brushes.Black;
				conditionalTable.BorderThickness = tableBorderThickness;
				conditionalTable.CellSpacing = 0D;

				TableRowGroup conditionalTableTableRowGroup = new TableRowGroup();

				/*
				 * Conditional | Row: header
				 */

				TableRow conditionalHeaderTableRow = new TableRow();

				conditionalHeaderTableRow.Background = Brushes.LightGray;

				TableCell conditionalHeader1TableCell = new TableCell();

				conditionalHeader1TableCell.BorderBrush = Brushes.Black;
				conditionalHeader1TableCell.BorderThickness = tableBorderThickness;
				conditionalHeader1TableCell.Padding = tableTableCellPaddingThickness;
				conditionalHeader1TableCell.TextAlignment = TextAlignment.Center;

				Run conditionalHeader1Run = new Run("yo");

				conditionalHeader1Run.FontSize = 12D;

				conditionalHeader1TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader1Run)));

				conditionalHeaderTableRow.Cells.Add(conditionalHeader1TableCell);

				TableCell conditionalHeader2TableCell = new TableCell();

				conditionalHeader2TableCell.BorderBrush = Brushes.Black;
				conditionalHeader2TableCell.BorderThickness = tableBorderThickness;
				conditionalHeader2TableCell.Padding = tableTableCellPaddingThickness;
				conditionalHeader2TableCell.TextAlignment = TextAlignment.Center;

				Run conditionalHeader2Run = new Run("tú");

				conditionalHeader2Run.FontSize = 12D;

				conditionalHeader2TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader2Run)));

				conditionalHeaderTableRow.Cells.Add(conditionalHeader2TableCell);

				TableCell conditionalHeader3TableCell = new TableCell();

				conditionalHeader3TableCell.BorderBrush = Brushes.Black;
				conditionalHeader3TableCell.BorderThickness = tableBorderThickness;
				conditionalHeader3TableCell.Padding = tableTableCellPaddingThickness;
				conditionalHeader3TableCell.TextAlignment = TextAlignment.Center;

				Run conditionalHeader3Run = new Run("él/ella/ello");

				conditionalHeader3Run.FontSize = 12D;

				conditionalHeader3TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader3Run)));

				conditionalHeaderTableRow.Cells.Add(conditionalHeader3TableCell);

				TableCell conditionalHeader4TableCell = new TableCell();

				conditionalHeader4TableCell.BorderBrush = Brushes.Black;
				conditionalHeader4TableCell.BorderThickness = tableBorderThickness;
				conditionalHeader4TableCell.Padding = tableTableCellPaddingThickness;
				conditionalHeader4TableCell.TextAlignment = TextAlignment.Center;

				Run conditionalHeader4Run = new Run("nosotros/nosotras");

				conditionalHeader4Run.FontSize = 12D;

				conditionalHeader4TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader4Run)));

				conditionalHeaderTableRow.Cells.Add(conditionalHeader4TableCell);

				TableCell conditionalHeader5TableCell = new TableCell();

				conditionalHeader5TableCell.BorderBrush = Brushes.Black;
				conditionalHeader5TableCell.BorderThickness = tableBorderThickness;
				conditionalHeader5TableCell.Padding = tableTableCellPaddingThickness;
				conditionalHeader5TableCell.TextAlignment = TextAlignment.Center;

				Run conditionalHeader5Run = new Run("vosotros/vosotras");

				conditionalHeader5Run.FontSize = 12D;

				conditionalHeader5TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader5Run)));

				conditionalHeaderTableRow.Cells.Add(conditionalHeader5TableCell);

				TableCell conditionalHeader6TableCell = new TableCell();

				conditionalHeader6TableCell.BorderBrush = Brushes.Black;
				conditionalHeader6TableCell.BorderThickness = tableBorderThickness;
				conditionalHeader6TableCell.Padding = tableTableCellPaddingThickness;
				conditionalHeader6TableCell.TextAlignment = TextAlignment.Center;

				Run conditionalHeader6Run = new Run("ellos/ellas");

				conditionalHeader6Run.FontSize = 12D;

				conditionalHeader6TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader6Run)));

				conditionalHeaderTableRow.Cells.Add(conditionalHeader6TableCell);

				conditionalTableTableRowGroup.Rows.Add(conditionalHeaderTableRow);

				/*
				 * Conditional | Row: conditional
				 */

				TableRow conditionalTableRow = new TableRow();

				TableCell conditional1TableCell = new TableCell();

				conditional1TableCell.BorderBrush = Brushes.Black;
				conditional1TableCell.BorderThickness = tableBorderThickness;
				conditional1TableCell.Padding = tableTableCellPaddingThickness;

				Run conditional1Run = new Run("infinitive + ía");

				conditional1Run.FontSize = 12D;

				conditional1TableCell.Blocks.Add(new Paragraph(conditional1Run));

				conditionalTableRow.Cells.Add(conditional1TableCell);

				TableCell conditional2TableCell = new TableCell();

				conditional2TableCell.BorderBrush = Brushes.Black;
				conditional2TableCell.BorderThickness = tableBorderThickness;
				conditional2TableCell.Padding = tableTableCellPaddingThickness;

				Run conditional2Run = new Run("infinitive + ías");

				conditional2Run.FontSize = 12D;

				conditional2TableCell.Blocks.Add(new Paragraph(conditional2Run));

				conditionalTableRow.Cells.Add(conditional2TableCell);

				TableCell conditional3TableCell = new TableCell();

				conditional3TableCell.BorderBrush = Brushes.Black;
				conditional3TableCell.BorderThickness = tableBorderThickness;
				conditional3TableCell.Padding = tableTableCellPaddingThickness;

				Run conditional3Run = new Run("infinitive + ía");

				conditional3Run.FontSize = 12D;

				conditional3TableCell.Blocks.Add(new Paragraph(conditional3Run));

				conditionalTableRow.Cells.Add(conditional3TableCell);

				TableCell conditional4TableCell = new TableCell();

				conditional4TableCell.BorderBrush = Brushes.Black;
				conditional4TableCell.BorderThickness = tableBorderThickness;
				conditional4TableCell.Padding = tableTableCellPaddingThickness;

				Run conditional4Run = new Run("infinitive + íamos");

				conditional4Run.FontSize = 12D;

				conditional4TableCell.Blocks.Add(new Paragraph(conditional4Run));

				conditionalTableRow.Cells.Add(conditional4TableCell);

				TableCell conditional5TableCell = new TableCell();

				conditional5TableCell.BorderBrush = Brushes.Black;
				conditional5TableCell.BorderThickness = tableBorderThickness;
				conditional5TableCell.Padding = tableTableCellPaddingThickness;

				Run conditional5Run = new Run("infinitive + íais");

				conditional5Run.FontSize = 12D;

				conditional5TableCell.Blocks.Add(new Paragraph(conditional5Run));

				conditionalTableRow.Cells.Add(conditional5TableCell);

				TableCell conditional6TableCell = new TableCell();

				conditional6TableCell.BorderBrush = Brushes.Black;
				conditional6TableCell.BorderThickness = tableBorderThickness;
				conditional6TableCell.Padding = tableTableCellPaddingThickness;

				Run conditional6Run = new Run("infinitive + ían");

				conditional6Run.FontSize = 12D;

				conditional6TableCell.Blocks.Add(new Paragraph(conditional6Run));

				conditionalTableRow.Cells.Add(conditional6TableCell);

				conditionalTableTableRowGroup.Rows.Add(conditionalTableRow);

				/*
				 * Conditional | RowGroup to Table and Table to FlowDocument.
				 */

				conditionalTable.RowGroups.Add(conditionalTableTableRowGroup);

				this.tableFlowDocument.Blocks.Add(conditionalTable);
			}
			else
			{
				try
				{
					Conjugation conjugation = Conjugator.Conjugate(new Verb(this.verbsListBox.SelectedItem.ToString()));

					SolidColorBrush irregularInflectionSolidColorBrush = new SolidColorBrush(Color.FromArgb(255, (byte)204, (byte)216, (byte)255));

					/*
					 * Forms
					 */

					Paragraph formsParagraph = new Paragraph();

					formsParagraph.Inlines.Add(new Bold(new Run("Forms")));

					this.tableFlowDocument.Blocks.Add(formsParagraph);

					Table formsTable = new Table();

					formsTable.BorderBrush = Brushes.Black;
					formsTable.BorderThickness = tableBorderThickness;
					formsTable.CellSpacing = 0D;

					TableRowGroup formsTableTableRowGroup = new TableRowGroup();

					/*
					 * Forms | Row: header
					 */

					TableRow formsHeaderTableRow = new TableRow();

					formsHeaderTableRow.Background = Brushes.LightGray;

					TableCell infinitiveTableCell = new TableCell();

					infinitiveTableCell.BorderBrush = Brushes.Black;
					infinitiveTableCell.BorderThickness = tableBorderThickness;
					infinitiveTableCell.Padding = tableTableCellPaddingThickness;
					infinitiveTableCell.TextAlignment = TextAlignment.Center;

					Run infinitiveRun = new Run("Infinitive");

					infinitiveRun.FontSize = 12D;

					infinitiveTableCell.Blocks.Add(new Paragraph(new Bold(infinitiveRun)));

					formsHeaderTableRow.Cells.Add(infinitiveTableCell);

					TableCell gerundTableCell = new TableCell();

					gerundTableCell.BorderBrush = Brushes.Black;
					gerundTableCell.BorderThickness = tableBorderThickness;
					gerundTableCell.Padding = tableTableCellPaddingThickness;
					gerundTableCell.TextAlignment = TextAlignment.Center;

					Run gerundRun = new Run("Gerund");

					gerundRun.FontSize = 12D;

					gerundTableCell.Blocks.Add(new Paragraph(new Bold(gerundRun)));

					formsHeaderTableRow.Cells.Add(gerundTableCell);

					TableCell pastParticipleTableCell = new TableCell();

					pastParticipleTableCell.BorderBrush = Brushes.Black;
					pastParticipleTableCell.BorderThickness = tableBorderThickness;
					pastParticipleTableCell.Padding = tableTableCellPaddingThickness;
					pastParticipleTableCell.TextAlignment = TextAlignment.Center;

					Run pastParticipleRun = new Run("Past participle");

					pastParticipleRun.FontSize = 12D;

					pastParticipleTableCell.Blocks.Add(new Paragraph(new Bold(pastParticipleRun)));

					formsHeaderTableRow.Cells.Add(pastParticipleTableCell);

					formsTableTableRowGroup.Rows.Add(formsHeaderTableRow);

					/*
					 * Forms | Row: forms
					 */

					TableRow formsFormsTableRow = new TableRow();

					TableCell forms1TableCell = new TableCell();

					forms1TableCell.BorderBrush = Brushes.Black;
					forms1TableCell.BorderThickness = tableBorderThickness;
					forms1TableCell.Padding = tableTableCellPaddingThickness;

					Run forms1Run = new Run(conjugation.Infinitive);

					forms1Run.FontSize = 12D;

					forms1TableCell.Blocks.Add(new Paragraph(forms1Run));

					formsFormsTableRow.Cells.Add(forms1TableCell);

					TableCell forms2TableCell = new TableCell();

					if (conjugation.Gerund.Inflection == Inflection.Irregular)
					{
						forms2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					forms2TableCell.BorderBrush = Brushes.Black;
					forms2TableCell.BorderThickness = tableBorderThickness;
					forms2TableCell.Padding = tableTableCellPaddingThickness;

					Run forms2Run = new Run(conjugation.Gerund.ToString());

					forms2Run.FontSize = 12D;

					forms2TableCell.Blocks.Add(new Paragraph(forms2Run));

					formsFormsTableRow.Cells.Add(forms2TableCell);

					formsTableTableRowGroup.Rows.Add(formsFormsTableRow);

					TableCell forms3TableCell = new TableCell();

					if (conjugation.PastParticiple.Inflection == Inflection.Irregular)
					{
						forms3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					forms3TableCell.BorderBrush = Brushes.Black;
					forms3TableCell.BorderThickness = tableBorderThickness;
					forms3TableCell.Padding = tableTableCellPaddingThickness;

					Run forms3Run = new Run(conjugation.PastParticiple.ToString());

					forms3Run.FontSize = 12D;

					forms3TableCell.Blocks.Add(new Paragraph(forms3Run));

					formsFormsTableRow.Cells.Add(forms3TableCell);

					/*
					 * Forms | RowGroup to Table and Table to FlowDocument.
					 */

					formsTable.RowGroups.Add(formsTableTableRowGroup);

					this.tableFlowDocument.Blocks.Add(formsTable);

					/*
					 * Indicative
					 */

					Paragraph indicativeParagraph = new Paragraph();

					indicativeParagraph.Inlines.Add(new Bold(new Run("Indicative")));

					this.tableFlowDocument.Blocks.Add(indicativeParagraph);

					Table indicativeTable = new Table();

					indicativeTable.BorderBrush = Brushes.Black;
					indicativeTable.BorderThickness = tableBorderThickness;
					indicativeTable.CellSpacing = 0D;

					TableRowGroup indicativeTableTableRowGroup = new TableRowGroup();

					/*
					 * Indicative | Row: header
					 */

					TableRow indicativeHeaderTableRow = new TableRow();

					indicativeHeaderTableRow.Background = Brushes.LightGray;

					TableCell indicativeHeader1TableCell = new TableCell();

					indicativeHeader1TableCell.BorderBrush = Brushes.Black;
					indicativeHeader1TableCell.BorderThickness = tableBorderThickness;
					indicativeHeader1TableCell.Padding = tableTableCellPaddingThickness;
					indicativeHeader1TableCell.TextAlignment = TextAlignment.Center;

					Run indicativeHeader1Run = new Run(string.Empty);

					indicativeHeader1Run.FontSize = 12D;

					indicativeHeader1TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader1Run)));

					indicativeHeaderTableRow.Cells.Add(indicativeHeader1TableCell);

					TableCell indicativeHeader2TableCell = new TableCell();

					indicativeHeader2TableCell.BorderBrush = Brushes.Black;
					indicativeHeader2TableCell.BorderThickness = tableBorderThickness;
					indicativeHeader2TableCell.Padding = tableTableCellPaddingThickness;
					indicativeHeader2TableCell.TextAlignment = TextAlignment.Center;

					Run indicativeHeader2Run = new Run("yo");

					indicativeHeader2Run.FontSize = 12D;

					indicativeHeader2TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader2Run)));

					indicativeHeaderTableRow.Cells.Add(indicativeHeader2TableCell);

					TableCell indicativeHeader3TableCell = new TableCell();

					indicativeHeader3TableCell.BorderBrush = Brushes.Black;
					indicativeHeader3TableCell.BorderThickness = tableBorderThickness;
					indicativeHeader3TableCell.Padding = tableTableCellPaddingThickness;
					indicativeHeader3TableCell.TextAlignment = TextAlignment.Center;

					Run indicativeHeader3Run = new Run("tú");

					indicativeHeader3Run.FontSize = 12D;

					indicativeHeader3TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader3Run)));

					indicativeHeaderTableRow.Cells.Add(indicativeHeader3TableCell);

					TableCell indicativeHeader4TableCell = new TableCell();

					indicativeHeader4TableCell.BorderBrush = Brushes.Black;
					indicativeHeader4TableCell.BorderThickness = tableBorderThickness;
					indicativeHeader4TableCell.Padding = tableTableCellPaddingThickness;
					indicativeHeader4TableCell.TextAlignment = TextAlignment.Center;

					Run indicativeHeader4Run = new Run("él/ella/ello");

					indicativeHeader4Run.FontSize = 12D;

					indicativeHeader4TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader4Run)));

					indicativeHeaderTableRow.Cells.Add(indicativeHeader4TableCell);

					TableCell indicativeHeader5TableCell = new TableCell();

					indicativeHeader5TableCell.BorderBrush = Brushes.Black;
					indicativeHeader5TableCell.BorderThickness = tableBorderThickness;
					indicativeHeader5TableCell.Padding = tableTableCellPaddingThickness;
					indicativeHeader5TableCell.TextAlignment = TextAlignment.Center;

					Run indicativeHeader5Run = new Run("nosotros/nosotras");

					indicativeHeader5Run.FontSize = 12D;

					indicativeHeader5TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader5Run)));

					indicativeHeaderTableRow.Cells.Add(indicativeHeader5TableCell);

					TableCell indicativeHeader6TableCell = new TableCell();

					indicativeHeader6TableCell.BorderBrush = Brushes.Black;
					indicativeHeader6TableCell.BorderThickness = tableBorderThickness;
					indicativeHeader6TableCell.Padding = tableTableCellPaddingThickness;
					indicativeHeader6TableCell.TextAlignment = TextAlignment.Center;

					Run indicativeHeader6Run = new Run("vosotros/vosotras");

					indicativeHeader6Run.FontSize = 12D;

					indicativeHeader6TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader6Run)));

					indicativeHeaderTableRow.Cells.Add(indicativeHeader6TableCell);

					TableCell indicativeHeader7TableCell = new TableCell();

					indicativeHeader7TableCell.BorderBrush = Brushes.Black;
					indicativeHeader7TableCell.BorderThickness = tableBorderThickness;
					indicativeHeader7TableCell.Padding = tableTableCellPaddingThickness;
					indicativeHeader7TableCell.TextAlignment = TextAlignment.Center;

					Run indicativeHeader7Run = new Run("ellos/ellas");

					indicativeHeader7Run.FontSize = 12D;

					indicativeHeader7TableCell.Blocks.Add(new Paragraph(new Bold(indicativeHeader7Run)));

					indicativeHeaderTableRow.Cells.Add(indicativeHeader7TableCell);

					indicativeTableTableRowGroup.Rows.Add(indicativeHeaderTableRow);

					/*
					 * Indicative | Row: present
					 */

					TableRow indicativePresentTableRow = new TableRow();

					TableCell indicativePresent1TableCell = new TableCell();

					indicativePresent1TableCell.Background = Brushes.LightGray;
					indicativePresent1TableCell.BorderBrush = Brushes.Black;
					indicativePresent1TableCell.BorderThickness = tableBorderThickness;
					indicativePresent1TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePresent1Run = new Run("Present:");

					indicativePresent1Run.FontSize = 12D;

					indicativePresent1TableCell.Blocks.Add(new Paragraph(new Bold(indicativePresent1Run)));

					indicativePresentTableRow.Cells.Add(indicativePresent1TableCell);

					TableCell indicativePresent2TableCell = new TableCell();

					if (conjugation.PresentIndicative.FirstPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativePresent2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePresent2TableCell.BorderBrush = Brushes.Black;
					indicativePresent2TableCell.BorderThickness = tableBorderThickness;
					indicativePresent2TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePresent2Run = new Run(conjugation.PresentIndicative.FirstPersonSingular.ToString());

					indicativePresent2Run.FontSize = 12D;

					indicativePresent2TableCell.Blocks.Add(new Paragraph(indicativePresent2Run));

					indicativePresentTableRow.Cells.Add(indicativePresent2TableCell);

					TableCell indicativePresent3TableCell = new TableCell();

					if (conjugation.PresentIndicative.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativePresent3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePresent3TableCell.BorderBrush = Brushes.Black;
					indicativePresent3TableCell.BorderThickness = tableBorderThickness;
					indicativePresent3TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePresent3Run = new Run(conjugation.PresentIndicative.SecondPersonSingular.ToString());

					indicativePresent3Run.FontSize = 12D;

					indicativePresent3TableCell.Blocks.Add(new Paragraph(indicativePresent3Run));

					indicativePresentTableRow.Cells.Add(indicativePresent3TableCell);

					TableCell indicativePresent4TableCell = new TableCell();

					if (conjugation.PresentIndicative.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativePresent4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePresent4TableCell.BorderBrush = Brushes.Black;
					indicativePresent4TableCell.BorderThickness = tableBorderThickness;
					indicativePresent4TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePresent4Run = new Run(conjugation.PresentIndicative.ThirdPersonSingular.ToString());

					indicativePresent4Run.FontSize = 12D;

					indicativePresent4TableCell.Blocks.Add(new Paragraph(indicativePresent4Run));

					indicativePresentTableRow.Cells.Add(indicativePresent4TableCell);

					TableCell indicativePresent5TableCell = new TableCell();

					if (conjugation.PresentIndicative.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativePresent5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePresent5TableCell.BorderBrush = Brushes.Black;
					indicativePresent5TableCell.BorderThickness = tableBorderThickness;
					indicativePresent5TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePresent5Run = new Run(conjugation.PresentIndicative.FirstPersonPlural.ToString());

					indicativePresent5Run.FontSize = 12D;

					indicativePresent5TableCell.Blocks.Add(new Paragraph(indicativePresent5Run));

					indicativePresentTableRow.Cells.Add(indicativePresent5TableCell);

					TableCell indicativePresent6TableCell = new TableCell();

					if (conjugation.PresentIndicative.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativePresent6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePresent6TableCell.BorderBrush = Brushes.Black;
					indicativePresent6TableCell.BorderThickness = tableBorderThickness;
					indicativePresent6TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePresent6Run = new Run(conjugation.PresentIndicative.SecondPersonPlural.ToString());

					indicativePresent6Run.FontSize = 12D;

					indicativePresent6TableCell.Blocks.Add(new Paragraph(indicativePresent6Run));

					indicativePresentTableRow.Cells.Add(indicativePresent6TableCell);

					TableCell indicativePresent7TableCell = new TableCell();

					if (conjugation.PresentIndicative.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativePresent7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePresent7TableCell.BorderBrush = Brushes.Black;
					indicativePresent7TableCell.BorderThickness = tableBorderThickness;
					indicativePresent7TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePresent7Run = new Run(conjugation.PresentIndicative.ThirdPersonPlural.ToString());

					indicativePresent7Run.FontSize = 12D;

					indicativePresent7TableCell.Blocks.Add(new Paragraph(indicativePresent7Run));

					indicativePresentTableRow.Cells.Add(indicativePresent7TableCell);

					indicativeTableTableRowGroup.Rows.Add(indicativePresentTableRow);

					/*
					 * Indicative | Row: imperfect
					 */

					TableRow indicativeImperfectTableRow = new TableRow();

					TableCell indicativeImperfect1TableCell = new TableCell();

					indicativeImperfect1TableCell.Background = Brushes.LightGray;
					indicativeImperfect1TableCell.BorderBrush = Brushes.Black;
					indicativeImperfect1TableCell.BorderThickness = tableBorderThickness;
					indicativeImperfect1TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeImperfect1Run = new Run("Imperfect:");

					indicativeImperfect1Run.FontSize = 12D;

					indicativeImperfect1TableCell.Blocks.Add(new Paragraph(new Bold(indicativeImperfect1Run)));

					indicativeImperfectTableRow.Cells.Add(indicativeImperfect1TableCell);

					TableCell indicativeImperfect2TableCell = new TableCell();

					if (conjugation.ImperfectIndicative.FirstPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativeImperfect2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeImperfect2TableCell.BorderBrush = Brushes.Black;
					indicativeImperfect2TableCell.BorderThickness = tableBorderThickness;
					indicativeImperfect2TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeImperfect2Run = new Run(conjugation.ImperfectIndicative.FirstPersonSingular.ToString());

					indicativeImperfect2Run.FontSize = 12D;

					indicativeImperfect2TableCell.Blocks.Add(new Paragraph(indicativeImperfect2Run));

					indicativeImperfectTableRow.Cells.Add(indicativeImperfect2TableCell);

					TableCell indicativeImperfect3TableCell = new TableCell();

					if (conjugation.ImperfectIndicative.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativeImperfect3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeImperfect3TableCell.BorderBrush = Brushes.Black;
					indicativeImperfect3TableCell.BorderThickness = tableBorderThickness;
					indicativeImperfect3TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeImperfect3Run = new Run(conjugation.ImperfectIndicative.SecondPersonSingular.ToString());

					indicativeImperfect3Run.FontSize = 12D;

					indicativeImperfect3TableCell.Blocks.Add(new Paragraph(indicativeImperfect3Run));

					indicativeImperfectTableRow.Cells.Add(indicativeImperfect3TableCell);

					TableCell indicativeImperfect4TableCell = new TableCell();

					if (conjugation.ImperfectIndicative.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativeImperfect4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeImperfect4TableCell.BorderBrush = Brushes.Black;
					indicativeImperfect4TableCell.BorderThickness = tableBorderThickness;
					indicativeImperfect4TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeImperfect4Run = new Run(conjugation.ImperfectIndicative.ThirdPersonSingular.ToString());

					indicativeImperfect4Run.FontSize = 12D;

					indicativeImperfect4TableCell.Blocks.Add(new Paragraph(indicativeImperfect4Run));

					indicativeImperfectTableRow.Cells.Add(indicativeImperfect4TableCell);

					TableCell indicativeImperfect5TableCell = new TableCell();

					if (conjugation.ImperfectIndicative.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativeImperfect5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeImperfect5TableCell.BorderBrush = Brushes.Black;
					indicativeImperfect5TableCell.BorderThickness = tableBorderThickness;
					indicativeImperfect5TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeImperfect5Run = new Run(conjugation.ImperfectIndicative.FirstPersonPlural.ToString());

					indicativeImperfect5Run.FontSize = 12D;

					indicativeImperfect5TableCell.Blocks.Add(new Paragraph(indicativeImperfect5Run));

					indicativeImperfectTableRow.Cells.Add(indicativeImperfect5TableCell);

					TableCell indicativeImperfect6TableCell = new TableCell();

					if (conjugation.ImperfectIndicative.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativeImperfect6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeImperfect6TableCell.BorderBrush = Brushes.Black;
					indicativeImperfect6TableCell.BorderThickness = tableBorderThickness;
					indicativeImperfect6TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeImperfect6Run = new Run(conjugation.ImperfectIndicative.SecondPersonPlural.ToString());

					indicativeImperfect6Run.FontSize = 12D;

					indicativeImperfect6TableCell.Blocks.Add(new Paragraph(indicativeImperfect6Run));

					indicativeImperfectTableRow.Cells.Add(indicativeImperfect6TableCell);

					TableCell indicativeImperfect7TableCell = new TableCell();

					if (conjugation.ImperfectIndicative.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativeImperfect7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeImperfect7TableCell.BorderBrush = Brushes.Black;
					indicativeImperfect7TableCell.BorderThickness = tableBorderThickness;
					indicativeImperfect7TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeImperfect7Run = new Run(conjugation.ImperfectIndicative.ThirdPersonPlural.ToString());

					indicativeImperfect7Run.FontSize = 12D;

					indicativeImperfect7TableCell.Blocks.Add(new Paragraph(indicativeImperfect7Run));

					indicativeImperfectTableRow.Cells.Add(indicativeImperfect7TableCell);

					indicativeTableTableRowGroup.Rows.Add(indicativeImperfectTableRow);

					/*
					 * Indicative | Row: preterite
					 */

					TableRow indicativePreteriteTableRow = new TableRow();

					TableCell indicativePreterite1TableCell = new TableCell();

					indicativePreterite1TableCell.Background = Brushes.LightGray;
					indicativePreterite1TableCell.BorderBrush = Brushes.Black;
					indicativePreterite1TableCell.BorderThickness = tableBorderThickness;
					indicativePreterite1TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePreterite1Run = new Run("Preterite:");

					indicativePreterite1Run.FontSize = 12D;

					indicativePreterite1TableCell.Blocks.Add(new Paragraph(new Bold(indicativePreterite1Run)));

					indicativePreteriteTableRow.Cells.Add(indicativePreterite1TableCell);

					TableCell indicativePreterite2TableCell = new TableCell();

					if (conjugation.PreteriteIndicative.FirstPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativePreterite2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePreterite2TableCell.BorderBrush = Brushes.Black;
					indicativePreterite2TableCell.BorderThickness = tableBorderThickness;
					indicativePreterite2TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePreterite2Run = new Run(conjugation.PreteriteIndicative.FirstPersonSingular.ToString());

					indicativePreterite2Run.FontSize = 12D;

					indicativePreterite2TableCell.Blocks.Add(new Paragraph(indicativePreterite2Run));

					indicativePreteriteTableRow.Cells.Add(indicativePreterite2TableCell);

					TableCell indicativePreterite3TableCell = new TableCell();

					if (conjugation.PreteriteIndicative.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativePreterite3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePreterite3TableCell.BorderBrush = Brushes.Black;
					indicativePreterite3TableCell.BorderThickness = tableBorderThickness;
					indicativePreterite3TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePreterite3Run = new Run(conjugation.PreteriteIndicative.SecondPersonSingular.ToString());

					indicativePreterite3Run.FontSize = 12D;

					indicativePreterite3TableCell.Blocks.Add(new Paragraph(indicativePreterite3Run));

					indicativePreteriteTableRow.Cells.Add(indicativePreterite3TableCell);

					TableCell indicativePreterite4TableCell = new TableCell();

					if (conjugation.PreteriteIndicative.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativePreterite4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePreterite4TableCell.BorderBrush = Brushes.Black;
					indicativePreterite4TableCell.BorderThickness = tableBorderThickness;
					indicativePreterite4TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePreterite4Run = new Run(conjugation.PreteriteIndicative.ThirdPersonSingular.ToString());

					indicativePreterite4Run.FontSize = 12D;

					indicativePreterite4TableCell.Blocks.Add(new Paragraph(indicativePreterite4Run));

					indicativePreteriteTableRow.Cells.Add(indicativePreterite4TableCell);

					TableCell indicativePreterite5TableCell = new TableCell();

					if (conjugation.PreteriteIndicative.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativePreterite5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePreterite5TableCell.BorderBrush = Brushes.Black;
					indicativePreterite5TableCell.BorderThickness = tableBorderThickness;
					indicativePreterite5TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePreterite5Run = new Run(conjugation.PreteriteIndicative.FirstPersonPlural.ToString());

					indicativePreterite5Run.FontSize = 12D;

					indicativePreterite5TableCell.Blocks.Add(new Paragraph(indicativePreterite5Run));

					indicativePreteriteTableRow.Cells.Add(indicativePreterite5TableCell);

					TableCell indicativePreterite6TableCell = new TableCell();

					if (conjugation.PreteriteIndicative.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativePreterite6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePreterite6TableCell.BorderBrush = Brushes.Black;
					indicativePreterite6TableCell.BorderThickness = tableBorderThickness;
					indicativePreterite6TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePreterite6Run = new Run(conjugation.PreteriteIndicative.SecondPersonPlural.ToString());

					indicativePreterite6Run.FontSize = 12D;

					indicativePreterite6TableCell.Blocks.Add(new Paragraph(indicativePreterite6Run));

					indicativePreteriteTableRow.Cells.Add(indicativePreterite6TableCell);

					TableCell indicativePreterite7TableCell = new TableCell();

					if (conjugation.PreteriteIndicative.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativePreterite7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativePreterite7TableCell.BorderBrush = Brushes.Black;
					indicativePreterite7TableCell.BorderThickness = tableBorderThickness;
					indicativePreterite7TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativePreterite7Run = new Run(conjugation.PreteriteIndicative.ThirdPersonPlural.ToString());

					indicativePreterite7Run.FontSize = 12D;

					indicativePreterite7TableCell.Blocks.Add(new Paragraph(indicativePreterite7Run));

					indicativePreteriteTableRow.Cells.Add(indicativePreterite7TableCell);

					indicativeTableTableRowGroup.Rows.Add(indicativePreteriteTableRow);

					/*
					 * Indicative | Row: future
					 */

					TableRow indicativeFutureTableRow = new TableRow();

					TableCell indicativeFuture1TableCell = new TableCell();

					indicativeFuture1TableCell.Background = Brushes.LightGray;
					indicativeFuture1TableCell.BorderBrush = Brushes.Black;
					indicativeFuture1TableCell.BorderThickness = tableBorderThickness;
					indicativeFuture1TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeFuture1Run = new Run("Future:");

					indicativeFuture1Run.FontSize = 12D;

					indicativeFuture1TableCell.Blocks.Add(new Paragraph(new Bold(indicativeFuture1Run)));

					indicativeFutureTableRow.Cells.Add(indicativeFuture1TableCell);

					TableCell indicativeFuture2TableCell = new TableCell();

					if (conjugation.FutureIndicative.FirstPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativeFuture2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeFuture2TableCell.BorderBrush = Brushes.Black;
					indicativeFuture2TableCell.BorderThickness = tableBorderThickness;
					indicativeFuture2TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeFuture2Run = new Run(conjugation.FutureIndicative.FirstPersonSingular.ToString());

					indicativeFuture2Run.FontSize = 12D;

					indicativeFuture2TableCell.Blocks.Add(new Paragraph(indicativeFuture2Run));

					indicativeFutureTableRow.Cells.Add(indicativeFuture2TableCell);

					TableCell indicativeFuture3TableCell = new TableCell();

					if (conjugation.FutureIndicative.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativeFuture3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeFuture3TableCell.BorderBrush = Brushes.Black;
					indicativeFuture3TableCell.BorderThickness = tableBorderThickness;
					indicativeFuture3TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeFuture3Run = new Run(conjugation.FutureIndicative.SecondPersonSingular.ToString());

					indicativeFuture3Run.FontSize = 12D;

					indicativeFuture3TableCell.Blocks.Add(new Paragraph(indicativeFuture3Run));

					indicativeFutureTableRow.Cells.Add(indicativeFuture3TableCell);

					TableCell indicativeFuture4TableCell = new TableCell();

					if (conjugation.FutureIndicative.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						indicativeFuture4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeFuture4TableCell.BorderBrush = Brushes.Black;
					indicativeFuture4TableCell.BorderThickness = tableBorderThickness;
					indicativeFuture4TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeFuture4Run = new Run(conjugation.FutureIndicative.ThirdPersonSingular.ToString());

					indicativeFuture4Run.FontSize = 12D;

					indicativeFuture4TableCell.Blocks.Add(new Paragraph(indicativeFuture4Run));

					indicativeFutureTableRow.Cells.Add(indicativeFuture4TableCell);

					TableCell indicativeFuture5TableCell = new TableCell();

					if (conjugation.FutureIndicative.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativeFuture5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeFuture5TableCell.BorderBrush = Brushes.Black;
					indicativeFuture5TableCell.BorderThickness = tableBorderThickness;
					indicativeFuture5TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeFuture5Run = new Run(conjugation.FutureIndicative.FirstPersonPlural.ToString());

					indicativeFuture5Run.FontSize = 12D;

					indicativeFuture5TableCell.Blocks.Add(new Paragraph(indicativeFuture5Run));

					indicativeFutureTableRow.Cells.Add(indicativeFuture5TableCell);

					TableCell indicativeFuture6TableCell = new TableCell();

					if (conjugation.FutureIndicative.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativeFuture6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeFuture6TableCell.BorderBrush = Brushes.Black;
					indicativeFuture6TableCell.BorderThickness = tableBorderThickness;
					indicativeFuture6TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeFuture6Run = new Run(conjugation.FutureIndicative.SecondPersonPlural.ToString());

					indicativeFuture6Run.FontSize = 12D;

					indicativeFuture6TableCell.Blocks.Add(new Paragraph(indicativeFuture6Run));

					indicativeFutureTableRow.Cells.Add(indicativeFuture6TableCell);

					TableCell indicativeFuture7TableCell = new TableCell();

					if (conjugation.FutureIndicative.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						indicativeFuture7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					indicativeFuture7TableCell.BorderBrush = Brushes.Black;
					indicativeFuture7TableCell.BorderThickness = tableBorderThickness;
					indicativeFuture7TableCell.Padding = tableTableCellPaddingThickness;

					Run indicativeFuture7Run = new Run(conjugation.FutureIndicative.ThirdPersonPlural.ToString());

					indicativeFuture7Run.FontSize = 12D;

					indicativeFuture7TableCell.Blocks.Add(new Paragraph(indicativeFuture7Run));

					indicativeFutureTableRow.Cells.Add(indicativeFuture7TableCell);

					indicativeTableTableRowGroup.Rows.Add(indicativeFutureTableRow);

					/*
					 * Indicative | RowGroup to Table and Table to FlowDocument.
					 */

					indicativeTable.RowGroups.Add(indicativeTableTableRowGroup);

					this.tableFlowDocument.Blocks.Add(indicativeTable);

					/*
					 * Subjunctive
					 */

					Paragraph subjunctiveParagraph = new Paragraph();

					subjunctiveParagraph.Inlines.Add(new Bold(new Run("Subjunctive")));

					this.tableFlowDocument.Blocks.Add(subjunctiveParagraph);

					Table subjunctiveTable = new Table();

					subjunctiveTable.BorderBrush = Brushes.Black;
					subjunctiveTable.BorderThickness = tableBorderThickness;
					subjunctiveTable.CellSpacing = 0D;

					TableRowGroup subjunctiveTableTableRowGroup = new TableRowGroup();

					/*
					 * Subjunctive | Row: header
					 */

					TableRow subjunctiveHeaderTableRow = new TableRow();

					subjunctiveHeaderTableRow.Background = Brushes.LightGray;

					TableCell subjunctiveHeader1TableCell = new TableCell();

					subjunctiveHeader1TableCell.BorderBrush = Brushes.Black;
					subjunctiveHeader1TableCell.BorderThickness = tableBorderThickness;
					subjunctiveHeader1TableCell.Padding = tableTableCellPaddingThickness;
					subjunctiveHeader1TableCell.TextAlignment = TextAlignment.Center;

					Run subjunctiveHeader1Run = new Run(string.Empty);

					subjunctiveHeader1Run.FontSize = 12D;

					subjunctiveHeader1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader1Run)));

					subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader1TableCell);

					TableCell subjunctiveHeader2TableCell = new TableCell();

					subjunctiveHeader2TableCell.BorderBrush = Brushes.Black;
					subjunctiveHeader2TableCell.BorderThickness = tableBorderThickness;
					subjunctiveHeader2TableCell.Padding = tableTableCellPaddingThickness;
					subjunctiveHeader2TableCell.TextAlignment = TextAlignment.Center;

					Run subjunctiveHeader2Run = new Run("yo");

					subjunctiveHeader2Run.FontSize = 12D;

					subjunctiveHeader2TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader2Run)));

					subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader2TableCell);

					TableCell subjunctiveHeader3TableCell = new TableCell();

					subjunctiveHeader3TableCell.BorderBrush = Brushes.Black;
					subjunctiveHeader3TableCell.BorderThickness = tableBorderThickness;
					subjunctiveHeader3TableCell.Padding = tableTableCellPaddingThickness;
					subjunctiveHeader3TableCell.TextAlignment = TextAlignment.Center;

					Run subjunctiveHeader3Run = new Run("tú");

					subjunctiveHeader3Run.FontSize = 12D;

					subjunctiveHeader3TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader3Run)));

					subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader3TableCell);

					TableCell subjunctiveHeader4TableCell = new TableCell();

					subjunctiveHeader4TableCell.BorderBrush = Brushes.Black;
					subjunctiveHeader4TableCell.BorderThickness = tableBorderThickness;
					subjunctiveHeader4TableCell.Padding = tableTableCellPaddingThickness;
					subjunctiveHeader4TableCell.TextAlignment = TextAlignment.Center;

					Run subjunctiveHeader4Run = new Run("él/ella/ello");

					subjunctiveHeader4Run.FontSize = 12D;

					subjunctiveHeader4TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader4Run)));

					subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader4TableCell);

					TableCell subjunctiveHeader5TableCell = new TableCell();

					subjunctiveHeader5TableCell.BorderBrush = Brushes.Black;
					subjunctiveHeader5TableCell.BorderThickness = tableBorderThickness;
					subjunctiveHeader5TableCell.Padding = tableTableCellPaddingThickness;
					subjunctiveHeader5TableCell.TextAlignment = TextAlignment.Center;

					Run subjunctiveHeader5Run = new Run("nosotros/nosotras");

					subjunctiveHeader5Run.FontSize = 12D;

					subjunctiveHeader5TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader5Run)));

					subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader5TableCell);

					TableCell subjunctiveHeader6TableCell = new TableCell();

					subjunctiveHeader6TableCell.BorderBrush = Brushes.Black;
					subjunctiveHeader6TableCell.BorderThickness = tableBorderThickness;
					subjunctiveHeader6TableCell.Padding = tableTableCellPaddingThickness;
					subjunctiveHeader6TableCell.TextAlignment = TextAlignment.Center;

					Run subjunctiveHeader6Run = new Run("vosotros/vosotras");

					subjunctiveHeader6Run.FontSize = 12D;

					subjunctiveHeader6TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader6Run)));

					subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader6TableCell);

					TableCell subjunctiveHeader7TableCell = new TableCell();

					subjunctiveHeader7TableCell.BorderBrush = Brushes.Black;
					subjunctiveHeader7TableCell.BorderThickness = tableBorderThickness;
					subjunctiveHeader7TableCell.Padding = tableTableCellPaddingThickness;
					subjunctiveHeader7TableCell.TextAlignment = TextAlignment.Center;

					Run subjunctiveHeader7Run = new Run("ellos/ellas");

					subjunctiveHeader7Run.FontSize = 12D;

					subjunctiveHeader7TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveHeader7Run)));

					subjunctiveHeaderTableRow.Cells.Add(subjunctiveHeader7TableCell);

					subjunctiveTableTableRowGroup.Rows.Add(subjunctiveHeaderTableRow);

					/*
					 * Subjunctive | Row: present
					 */

					TableRow subjunctivePresentTableRow = new TableRow();

					TableCell subjunctivePresent1TableCell = new TableCell();

					subjunctivePresent1TableCell.Background = Brushes.LightGray;
					subjunctivePresent1TableCell.BorderBrush = Brushes.Black;
					subjunctivePresent1TableCell.BorderThickness = tableBorderThickness;
					subjunctivePresent1TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctivePresent1Run = new Run("Present:");

					subjunctivePresent1Run.FontSize = 12D;

					subjunctivePresent1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctivePresent1Run)));

					subjunctivePresentTableRow.Cells.Add(subjunctivePresent1TableCell);

					TableCell subjunctivePresent2TableCell = new TableCell();

					if (conjugation.PresentSubjunctive.FirstPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctivePresent2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctivePresent2TableCell.BorderBrush = Brushes.Black;
					subjunctivePresent2TableCell.BorderThickness = tableBorderThickness;
					subjunctivePresent2TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctivePresent2Run = new Run(conjugation.PresentSubjunctive.FirstPersonSingular.ToString());

					subjunctivePresent2Run.FontSize = 12D;

					subjunctivePresent2TableCell.Blocks.Add(new Paragraph(subjunctivePresent2Run));

					subjunctivePresentTableRow.Cells.Add(subjunctivePresent2TableCell);

					TableCell subjunctivePresent3TableCell = new TableCell();

					if (conjugation.PresentSubjunctive.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctivePresent3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctivePresent3TableCell.BorderBrush = Brushes.Black;
					subjunctivePresent3TableCell.BorderThickness = tableBorderThickness;
					subjunctivePresent3TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctivePresent3Run = new Run(conjugation.PresentSubjunctive.SecondPersonSingular.ToString());

					subjunctivePresent3Run.FontSize = 12D;

					subjunctivePresent3TableCell.Blocks.Add(new Paragraph(subjunctivePresent3Run));

					subjunctivePresentTableRow.Cells.Add(subjunctivePresent3TableCell);

					TableCell subjunctivePresent4TableCell = new TableCell();

					if (conjugation.PresentSubjunctive.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctivePresent4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctivePresent4TableCell.BorderBrush = Brushes.Black;
					subjunctivePresent4TableCell.BorderThickness = tableBorderThickness;
					subjunctivePresent4TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctivePresent4Run = new Run(conjugation.PresentSubjunctive.ThirdPersonSingular.ToString());

					subjunctivePresent4Run.FontSize = 12D;

					subjunctivePresent4TableCell.Blocks.Add(new Paragraph(subjunctivePresent4Run));

					subjunctivePresentTableRow.Cells.Add(subjunctivePresent4TableCell);

					TableCell subjunctivePresent5TableCell = new TableCell();

					if (conjugation.PresentSubjunctive.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctivePresent5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctivePresent5TableCell.BorderBrush = Brushes.Black;
					subjunctivePresent5TableCell.BorderThickness = tableBorderThickness;
					subjunctivePresent5TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctivePresent5Run = new Run(conjugation.PresentSubjunctive.FirstPersonPlural.ToString());

					subjunctivePresent5Run.FontSize = 12D;

					subjunctivePresent5TableCell.Blocks.Add(new Paragraph(subjunctivePresent5Run));

					subjunctivePresentTableRow.Cells.Add(subjunctivePresent5TableCell);

					TableCell subjunctivePresent6TableCell = new TableCell();

					if (conjugation.PresentSubjunctive.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctivePresent6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctivePresent6TableCell.BorderBrush = Brushes.Black;
					subjunctivePresent6TableCell.BorderThickness = tableBorderThickness;
					subjunctivePresent6TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctivePresent6Run = new Run(conjugation.PresentSubjunctive.SecondPersonPlural.ToString());

					subjunctivePresent6Run.FontSize = 12D;

					subjunctivePresent6TableCell.Blocks.Add(new Paragraph(subjunctivePresent6Run));

					subjunctivePresentTableRow.Cells.Add(subjunctivePresent6TableCell);

					TableCell subjunctivePresent7TableCell = new TableCell();

					if (conjugation.PresentSubjunctive.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctivePresent7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctivePresent7TableCell.BorderBrush = Brushes.Black;
					subjunctivePresent7TableCell.BorderThickness = tableBorderThickness;
					subjunctivePresent7TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctivePresent7Run = new Run(conjugation.PresentSubjunctive.ThirdPersonPlural.ToString());

					subjunctivePresent7Run.FontSize = 12D;

					subjunctivePresent7TableCell.Blocks.Add(new Paragraph(subjunctivePresent7Run));

					subjunctivePresentTableRow.Cells.Add(subjunctivePresent7TableCell);

					subjunctiveTableTableRowGroup.Rows.Add(subjunctivePresentTableRow);

					/*
					 * Subjunctive | Row: imperfect (re)
					 */

					TableRow subjunctiveImperfectRaTableRow = new TableRow();

					TableCell subjunctiveImperfectRa1TableCell = new TableCell();

					subjunctiveImperfectRa1TableCell.Background = Brushes.LightGray;
					subjunctiveImperfectRa1TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectRa1TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectRa1TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectRa1Run = new Run("Imperfect (ra):");

					subjunctiveImperfectRa1Run.FontSize = 12D;

					subjunctiveImperfectRa1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveImperfectRa1Run)));

					subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa1TableCell);

					TableCell subjunctiveImperfectRa2TableCell = new TableCell();

					if (conjugation.ImperfectRaSubjunctive.FirstPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectRa2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectRa2TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectRa2TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectRa2TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectRa2Run = new Run(conjugation.ImperfectRaSubjunctive.FirstPersonSingular.ToString());

					subjunctiveImperfectRa2Run.FontSize = 12D;

					subjunctiveImperfectRa2TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa2Run));

					subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa2TableCell);

					TableCell subjunctiveImperfectRa3TableCell = new TableCell();

					if (conjugation.ImperfectRaSubjunctive.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectRa3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectRa3TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectRa3TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectRa3TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectRa3Run = new Run(conjugation.ImperfectRaSubjunctive.SecondPersonSingular.ToString());

					subjunctiveImperfectRa3Run.FontSize = 12D;

					subjunctiveImperfectRa3TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa3Run));

					subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa3TableCell);

					TableCell subjunctiveImperfectRa4TableCell = new TableCell();

					if (conjugation.ImperfectRaSubjunctive.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectRa4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectRa4TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectRa4TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectRa4TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectRa4Run = new Run(conjugation.ImperfectRaSubjunctive.ThirdPersonSingular.ToString());

					subjunctiveImperfectRa4Run.FontSize = 12D;

					subjunctiveImperfectRa4TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa4Run));

					subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa4TableCell);

					TableCell subjunctiveImperfectRa5TableCell = new TableCell();

					if (conjugation.ImperfectRaSubjunctive.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectRa5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectRa5TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectRa5TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectRa5TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectRa5Run = new Run(conjugation.ImperfectRaSubjunctive.FirstPersonPlural.ToString());

					subjunctiveImperfectRa5Run.FontSize = 12D;

					subjunctiveImperfectRa5TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa5Run));

					subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa5TableCell);

					TableCell subjunctiveImperfectRa6TableCell = new TableCell();

					if (conjugation.ImperfectRaSubjunctive.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectRa6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectRa6TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectRa6TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectRa6TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectRa6Run = new Run(conjugation.ImperfectRaSubjunctive.SecondPersonPlural.ToString());

					subjunctiveImperfectRa6Run.FontSize = 12D;

					subjunctiveImperfectRa6TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa6Run));

					subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa6TableCell);

					TableCell subjunctiveImperfectRa7TableCell = new TableCell();

					if (conjugation.ImperfectRaSubjunctive.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectRa7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectRa7TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectRa7TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectRa7TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectRa7Run = new Run(conjugation.ImperfectRaSubjunctive.ThirdPersonPlural.ToString());

					subjunctiveImperfectRa7Run.FontSize = 12D;

					subjunctiveImperfectRa7TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectRa7Run));

					subjunctiveImperfectRaTableRow.Cells.Add(subjunctiveImperfectRa7TableCell);

					subjunctiveTableTableRowGroup.Rows.Add(subjunctiveImperfectRaTableRow);

					/*
					 * Subjunctive | Row: imperfect (se)
					 */

					TableRow subjunctiveImperfectSeTableRow = new TableRow();

					TableCell subjunctiveImperfectSe1TableCell = new TableCell();

					subjunctiveImperfectSe1TableCell.Background = Brushes.LightGray;
					subjunctiveImperfectSe1TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectSe1TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectSe1TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectSe1Run = new Run("Imperfect (se):");

					subjunctiveImperfectSe1Run.FontSize = 12D;

					subjunctiveImperfectSe1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveImperfectSe1Run)));

					subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe1TableCell);

					TableCell subjunctiveImperfectSe2TableCell = new TableCell();

					if (conjugation.ImperfectSeSubjunctive.FirstPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectSe2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectSe2TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectSe2TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectSe2TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectSe2Run = new Run(conjugation.ImperfectSeSubjunctive.FirstPersonSingular.ToString());

					subjunctiveImperfectSe2Run.FontSize = 12D;

					subjunctiveImperfectSe2TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe2Run));

					subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe2TableCell);

					TableCell subjunctiveImperfectSe3TableCell = new TableCell();

					if (conjugation.ImperfectSeSubjunctive.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectSe3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectSe3TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectSe3TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectSe3TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectSe3Run = new Run(conjugation.ImperfectSeSubjunctive.SecondPersonSingular.ToString());

					subjunctiveImperfectSe3Run.FontSize = 12D;

					subjunctiveImperfectSe3TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe3Run));

					subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe3TableCell);

					TableCell subjunctiveImperfectSe4TableCell = new TableCell();

					if (conjugation.ImperfectSeSubjunctive.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectSe4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectSe4TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectSe4TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectSe4TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectSe4Run = new Run(conjugation.ImperfectSeSubjunctive.ThirdPersonSingular.ToString());

					subjunctiveImperfectSe4Run.FontSize = 12D;

					subjunctiveImperfectSe4TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe4Run));

					subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe4TableCell);

					TableCell subjunctiveImperfectSe5TableCell = new TableCell();

					if (conjugation.ImperfectSeSubjunctive.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectSe5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectSe5TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectSe5TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectSe5TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectSe5Run = new Run(conjugation.ImperfectSeSubjunctive.FirstPersonPlural.ToString());

					subjunctiveImperfectSe5Run.FontSize = 12D;

					subjunctiveImperfectSe5TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe5Run));

					subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe5TableCell);

					TableCell subjunctiveImperfectSe6TableCell = new TableCell();

					if (conjugation.ImperfectSeSubjunctive.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectSe6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectSe6TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectSe6TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectSe6TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectSe6Run = new Run(conjugation.ImperfectSeSubjunctive.SecondPersonPlural.ToString());

					subjunctiveImperfectSe6Run.FontSize = 12D;

					subjunctiveImperfectSe6TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe6Run));

					subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe6TableCell);

					TableCell subjunctiveImperfectSe7TableCell = new TableCell();

					if (conjugation.ImperfectSeSubjunctive.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctiveImperfectSe7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveImperfectSe7TableCell.BorderBrush = Brushes.Black;
					subjunctiveImperfectSe7TableCell.BorderThickness = tableBorderThickness;
					subjunctiveImperfectSe7TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveImperfectSe7Run = new Run(conjugation.ImperfectSeSubjunctive.ThirdPersonPlural.ToString());

					subjunctiveImperfectSe7Run.FontSize = 12D;

					subjunctiveImperfectSe7TableCell.Blocks.Add(new Paragraph(subjunctiveImperfectSe7Run));

					subjunctiveImperfectSeTableRow.Cells.Add(subjunctiveImperfectSe7TableCell);

					subjunctiveTableTableRowGroup.Rows.Add(subjunctiveImperfectSeTableRow);

					/*
					 * Subjunctive | Row: future
					 */

					TableRow subjunctiveFutureTableRow = new TableRow();

					TableCell subjunctiveFuture1TableCell = new TableCell();

					subjunctiveFuture1TableCell.Background = Brushes.LightGray;
					subjunctiveFuture1TableCell.BorderBrush = Brushes.Black;
					subjunctiveFuture1TableCell.BorderThickness = tableBorderThickness;
					subjunctiveFuture1TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveFuture1Run = new Run("Future:");

					subjunctiveFuture1Run.FontSize = 12D;

					subjunctiveFuture1TableCell.Blocks.Add(new Paragraph(new Bold(subjunctiveFuture1Run)));

					subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture1TableCell);

					TableCell subjunctiveFuture2TableCell = new TableCell();

					if (conjugation.FutureSubjunctive.FirstPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctiveFuture2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveFuture2TableCell.BorderBrush = Brushes.Black;
					subjunctiveFuture2TableCell.BorderThickness = tableBorderThickness;
					subjunctiveFuture2TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveFuture2Run = new Run(conjugation.FutureSubjunctive.FirstPersonSingular.ToString());

					subjunctiveFuture2Run.FontSize = 12D;

					subjunctiveFuture2TableCell.Blocks.Add(new Paragraph(subjunctiveFuture2Run));

					subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture2TableCell);

					TableCell subjunctiveFuture3TableCell = new TableCell();

					if (conjugation.FutureSubjunctive.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctiveFuture3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveFuture3TableCell.BorderBrush = Brushes.Black;
					subjunctiveFuture3TableCell.BorderThickness = tableBorderThickness;
					subjunctiveFuture3TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveFuture3Run = new Run(conjugation.FutureSubjunctive.SecondPersonSingular.ToString());

					subjunctiveFuture3Run.FontSize = 12D;

					subjunctiveFuture3TableCell.Blocks.Add(new Paragraph(subjunctiveFuture3Run));

					subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture3TableCell);

					TableCell subjunctiveFuture4TableCell = new TableCell();

					if (conjugation.FutureSubjunctive.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						subjunctiveFuture4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveFuture4TableCell.BorderBrush = Brushes.Black;
					subjunctiveFuture4TableCell.BorderThickness = tableBorderThickness;
					subjunctiveFuture4TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveFuture4Run = new Run(conjugation.FutureSubjunctive.ThirdPersonSingular.ToString());

					subjunctiveFuture4Run.FontSize = 12D;

					subjunctiveFuture4TableCell.Blocks.Add(new Paragraph(subjunctiveFuture4Run));

					subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture4TableCell);

					TableCell subjunctiveFuture5TableCell = new TableCell();

					if (conjugation.FutureSubjunctive.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctiveFuture5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveFuture5TableCell.BorderBrush = Brushes.Black;
					subjunctiveFuture5TableCell.BorderThickness = tableBorderThickness;
					subjunctiveFuture5TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveFuture5Run = new Run(conjugation.FutureSubjunctive.FirstPersonPlural.ToString());

					subjunctiveFuture5Run.FontSize = 12D;

					subjunctiveFuture5TableCell.Blocks.Add(new Paragraph(subjunctiveFuture5Run));

					subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture5TableCell);

					TableCell subjunctiveFuture6TableCell = new TableCell();

					if (conjugation.FutureSubjunctive.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctiveFuture6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveFuture6TableCell.BorderBrush = Brushes.Black;
					subjunctiveFuture6TableCell.BorderThickness = tableBorderThickness;
					subjunctiveFuture6TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveFuture6Run = new Run(conjugation.FutureSubjunctive.SecondPersonPlural.ToString());

					subjunctiveFuture6Run.FontSize = 12D;

					subjunctiveFuture6TableCell.Blocks.Add(new Paragraph(subjunctiveFuture6Run));

					subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture6TableCell);

					TableCell subjunctiveFuture7TableCell = new TableCell();

					if (conjugation.FutureSubjunctive.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						subjunctiveFuture7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					subjunctiveFuture7TableCell.BorderBrush = Brushes.Black;
					subjunctiveFuture7TableCell.BorderThickness = tableBorderThickness;
					subjunctiveFuture7TableCell.Padding = tableTableCellPaddingThickness;

					Run subjunctiveFuture7Run = new Run(conjugation.FutureSubjunctive.ThirdPersonPlural.ToString());

					subjunctiveFuture7Run.FontSize = 12D;

					subjunctiveFuture7TableCell.Blocks.Add(new Paragraph(subjunctiveFuture7Run));

					subjunctiveFutureTableRow.Cells.Add(subjunctiveFuture7TableCell);

					subjunctiveTableTableRowGroup.Rows.Add(subjunctiveFutureTableRow);

					/*
					 * Subjunctive | RowGroup to Table and Table to FlowDocument.
					 */

					subjunctiveTable.RowGroups.Add(subjunctiveTableTableRowGroup);

					this.tableFlowDocument.Blocks.Add(subjunctiveTable);

					/*
					 * Imperative
					 */

					Paragraph imperativeParagraph = new Paragraph();

					imperativeParagraph.Inlines.Add(new Bold(new Run("Imperative")));

					this.tableFlowDocument.Blocks.Add(imperativeParagraph);

					Table imperativeTable = new Table();

					imperativeTable.BorderBrush = Brushes.Black;
					imperativeTable.BorderThickness = tableBorderThickness;
					imperativeTable.CellSpacing = 0D;

					TableRowGroup imperativeTableTableRowGroup = new TableRowGroup();

					/*
					 * Imperative | Row: header
					 */

					TableRow imperativeHeaderTableRow = new TableRow();

					imperativeHeaderTableRow.Background = Brushes.LightGray;

					TableCell imperativeHeader1TableCell = new TableCell();

					imperativeHeader1TableCell.BorderBrush = Brushes.Black;
					imperativeHeader1TableCell.BorderThickness = tableBorderThickness;
					imperativeHeader1TableCell.Padding = tableTableCellPaddingThickness;
					imperativeHeader1TableCell.TextAlignment = TextAlignment.Center;

					Run imperativeHeader1Run = new Run(string.Empty);

					imperativeHeader1Run.FontSize = 12D;

					imperativeHeader1TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader1Run)));

					imperativeHeaderTableRow.Cells.Add(imperativeHeader1TableCell);

					TableCell imperativeHeader2TableCell = new TableCell();

					imperativeHeader2TableCell.BorderBrush = Brushes.Black;
					imperativeHeader2TableCell.BorderThickness = tableBorderThickness;
					imperativeHeader2TableCell.Padding = tableTableCellPaddingThickness;
					imperativeHeader2TableCell.TextAlignment = TextAlignment.Center;

					Run imperativeHeader2Run = new Run("yo");

					imperativeHeader2Run.FontSize = 12D;

					imperativeHeader2TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader2Run)));

					imperativeHeaderTableRow.Cells.Add(imperativeHeader2TableCell);

					TableCell imperativeHeader3TableCell = new TableCell();

					imperativeHeader3TableCell.BorderBrush = Brushes.Black;
					imperativeHeader3TableCell.BorderThickness = tableBorderThickness;
					imperativeHeader3TableCell.Padding = tableTableCellPaddingThickness;
					imperativeHeader3TableCell.TextAlignment = TextAlignment.Center;

					Run imperativeHeader3Run = new Run("tú");

					imperativeHeader3Run.FontSize = 12D;

					imperativeHeader3TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader3Run)));

					imperativeHeaderTableRow.Cells.Add(imperativeHeader3TableCell);

					TableCell imperativeHeader4TableCell = new TableCell();

					imperativeHeader4TableCell.BorderBrush = Brushes.Black;
					imperativeHeader4TableCell.BorderThickness = tableBorderThickness;
					imperativeHeader4TableCell.Padding = tableTableCellPaddingThickness;
					imperativeHeader4TableCell.TextAlignment = TextAlignment.Center;

					Run imperativeHeader4Run = new Run("él/ella/ello");

					imperativeHeader4Run.FontSize = 12D;

					imperativeHeader4TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader4Run)));

					imperativeHeaderTableRow.Cells.Add(imperativeHeader4TableCell);

					TableCell imperativeHeader5TableCell = new TableCell();

					imperativeHeader5TableCell.BorderBrush = Brushes.Black;
					imperativeHeader5TableCell.BorderThickness = tableBorderThickness;
					imperativeHeader5TableCell.Padding = tableTableCellPaddingThickness;
					imperativeHeader5TableCell.TextAlignment = TextAlignment.Center;

					Run imperativeHeader5Run = new Run("nosotros/nosotras");

					imperativeHeader5Run.FontSize = 12D;

					imperativeHeader5TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader5Run)));

					imperativeHeaderTableRow.Cells.Add(imperativeHeader5TableCell);

					TableCell imperativeHeader6TableCell = new TableCell();

					imperativeHeader6TableCell.BorderBrush = Brushes.Black;
					imperativeHeader6TableCell.BorderThickness = tableBorderThickness;
					imperativeHeader6TableCell.Padding = tableTableCellPaddingThickness;
					imperativeHeader6TableCell.TextAlignment = TextAlignment.Center;

					Run imperativeHeader6Run = new Run("vosotros/vosotras");

					imperativeHeader6Run.FontSize = 12D;

					imperativeHeader6TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader6Run)));

					imperativeHeaderTableRow.Cells.Add(imperativeHeader6TableCell);

					TableCell imperativeHeader7TableCell = new TableCell();

					imperativeHeader7TableCell.BorderBrush = Brushes.Black;
					imperativeHeader7TableCell.BorderThickness = tableBorderThickness;
					imperativeHeader7TableCell.Padding = tableTableCellPaddingThickness;
					imperativeHeader7TableCell.TextAlignment = TextAlignment.Center;

					Run imperativeHeader7Run = new Run("ellos/ellas");

					imperativeHeader7Run.FontSize = 12D;

					imperativeHeader7TableCell.Blocks.Add(new Paragraph(new Bold(imperativeHeader7Run)));

					imperativeHeaderTableRow.Cells.Add(imperativeHeader7TableCell);

					imperativeTableTableRowGroup.Rows.Add(imperativeHeaderTableRow);

					/*
					 * Imperative | Row: affirmative
					 */

					TableRow imperativeAffirmativeTableRow = new TableRow();

					TableCell imperativeAffirmative1TableCell = new TableCell();

					imperativeAffirmative1TableCell.Background = Brushes.LightGray;
					imperativeAffirmative1TableCell.BorderBrush = Brushes.Black;
					imperativeAffirmative1TableCell.BorderThickness = tableBorderThickness;
					imperativeAffirmative1TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeAffirmative1Run = new Run("Affirmative:");

					imperativeAffirmative1Run.FontSize = 12D;

					imperativeAffirmative1TableCell.Blocks.Add(new Paragraph(new Bold(imperativeAffirmative1Run)));

					imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative1TableCell);

					TableCell imperativeAffirmative2TableCell = new TableCell();

					imperativeAffirmative2TableCell.BorderBrush = Brushes.Black;
					imperativeAffirmative2TableCell.BorderThickness = tableBorderThickness;
					imperativeAffirmative2TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeAffirmative2Run = new Run("—");

					imperativeAffirmative2Run.FontSize = 12D;

					imperativeAffirmative2TableCell.Blocks.Add(new Paragraph(imperativeAffirmative2Run));

					imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative2TableCell);

					TableCell imperativeAffirmative3TableCell = new TableCell();

					if (conjugation.AffirmativeImperative.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						imperativeAffirmative3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeAffirmative3TableCell.BorderBrush = Brushes.Black;
					imperativeAffirmative3TableCell.BorderThickness = tableBorderThickness;
					imperativeAffirmative3TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeAffirmative3Run = new Run(conjugation.AffirmativeImperative.SecondPersonSingular.ToString());

					imperativeAffirmative3Run.FontSize = 12D;

					imperativeAffirmative3TableCell.Blocks.Add(new Paragraph(imperativeAffirmative3Run));

					imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative3TableCell);

					TableCell imperativeAffirmative4TableCell = new TableCell();

					if (conjugation.AffirmativeImperative.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						imperativeAffirmative4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeAffirmative4TableCell.BorderBrush = Brushes.Black;
					imperativeAffirmative4TableCell.BorderThickness = tableBorderThickness;
					imperativeAffirmative4TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeAffirmative4Run = new Run(conjugation.AffirmativeImperative.ThirdPersonSingular.ToString());

					imperativeAffirmative4Run.FontSize = 12D;

					imperativeAffirmative4TableCell.Blocks.Add(new Paragraph(imperativeAffirmative4Run));

					imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative4TableCell);

					TableCell imperativeAffirmative5TableCell = new TableCell();

					if (conjugation.AffirmativeImperative.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						imperativeAffirmative5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeAffirmative5TableCell.BorderBrush = Brushes.Black;
					imperativeAffirmative5TableCell.BorderThickness = tableBorderThickness;
					imperativeAffirmative5TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeAffirmative5Run = new Run(conjugation.AffirmativeImperative.FirstPersonPlural.ToString());

					imperativeAffirmative5Run.FontSize = 12D;

					imperativeAffirmative5TableCell.Blocks.Add(new Paragraph(imperativeAffirmative5Run));

					imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative5TableCell);

					TableCell imperativeAffirmative6TableCell = new TableCell();

					if (conjugation.AffirmativeImperative.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						imperativeAffirmative6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeAffirmative6TableCell.BorderBrush = Brushes.Black;
					imperativeAffirmative6TableCell.BorderThickness = tableBorderThickness;
					imperativeAffirmative6TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeAffirmative6Run = new Run(conjugation.AffirmativeImperative.SecondPersonPlural.ToString());

					imperativeAffirmative6Run.FontSize = 12D;

					imperativeAffirmative6TableCell.Blocks.Add(new Paragraph(imperativeAffirmative6Run));

					imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative6TableCell);

					TableCell imperativeAffirmative7TableCell = new TableCell();

					if (conjugation.AffirmativeImperative.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						imperativeAffirmative7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeAffirmative7TableCell.BorderBrush = Brushes.Black;
					imperativeAffirmative7TableCell.BorderThickness = tableBorderThickness;
					imperativeAffirmative7TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeAffirmative7Run = new Run(conjugation.AffirmativeImperative.ThirdPersonPlural.ToString());

					imperativeAffirmative7Run.FontSize = 12D;

					imperativeAffirmative7TableCell.Blocks.Add(new Paragraph(imperativeAffirmative7Run));

					imperativeAffirmativeTableRow.Cells.Add(imperativeAffirmative7TableCell);

					imperativeTableTableRowGroup.Rows.Add(imperativeAffirmativeTableRow);

					/*
					 * Imperative | Row: negative
					 */

					TableRow imperativeNegativeTableRow = new TableRow();

					TableCell imperativeNegative1TableCell = new TableCell();

					imperativeNegative1TableCell.Background = Brushes.LightGray;
					imperativeNegative1TableCell.BorderBrush = Brushes.Black;
					imperativeNegative1TableCell.BorderThickness = tableBorderThickness;
					imperativeNegative1TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeNegative1Run = new Run("Negative:");

					imperativeNegative1Run.FontSize = 12D;

					imperativeNegative1TableCell.Blocks.Add(new Paragraph(new Bold(imperativeNegative1Run)));

					imperativeNegativeTableRow.Cells.Add(imperativeNegative1TableCell);

					TableCell imperativeNegative2TableCell = new TableCell();

					imperativeNegative2TableCell.BorderBrush = Brushes.Black;
					imperativeNegative2TableCell.BorderThickness = tableBorderThickness;
					imperativeNegative2TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeNegative2Run = new Run("—");

					imperativeNegative2Run.FontSize = 12D;

					imperativeNegative2TableCell.Blocks.Add(new Paragraph(imperativeNegative2Run));

					imperativeNegativeTableRow.Cells.Add(imperativeNegative2TableCell);

					TableCell imperativeNegative3TableCell = new TableCell();

					if (conjugation.NegativeImperative.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						imperativeNegative3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeNegative3TableCell.BorderBrush = Brushes.Black;
					imperativeNegative3TableCell.BorderThickness = tableBorderThickness;
					imperativeNegative3TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeNegative3Run = new Run(conjugation.NegativeImperative.SecondPersonSingular.ToString());

					imperativeNegative3Run.FontSize = 12D;

					imperativeNegative3TableCell.Blocks.Add(new Paragraph(imperativeNegative3Run));

					imperativeNegativeTableRow.Cells.Add(imperativeNegative3TableCell);

					TableCell imperativeNegative4TableCell = new TableCell();

					if (conjugation.NegativeImperative.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						imperativeNegative4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeNegative4TableCell.BorderBrush = Brushes.Black;
					imperativeNegative4TableCell.BorderThickness = tableBorderThickness;
					imperativeNegative4TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeNegative4Run = new Run(conjugation.NegativeImperative.ThirdPersonSingular.ToString());

					imperativeNegative4Run.FontSize = 12D;

					imperativeNegative4TableCell.Blocks.Add(new Paragraph(imperativeNegative4Run));

					imperativeNegativeTableRow.Cells.Add(imperativeNegative4TableCell);

					TableCell imperativeNegative5TableCell = new TableCell();

					if (conjugation.NegativeImperative.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						imperativeNegative5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeNegative5TableCell.BorderBrush = Brushes.Black;
					imperativeNegative5TableCell.BorderThickness = tableBorderThickness;
					imperativeNegative5TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeNegative5Run = new Run(conjugation.NegativeImperative.FirstPersonPlural.ToString());

					imperativeNegative5Run.FontSize = 12D;

					imperativeNegative5TableCell.Blocks.Add(new Paragraph(imperativeNegative5Run));

					imperativeNegativeTableRow.Cells.Add(imperativeNegative5TableCell);

					TableCell imperativeNegative6TableCell = new TableCell();

					if (conjugation.NegativeImperative.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						imperativeNegative6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeNegative6TableCell.BorderBrush = Brushes.Black;
					imperativeNegative6TableCell.BorderThickness = tableBorderThickness;
					imperativeNegative6TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeNegative6Run = new Run(conjugation.NegativeImperative.SecondPersonPlural.ToString());

					imperativeNegative6Run.FontSize = 12D;

					imperativeNegative6TableCell.Blocks.Add(new Paragraph(imperativeNegative6Run));

					imperativeNegativeTableRow.Cells.Add(imperativeNegative6TableCell);

					TableCell imperativeNegative7TableCell = new TableCell();

					if (conjugation.NegativeImperative.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						imperativeNegative7TableCell.Background = irregularInflectionSolidColorBrush;
					}

					imperativeNegative7TableCell.BorderBrush = Brushes.Black;
					imperativeNegative7TableCell.BorderThickness = tableBorderThickness;
					imperativeNegative7TableCell.Padding = tableTableCellPaddingThickness;

					Run imperativeNegative7Run = new Run(conjugation.NegativeImperative.ThirdPersonPlural.ToString());

					imperativeNegative7Run.FontSize = 12D;

					imperativeNegative7TableCell.Blocks.Add(new Paragraph(imperativeNegative7Run));

					imperativeNegativeTableRow.Cells.Add(imperativeNegative7TableCell);

					imperativeTableTableRowGroup.Rows.Add(imperativeNegativeTableRow);

					/*
					 * Imperative | RowGroup to Table and Table to FlowDocument.
					 */

					imperativeTable.RowGroups.Add(imperativeTableTableRowGroup);

					this.tableFlowDocument.Blocks.Add(imperativeTable);

					/*
					 * Conditional
					 */

					Paragraph conditionalParagraph = new Paragraph();

					conditionalParagraph.Inlines.Add(new Bold(new Run("Conditional")));

					this.tableFlowDocument.Blocks.Add(conditionalParagraph);

					Table conditionalTable = new Table();

					conditionalTable.BorderBrush = Brushes.Black;
					conditionalTable.BorderThickness = tableBorderThickness;
					conditionalTable.CellSpacing = 0D;

					TableRowGroup conditionalTableTableRowGroup = new TableRowGroup();

					/*
					 * Conditional | Row: header
					 */

					TableRow conditionalHeaderTableRow = new TableRow();

					conditionalHeaderTableRow.Background = Brushes.LightGray;

					TableCell conditionalHeader1TableCell = new TableCell();

					conditionalHeader1TableCell.BorderBrush = Brushes.Black;
					conditionalHeader1TableCell.BorderThickness = tableBorderThickness;
					conditionalHeader1TableCell.Padding = tableTableCellPaddingThickness;
					conditionalHeader1TableCell.TextAlignment = TextAlignment.Center;

					Run conditionalHeader1Run = new Run("yo");

					conditionalHeader1Run.FontSize = 12D;

					conditionalHeader1TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader1Run)));

					conditionalHeaderTableRow.Cells.Add(conditionalHeader1TableCell);

					TableCell conditionalHeader2TableCell = new TableCell();

					conditionalHeader2TableCell.BorderBrush = Brushes.Black;
					conditionalHeader2TableCell.BorderThickness = tableBorderThickness;
					conditionalHeader2TableCell.Padding = tableTableCellPaddingThickness;
					conditionalHeader2TableCell.TextAlignment = TextAlignment.Center;

					Run conditionalHeader2Run = new Run("tú");

					conditionalHeader2Run.FontSize = 12D;

					conditionalHeader2TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader2Run)));

					conditionalHeaderTableRow.Cells.Add(conditionalHeader2TableCell);

					TableCell conditionalHeader3TableCell = new TableCell();

					conditionalHeader3TableCell.BorderBrush = Brushes.Black;
					conditionalHeader3TableCell.BorderThickness = tableBorderThickness;
					conditionalHeader3TableCell.Padding = tableTableCellPaddingThickness;
					conditionalHeader3TableCell.TextAlignment = TextAlignment.Center;

					Run conditionalHeader3Run = new Run("él/ella/ello");

					conditionalHeader3Run.FontSize = 12D;

					conditionalHeader3TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader3Run)));

					conditionalHeaderTableRow.Cells.Add(conditionalHeader3TableCell);

					TableCell conditionalHeader4TableCell = new TableCell();

					conditionalHeader4TableCell.BorderBrush = Brushes.Black;
					conditionalHeader4TableCell.BorderThickness = tableBorderThickness;
					conditionalHeader4TableCell.Padding = tableTableCellPaddingThickness;
					conditionalHeader4TableCell.TextAlignment = TextAlignment.Center;

					Run conditionalHeader4Run = new Run("nosotros/nosotras");

					conditionalHeader4Run.FontSize = 12D;

					conditionalHeader4TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader4Run)));

					conditionalHeaderTableRow.Cells.Add(conditionalHeader4TableCell);

					TableCell conditionalHeader5TableCell = new TableCell();

					conditionalHeader5TableCell.BorderBrush = Brushes.Black;
					conditionalHeader5TableCell.BorderThickness = tableBorderThickness;
					conditionalHeader5TableCell.Padding = tableTableCellPaddingThickness;
					conditionalHeader5TableCell.TextAlignment = TextAlignment.Center;

					Run conditionalHeader5Run = new Run("vosotros/vosotras");

					conditionalHeader5Run.FontSize = 12D;

					conditionalHeader5TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader5Run)));

					conditionalHeaderTableRow.Cells.Add(conditionalHeader5TableCell);

					TableCell conditionalHeader6TableCell = new TableCell();

					conditionalHeader6TableCell.BorderBrush = Brushes.Black;
					conditionalHeader6TableCell.BorderThickness = tableBorderThickness;
					conditionalHeader6TableCell.Padding = tableTableCellPaddingThickness;
					conditionalHeader6TableCell.TextAlignment = TextAlignment.Center;

					Run conditionalHeader6Run = new Run("ellos/ellas");

					conditionalHeader6Run.FontSize = 12D;

					conditionalHeader6TableCell.Blocks.Add(new Paragraph(new Bold(conditionalHeader6Run)));

					conditionalHeaderTableRow.Cells.Add(conditionalHeader6TableCell);

					conditionalTableTableRowGroup.Rows.Add(conditionalHeaderTableRow);

					/*
					 * Conditional | Row: conditional
					 */

					TableRow conditionalTableRow = new TableRow();

					TableCell conditional1TableCell = new TableCell();

					if (conjugation.Conditional.FirstPersonSingular.Inflection == Inflection.Irregular)
					{
						conditional1TableCell.Background = irregularInflectionSolidColorBrush;
					}

					conditional1TableCell.BorderBrush = Brushes.Black;
					conditional1TableCell.BorderThickness = tableBorderThickness;
					conditional1TableCell.Padding = tableTableCellPaddingThickness;

					Run conditional1Run = new Run(conjugation.Conditional.FirstPersonSingular.ToString());

					conditional1Run.FontSize = 12D;

					conditional1TableCell.Blocks.Add(new Paragraph(conditional1Run));

					conditionalTableRow.Cells.Add(conditional1TableCell);

					TableCell conditional2TableCell = new TableCell();

					if (conjugation.Conditional.SecondPersonSingular.Inflection == Inflection.Irregular)
					{
						conditional2TableCell.Background = irregularInflectionSolidColorBrush;
					}

					conditional2TableCell.BorderBrush = Brushes.Black;
					conditional2TableCell.BorderThickness = tableBorderThickness;
					conditional2TableCell.Padding = tableTableCellPaddingThickness;

					Run conditional2Run = new Run(conjugation.Conditional.SecondPersonSingular.ToString());

					conditional2Run.FontSize = 12D;

					conditional2TableCell.Blocks.Add(new Paragraph(conditional2Run));

					conditionalTableRow.Cells.Add(conditional2TableCell);

					TableCell conditional3TableCell = new TableCell();

					if (conjugation.Conditional.ThirdPersonSingular.Inflection == Inflection.Irregular)
					{
						conditional3TableCell.Background = irregularInflectionSolidColorBrush;
					}

					conditional3TableCell.BorderBrush = Brushes.Black;
					conditional3TableCell.BorderThickness = tableBorderThickness;
					conditional3TableCell.Padding = tableTableCellPaddingThickness;

					Run conditional3Run = new Run(conjugation.Conditional.ThirdPersonSingular.ToString());

					conditional3Run.FontSize = 12D;

					conditional3TableCell.Blocks.Add(new Paragraph(conditional3Run));

					conditionalTableRow.Cells.Add(conditional3TableCell);

					TableCell conditional4TableCell = new TableCell();

					if (conjugation.Conditional.FirstPersonPlural.Inflection == Inflection.Irregular)
					{
						conditional4TableCell.Background = irregularInflectionSolidColorBrush;
					}

					conditional4TableCell.BorderBrush = Brushes.Black;
					conditional4TableCell.BorderThickness = tableBorderThickness;
					conditional4TableCell.Padding = tableTableCellPaddingThickness;

					Run conditional4Run = new Run(conjugation.Conditional.FirstPersonPlural.ToString());

					conditional4Run.FontSize = 12D;

					conditional4TableCell.Blocks.Add(new Paragraph(conditional4Run));

					conditionalTableRow.Cells.Add(conditional4TableCell);

					TableCell conditional5TableCell = new TableCell();

					if (conjugation.Conditional.SecondPersonPlural.Inflection == Inflection.Irregular)
					{
						conditional5TableCell.Background = irregularInflectionSolidColorBrush;
					}

					conditional5TableCell.BorderBrush = Brushes.Black;
					conditional5TableCell.BorderThickness = tableBorderThickness;
					conditional5TableCell.Padding = tableTableCellPaddingThickness;

					Run conditional5Run = new Run(conjugation.Conditional.SecondPersonPlural.ToString());

					conditional5Run.FontSize = 12D;

					conditional5TableCell.Blocks.Add(new Paragraph(conditional5Run));

					conditionalTableRow.Cells.Add(conditional5TableCell);

					TableCell conditional6TableCell = new TableCell();

					if (conjugation.Conditional.ThirdPersonPlural.Inflection == Inflection.Irregular)
					{
						conditional6TableCell.Background = irregularInflectionSolidColorBrush;
					}

					conditional6TableCell.BorderBrush = Brushes.Black;
					conditional6TableCell.BorderThickness = tableBorderThickness;
					conditional6TableCell.Padding = tableTableCellPaddingThickness;

					Run conditional6Run = new Run(conjugation.Conditional.ThirdPersonPlural.ToString());

					conditional6Run.FontSize = 12D;

					conditional6TableCell.Blocks.Add(new Paragraph(conditional6Run));

					conditionalTableRow.Cells.Add(conditional6TableCell);

					conditionalTableTableRowGroup.Rows.Add(conditionalTableRow);

					/*
					 * Conditional | RowGroup to Table and Table to FlowDocument.
					 */

					conditionalTable.RowGroups.Add(conditionalTableTableRowGroup);

					this.tableFlowDocument.Blocks.Add(conditionalTable);
				}
				catch (Exception)
				{
					this.verbsListBox.Visibility = Visibility.Collapsed;

					MessageBox.Show("The software could not access or locate one or more of the verb files on the system. Ensure that the verb files are in their proper location and that you have permission to access them, then restart the program.", "Error");
				}
			}
		}

		private void InflectionGroupRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;

			switch (radioButton.Content.ToString())
			{
				case "irregular":
					this.showIrregular = true;
					this.showRegular = false;

					break;
				case "regular":
					this.showIrregular = false;
					this.showRegular = true;

					break;
			}

			this.isResetting = true;

			this.PopulateVerbsListBox();

			this.isResetting = false;
		}

		private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Directory.Exists(this.PathTextBox.Text))
			{
				this.ExportButton.IsEnabled = true;

				this.PathTextBox.Background = new SolidColorBrush(Color.FromArgb(255, (byte)153, (byte)255, (byte)153));
			}
			else
			{
				this.ExportButton.IsEnabled = false;

				this.PathTextBox.Background = new SolidColorBrush(Color.FromArgb(255, (byte)255, (byte)153, (byte)153));
			}
		}

		private void PopulateVerbsListBox()
		{
			this.verbsList.Clear();

			if (this.showIrregular)
			{
				if (this.showArEndings)
				{
					this.verbsList.AddRange(VerbManager.IrregularArVerbs);
				}

				if (this.showErEndings)
				{
					this.verbsList.AddRange(VerbManager.IrregularErVerbs);
				}

				if (this.showIrEndings)
				{
					this.verbsList.AddRange(VerbManager.IrregularIrVerbs);
				}
			}

			if (this.showRegular)
			{
				if (this.showArEndings)
				{
					this.verbsList.AddRange(VerbManager.RegularArVerbs);
				}

				if (this.showErEndings)
				{
					this.verbsList.AddRange(VerbManager.RegularErVerbs);
				}

				if (this.showIrEndings)
				{
					this.verbsList.AddRange(VerbManager.RegularIrVerbs);
				}
			}

			this.verbsList.Sort();

			this.verbsListBox.Items.Clear();

			if (!this.hasDefectiveFilter)
			{
				foreach (string verb in this.verbsList)
				{
					this.verbsListBox.Items.Add(verb);
				}
			}
			else
			{
				foreach (string verb in this.verbsList)
				{
					foreach (string defectiveVerb in VerbManager.DefectiveVerbs)
					{
						if (verb == defectiveVerb)
						{
							this.verbsListBox.Items.Add(verb);
						}
					}
				}
			}
		}

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			this.regularRadioButton.IsChecked = false;
			this.irregularRadioButton.IsChecked = false;

			this.arRadioButton.IsChecked = false;
			this.erRadioButton.IsChecked = false;
			this.irRadioButton.IsChecked = false;

			this.DefectiveCheckBox.IsChecked = false;

			this.showIrregular = true;
			this.showRegular = true;

			this.showArEndings = true;
			this.showErEndings = true;
			this.showIrEndings = true;

			this.hasDefectiveFilter = false;

			this.isResetting = true;

			this.PopulateVerbsListBox();

			this.isResetting = false;

			if (this.verbsListBox.Items.Count > 0)
			{
				this.verbsListBox.ScrollIntoView(this.verbsListBox.Items[0]);
			}

			this.GenerateConjugationTable();
		}

		private void VerbsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!this.isResetting)
			{
				this.GenerateConjugationTable();
			}
		}
	}
}