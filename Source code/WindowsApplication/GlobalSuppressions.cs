// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="GlobalSuppressions.cs">
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

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The exception type is not important.", Scope = "member", Target = "Emi.SpanishVerbConjugator.WindowsApplication.MainWindow.#GenerateConjugationTable()")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Assembly signing is not required.")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "No localisation required.", MessageId = "System.Windows.Documents.Run.#ctor(System.String)", Scope = "member", Target = "Emi.SpanishVerbConjugator.WindowsApplication.MainWindow.#GenerateConjugationTable()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "No localisation required.", MessageId = "System.Windows.MessageBox.Show(System.String,System.String)", Scope = "member", Target = "Emi.SpanishVerbConjugator.WindowsApplication.MainWindow.#GenerateConjugationTable()")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.MessageBox.Show(System.String,System.String)", Justification = "No localisation required.", Scope = "member", Target = "Emi.SpanishVerbConjugator.WindowsApplication.MainWindow.#.ctor()")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The method is as complex as it must be.", Scope = "member", Target = "Emi.SpanishVerbConjugator.WindowsApplication.MainWindow.#GenerateConjugationTable()")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "The method is as complex as it must be.", Scope = "member", Target = "Emi.SpanishVerbConjugator.WindowsApplication.MainWindow.#GenerateConjugationTable()")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "The method is as complex as it must be.", Scope = "type", Target = "Emi.SpanishVerbConjugator.WindowsApplication.MainWindow")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "The locals are as many as they must be.", Scope = "member", Target = "Emi.SpanishVerbConjugator.WindowsApplication.MainWindow.#GenerateConjugationTable()")]