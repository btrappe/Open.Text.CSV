﻿using Microsoft.Toolkit.HighPerformance.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Open.Text.CSV;

/// <summary>
/// Receives characters in a CSV sequence and translates them into values in a row.
/// </summary>
public sealed class CsvRowBuilder : CsvRowBuilderBase
{
	List<string>? _fields;
	readonly StringPool _stringPool = new();
	readonly StringBuilder _fb = new();
	readonly Action<List<string>> _rowHandler;

	public CsvRowBuilder(Action<List<string>> rowHandler)
	{
		_rowHandler = rowHandler ?? throw new ArgumentNullException(nameof(rowHandler));
	}

	/// <inheritdoc />
	public override void Reset()
	{
		_fields = null;
		base.Reset();
	}

	protected override void ResetFieldBuffer() => _fb.Clear();

	protected override void AddNextChar(in char c, bool ws = false)
	{
		_fb.Append(c);
		if (!ws) FieldLen = _fb.Length;
	}

	protected override void AddEntry()
	{
		_fields ??= new List<string>(MaxFields);
		if (FieldLen == 0)
		{
			_fields.Add(string.Empty);
			if (_fb.Length != 0) _fb.Clear();
			return;
		}

		if (FieldLen < _fb.Length) _fb.Length = FieldLen;
		_fields.Add(_stringPool.GetOrAdd(_fb.ToString()));
		_fb.Clear();
		FieldLen = 0;
	}

	protected override bool Complete()
	{
		var f = _fields;
		Reset();
		if (f is null) return false;

		var count = f.Count;
		if (count == 0) return false;
		if (count > MaxFields) MaxFields = count;

		_rowHandler(f);
		return true;
	}

}
