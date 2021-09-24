﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Open.Text.CSV
{
	public abstract class CsvReaderBase : IDisposable
	{
		protected CsvReaderBase(TextReader source)
		{
			_source = source ?? throw new ArgumentNullException(nameof(source));
			RowBuilder = new(SetNextRow);
		}

		protected readonly CsvRowBuilder RowBuilder;
		protected List<string>? NextRow;
		void SetNextRow(List<string> row) => NextRow = row;

		TextReader? _source;
		protected TextReader Source => _source ?? throw new ObjectDisposedException(GetType().ToString());

		public virtual void Dispose()
		{
			_source = null; // The intention here is if this object is disposed, then prevent further reading.
		}
	}
}
