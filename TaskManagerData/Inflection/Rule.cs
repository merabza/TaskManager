﻿using System.Text.RegularExpressions;

namespace TaskManagerData.Inflection
{
  public class Rule
  {
    private readonly Regex _regex;
    private readonly string _replacement;

    public Rule(string pattern, string replacement)
    {
      _regex = new Regex(pattern, RegexOptions.IgnoreCase);
      _replacement = replacement;
    }

    public string Apply(string word)
    {
      return !_regex.IsMatch(word) ? null : _regex.Replace(word, _replacement);
    }
  }
}