﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace TaskManagerData.Inflection
{
  public static class Inflector
  {
    #region Default Rules

    static Inflector()
    {
      AddPlural("$", "s");
      AddPlural("s$", "s");
      AddPlural("(ax|test)is$", "$1es");
      AddPlural("(octop|vir|alumn|fung)us$", "$1i");
      AddPlural("(alias|status)$", "$1es");
      AddPlural("(bu)s$", "$1ses");
      AddPlural("(buffal|tomat|volcan)o$", "$1oes");
      AddPlural("([ti])um$", "$1a");
      AddPlural("sis$", "ses");
      AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
      AddPlural("(hive)$", "$1s");
      AddPlural("([^aeiouy]|qu)y$", "$1ies");
      AddPlural("(x|ch|ss|sh)$", "$1es");
      AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
      AddPlural("([m|l])ouse$", "$1ice");
      AddPlural("^(ox)$", "$1en");
      AddPlural("(quiz)$", "$1zes");

      AddSingular("s$", "");
      AddSingular("(n)ews$", "$1ews");
      AddSingular("([ti])a$", "$1um");
      AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
      AddSingular("(^analy)ses$", "$1sis");
      AddSingular("([^f])ves$", "$1fe");
      AddSingular("(hive)s$", "$1");
      AddSingular("(tive)s$", "$1");
      AddSingular("([lr])ves$", "$1f");
      AddSingular("([^aeiouy]|qu)ies$", "$1y");
      AddSingular("(s)eries$", "$1eries");
      AddSingular("(m)ovies$", "$1ovie");
      AddSingular("(x|ch|ss|sh)es$", "$1");
      AddSingular("([m|l])ice$", "$1ouse");
      AddSingular("(bus)es$", "$1");
      AddSingular("(o)es$", "$1");
      AddSingular("(shoe)s$", "$1");
      AddSingular("(cris|ax|test)es$", "$1is");
      AddSingular("(octop|vir|alumn|fung)i$", "$1us");
      AddSingular("(alias|status)$", "$1");
      AddSingular("(alias|status)es$", "$1");
      AddSingular("^(ox)en", "$1");
      AddSingular("(vert|ind)ices$", "$1ex");
      AddSingular("(matr)ices$", "$1ix");
      AddSingular("(quiz)zes$", "$1");

      AddIrregular("person", "people");
      AddIrregular("man", "men");
      AddIrregular("child", "children");
      AddIrregular("sex", "sexes");
      AddIrregular("move", "moves");
      AddIrregular("goose", "geese");
      AddIrregular("alumna", "alumnae");

      AddUncountable("equipment");
      AddUncountable("information");
      AddUncountable("rice");
      AddUncountable("money");
      AddUncountable("species");
      AddUncountable("series");
      AddUncountable("fish");
      AddUncountable("sheep");
      AddUncountable("deer");
      AddUncountable("aircraft");
    }

    #endregion


    private static void AddIrregular(string singular, string plural)
    {
      AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
      AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
    }

    private static void AddUncountable(string word)
    {
      Uncountables.Add(word.ToLower());
    }

    private static void AddPlural(string rule, string replacement)
    {
      Plurals.Add(new Rule(rule, replacement));
    }

    private static void AddSingular(string rule, string replacement)
    {
      Singulars.Add(new Rule(rule, replacement));
    }

    private static readonly List<Rule> Plurals = new List<Rule>();
    private static readonly List<Rule> Singulars = new List<Rule>();
    private static readonly List<string> Uncountables = new List<string>();

    public static string Pluralize(this string word)
    {
      return ApplyRules(Plurals, word);
    }

    public static string Singularize(this string word)
    {
      return ApplyRules(Singulars, word);
    }

    public static string SplitWithSpacesCamelParts(this string word, string separator = " ")
    {
      return string.Join(separator, word.SplitUpperCase());
    }

    public static string SingularizeCamelParts(this string word)
    {
      return string.Join("", word.SplitUpperCase().Select(s => s.Singularize()));
    }

    private static string ApplyRules(List<Rule> rules, string word)
    {
      string result = word;

      if (!Uncountables.Contains(word.ToLower()))
      {
        for (int i = rules.Count - 1; i >= 0; i--)
        {
          if ((result = rules[i].Apply(word)) != null)
          {
            break;
          }
        }
      }

      return result ?? word;
    }

    public static string Titleize(this string word)
    {
      return Regex.Replace(Humanize(Underscore(word)), @"\b([a-z])", match => match.Captures[0].Value.ToUpper());
    }

    public static string Humanize(this string lowercaseAndUnderscoredWord)
    {
      return Capitalize(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
    }

    public static string Pascalize(this string lowercaseAndUnderscoredWord)
    {
      return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)", match => match.Groups[1].Value.ToUpper());
    }

    public static string Camelize(this string lowercaseAndUnderscoredWord)
    {
      return UnCapitalize(Pascalize(lowercaseAndUnderscoredWord));
    }

    public static string Underscore(this string pascalCasedWord)
    {
      return Regex
        .Replace(
          Regex.Replace(Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])", "$1_$2"),
          @"[-\s]", "_").ToLower();
    }

    public static string Capitalize(this string word)
    {
      return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
    }

    public static string CapitalizeCamel(this string word)
    {
      return word.Substring(0, 1).ToUpper() + word.Substring(1);
    }

    public static string UnCapitalize(this string word)
    {
      return word.Substring(0, 1).ToLower() + word.Substring(1);
    }

    public static string Ordinalize(this string numberString)
    {
      return Ordanize(int.Parse(numberString), numberString);
    }

    public static string Ordinalize(this int number)
    {
      return Ordanize(number, number.ToString());
    }

#if NET45 || NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private static string Ordanize(int number, string numberString)
    {
      int nMod100 = number % 100;

      if (nMod100 >= 11 && nMod100 <= 13)
      {
        return numberString + "th";
      }

      switch (number % 10)
      {
        case 1:
          return numberString + "st";
        case 2:
          return numberString + "nd";
        case 3:
          return numberString + "rd";
        default:
          return numberString + "th";
      }
    }


    public static string Dasherize(this string underscoredWord)
    {
      return underscoredWord.Replace('_', '-');
    }


    public static string[] SplitUpperCase(this string source)
    {
      if(source == null)
        return new string[] {}; //Return empty array.
    
      if(source.Length == 0)
        return new[] {""};
    
      StringCollection words = new StringCollection();
      int wordStartIndex = 0;
    
      char[] letters = source.ToCharArray();
      // Skip the first letter. we don't care what case it is.
      for(int i = 1; i < letters.Length; i++)
      {
        if (!char.IsUpper(letters[i])) 
          continue;
        //Grab everything before the current index.
        words.Add(new string(letters, wordStartIndex, i - wordStartIndex));
        wordStartIndex = i;
      }
    
      //We need to have the last word.
      words.Add(new string(letters, wordStartIndex, letters.Length - wordStartIndex));
    
      //Copy to a string array.
      string[] wordArray = new string[words.Count];
      words.CopyTo(wordArray, 0);
      return wordArray;
    }

  }


}
