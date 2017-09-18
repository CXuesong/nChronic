using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Chronic.Tags.Repeaters;

namespace Chronic.Tags.Repeaters
{
    public class RepeaterScanner : ITokenScanner
    {
        static readonly List<Func<Token, Options, ITag>> _scanners = new List
            <Func<Token, Options, ITag>>
            {
                //ScanSeasonNames(token, options),
                ScanMonthNames,
                ScanDayNames,
                ScanDayPortions,
                ScanTimes,
                ScanUnits,
            };

        public IList<Token> Scan(IList<Token> tokens, Options options)
        {
            tokens.ForEach(token =>
                {
                    foreach (var scanner in _scanners)
                    {
                        var tag = scanner(token, options);
                        if (tag != null)
                        {
                            token.Tag(tag);
                            break;
                        }
                    }
                });

            return tokens;
        }

        static ITag ScanUnits(Token token, Options options)
        {
            ITag tag = null;
            UnitPatterns.ForEach(item =>
            {
                if (item.Key.IsMatch(token.Value))
                {
                    var type = item.Value;
                    var hasCtorWithOptions = type.GetTypeInfo().DeclaredConstructors.Any(ctor =>
                    {
                        var parameters = ctor.GetParameters().ToArray();
                        return
                            parameters.Length == 1
                            && parameters.First().ParameterType == typeof(Options);
                    });
                    var ctorParameters = hasCtorWithOptions
                        ? new[] { options }
                        : new object[0];

                    tag = Activator.CreateInstance(
                        type,
                        ctorParameters) as ITag;

                    return;
                }
            });
            return tag;
        }

        static ITag ScanTimes(Token token, Options options)
        {
            var match = _timePattern.Match(token.Value);
            if (match.Success)
            {
                return new RepeaterTime(match.Value);
            }
            return null;
        }

        static ITag ScanDayPortions(Token token, Options options)
        {
            ITag tag = null;
            DayPortionPatterns.ForEach(item =>
                {
                    if (item.Key.IsMatch(token.Value))
                    {
                        tag = new EnumRepeaterDayPortion(item.Value);
                        return;
                    }
                });
            return tag;
        }

        static ITag ScanDayNames(Token token, Options options)
        {
            ITag tag = null;
            DayPatterns.ForEach(item =>
                {
                    if (item.Key.IsMatch(token.Value))
                    {
                        tag = new RepeaterDayName(item.Value);
                        return;
                    }
                });
            return tag;
        }

        static ITag ScanMonthNames(Token token, Options options)
        {
            ITag tag = null;
            MonthPatterns.ForEach(item =>
                {
                    if (item.Key.IsMatch(token.Value))
                    {
                        tag = new RepeaterMonthName(item.Value);
                        return;
                    }
                });
            return tag;
        }

        static ITag ScanSeasonNames(Token token, Options options)
        {
            throw new NotImplementedException();
        }

        static readonly Regex _timePattern =
            @"^\d{1,2}(:?\d{2})?([\.:]?\d{2})?$".Compile();

        private static readonly Dictionary<Regex, DayPortion> DayPortionPatterns = new Dictionary<Regex, DayPortion>
        {
            {"^ams?$".Compile(), DayPortion.AM},
            {"^pms?$".Compile(), DayPortion.PM},
            {"^mornings?$".Compile(), DayPortion.MORNING},
            {"^afternoons?$".Compile(), DayPortion.AFTERNOON},
            {"^evenings?$".Compile(), DayPortion.EVENING},
            {"^(night|nite)s?$".Compile(), DayPortion.NIGHT},
        };

        private static readonly Dictionary<Regex, DayOfWeek> DayPatterns = new Dictionary<Regex, DayOfWeek>
        {
            {"^m[ou]n(day)?$".Compile(), DayOfWeek.Monday},
            {"^t(ue|eu|oo|u|)s(day)?$".Compile(), DayOfWeek.Tuesday},
            {"^tue$".Compile(), DayOfWeek.Tuesday},
            {"^we(dnes|nds|nns)day$".Compile(), DayOfWeek.Wednesday},
            {"^wed$".Compile(), DayOfWeek.Wednesday},
            {"^th(urs|ers)day$".Compile(), DayOfWeek.Thursday},
            {"^thu$".Compile(), DayOfWeek.Thursday},
            {"^fr[iy](day)?$".Compile(), DayOfWeek.Friday},
            {"^sat(t?[ue]rday)?$".Compile(), DayOfWeek.Saturday},
            {"^su[nm](day)?$".Compile(), DayOfWeek.Sunday},
        };

        static readonly Dictionary<Regex, MonthName> MonthPatterns = new Dictionary<Regex, MonthName>
            {
                {"^jan\\.?(uary)?$".Compile(),  MonthName.January},
                {"^feb\\.?(ruary)?$".Compile(),  MonthName.February},
                {"^mar\\.?(ch)?$".Compile(),  MonthName.March},
                {"^apr\\.?(il)?$".Compile(),  MonthName.April},
                {"^may$".Compile(),  MonthName.May},
                {"^jun\\.?e?$".Compile(),  MonthName.June},
                {"^jul\\.?y?$".Compile(),  MonthName.July},
                {"^aug\\.?(ust)?$".Compile(),  MonthName.August},

                {
                  "^sep\\.?(t\\.?|tember)?$".Compile(),
                     MonthName.September
                },
                {"^oct\\.?(ober)?$".Compile(),  MonthName.October},
                {"^nov\\.?(ember)?$".Compile(),  MonthName.November},
                {"^dec\\.?(ember)?$".Compile(),  MonthName.December},
            };

        private static readonly Dictionary<Regex, Type> UnitPatterns = new Dictionary<Regex, Type>
        {
            {"^years?$".Compile(), typeof(RepeaterYear)},
            {"^seasons?$".Compile(), typeof(RepeaterSeason)},
            {"^months?$".Compile(), typeof(RepeaterMonth)},
            {"^fortnights?$".Compile(), typeof(RepeaterFortnight)},
            {"^weeks?$".Compile(), typeof(RepeaterWeek)},
            {"^weekends?$".Compile(), typeof(RepeaterWeekend)},
            {"^days?$".Compile(), typeof(RepeaterDay)},
            {"^hours?$".Compile(), typeof(RepeaterHour)},
            {"^minutes?$".Compile(), typeof(RepeaterMinute)},
            {"^seconds?$".Compile(), typeof(RepeaterSecond)}
        };
    }
}