using Xunit;
using TimeSpanHelper;
using System.Globalization;

namespace TimeSpanHelper.Tests;

public class TimeSpanToStringConverterTest
{
    [Theory]
    [InlineData(null, "", null)]
    [InlineData("1:2:3", "1:2:3", null)]
    [InlineData("-1:2:3", "-1:2:3", null)]
    [InlineData("1.0:0:0", "24:0:0", null)] //1day
    [InlineData("-1.0:0:0", "-24:0:0", null)] //1day negative
    [InlineData("1.0:0:0", "24:00:00", "'{SumHours}'\\:mm\\:ss")] //1day
    [InlineData("-1.0:0:0", "-24:00:00", "'{SumHours}'\\:mm\\:ss")] //1day negative
    [InlineData("1.0:0:0", "01.00:00:00", "'{Sign}'dd\\.hh\\:mm\\:ss")] //1day
    [InlineData("-1.0:0:0", "-01.00:00:00", "'{Sign}'dd\\.hh\\:mm\\:ss")] //1day negative
    public void ConvertTimeSpanOnParameter_RerturnString(string timeSpanObject, string expectedResult, string parameter)
    {
        TimeSpan? obj = timeSpanObject is null ? null : TimeSpan.Parse(timeSpanObject);
        var converter = new TimeSpanToStringConverter();
        var actual = converter.Convert(obj, null, parameter, CultureInfo.InvariantCulture);
        Assert.Equal(expectedResult, actual);
    }
}
public class TimeSpanTextBoxRuleTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("1:2")]
    [InlineData("1:2:3")]
    [InlineData("1:2:3:4")]
    [InlineData("1,2 3aaaa4")]
    [InlineData("-1")]
    [InlineData("-1 2")]
    [InlineData("abc1 2")]
    public void TimeSpanTextBoxRuleValidate_ReturnValid(object timeSpanObject)
    {
        var timeSpanTextBoxRule = new TimeSpanTextBoxRule();
        var actual = timeSpanTextBoxRule.Validate(timeSpanObject, CultureInfo.InvariantCulture);
        Assert.Equal(System.Windows.Controls.ValidationResult.ValidResult, actual);
    }

    [Theory]
    [InlineData("9999999999")]
    [InlineData("-9999999999")]
    [InlineData("0.0.0.0.0")]
    [InlineData("1 2 3 4 5")]
    public void TimeSpanTextBoxRuleValidate_ReturnInvalid(object timeSpanObject)
    {
        var timeSpanTextBoxRule = new TimeSpanTextBoxRule();
        var actual = timeSpanTextBoxRule.Validate(timeSpanObject, CultureInfo.InvariantCulture);
        Assert.False(actual.IsValid);
    }

    [Theory]
    [InlineData("1", "1:0:0", true)]
    [InlineData("1", "00:59:59.9990000", false)]
    [InlineData("-1:0:0", "-1:0:0", true)]
    [InlineData("0:0:0:1", "0:0:0:1", true)]
    [InlineData("1:2:3:4", "0:0:0:4", false)]
    [InlineData("10675199.02:48:05", "10675199.02:48:05.4775807", true)]
    [InlineData("-10675199.02:48:05", "-10675199.02:48:05.4775808", false)]
    public void TimeSpanTextBoxRuleValidateWithMaxTimeSpan(object timeSpanObject, string maxTimeSpanString, bool expectedResult)
    {
        var maxTimeSpan = TimeSpan.Parse(maxTimeSpanString);
        var timeSpanTextBoxRule = new TimeSpanTextBoxRule() { Max = maxTimeSpan };
        var actual = timeSpanTextBoxRule.Validate(timeSpanObject, CultureInfo.InvariantCulture);
        Assert.Equal(expectedResult, actual.IsValid);
    }

    [Theory]
    [InlineData("1", "1:0:0", true)]
    [InlineData("1", "00:59:59.9990000", true)]
    [InlineData("-1:0:0", "-1:0:0", true)]
    [InlineData("0:0:0:1", "0:0:0:1", true)]
    [InlineData("1:2:3:4", "0:0:0:4", true)]
    [InlineData("10675199.02:48:05", "10675199.02:48:05.4775807", false)]
    [InlineData("-10675199.02:48:05", "-10675199.02:48:05.4775808", true)]
    public void TimeSpanTextBoxRuleValidateWithMinTimeSpan(object timeSpanObject, string minTimeSpanString, bool expectedResult)
    {
        var minTimeSpan = TimeSpan.Parse(minTimeSpanString);
        var timeSpanTextBoxRule = new TimeSpanTextBoxRule() { Min = minTimeSpan };
        var actual = timeSpanTextBoxRule.Validate(timeSpanObject, CultureInfo.InvariantCulture);
        Assert.Equal(expectedResult, actual.IsValid);
    }

    [Theory]
    [InlineData("0:0:0", true)]
    [InlineData("0", true)]
    [InlineData("1:2", true)]
    [InlineData("1:2:3", false)]
    [InlineData("1:2:3:4", false)]
    [InlineData("-1:2:3", false)]
    [InlineData("-1:2:3:4", false)]
    [InlineData("1,2 3aaaa4", false)]
    [InlineData("-1", true)]
    [InlineData("-1 2", true)]
    [InlineData("abc1 2", true)]
    public void TimeSpanTextBoxRuleValidateWithIsShortTime(object timeSpanObject, bool expectedResult)
    {
        var timeSpanTextBoxRule = new TimeSpanTextBoxRule() { IsShortTime = true };
        var actual = timeSpanTextBoxRule.Validate(timeSpanObject, CultureInfo.InvariantCulture);
        Assert.Equal(expectedResult, actual.IsValid);
    }
}
public class TimeSpanUtilsTest
{
    [Theory]
    [InlineData("0", 0, 0, 0, 0)]
    [InlineData("1:2", 0, 1, 2, 0)]
    [InlineData("1:2:3", 0, 1, 2, 3)]
    [InlineData("1:2:3:4", 1, 2, 3, 4)]
    [InlineData("1,2 3aaaa4", 1, 2, 3, 4)]
    [InlineData("-1", 0, -1, 0, 0)]
    [InlineData("-1 2", 0, -1, -2, 0)]
    [InlineData("abc1 2", 0, 1, 2, 0)]
    public void ChangeTimeSpanStringToTimeSpan(string timeSpanString, int days, int hours, int minutes, int seconds)
    {
        var ts = new TimeSpan();
        var actual = TimeSpanUtils.GetTimeSpan(timeSpanString, ref ts);
        var expedtedTimeSpan = new TimeSpan(days, hours, minutes, seconds);
        Assert.Equal(expedtedTimeSpan, ts);
    }

    [Theory]
    [InlineData("0", true)]
    [InlineData("1:2", true)]
    [InlineData("1:2:3", true)]
    [InlineData("1:2:3:4", true)]
    [InlineData("1,2 3aaaa4", true)]
    [InlineData("-1", true)]
    [InlineData("-1 2", true)]
    [InlineData("abc1 2", true)]
    [InlineData("9999999999", false)]
    public void ChangeTimeSpanString_ReturnSuccessOrFail(string timeSpanString, bool expectedSuccessOrFail)
    {
        var ts = new TimeSpan();
        var actual = TimeSpanUtils.GetTimeSpan(timeSpanString, ref ts);
        Assert.Equal(expectedSuccessOrFail, actual);
    }
}
