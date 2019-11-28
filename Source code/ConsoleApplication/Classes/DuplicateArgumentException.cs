// <author>Stefán Örvar Sigmundsson</author>
// <copyright company="eMedia Intellect" file="DuplicateArgumentException.cs">
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
	using System.Runtime.Serialization;
	using System.Security.Permissions;

	[Serializable]
	public class DuplicateArgumentException : Exception
	{
		public DuplicateArgumentException()
		{
		}

		public DuplicateArgumentException(string message)
			: base(message)
		{
		}

		public DuplicateArgumentException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DuplicateArgumentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public string Argument { get; set; }

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			info.AddValue("Argument", this.Argument);

			base.GetObjectData(info, context);
		}
	}
}