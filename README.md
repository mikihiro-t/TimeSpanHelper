# TimeSpanHelper
Convert TimeSpan to over 24 hours number.
TimeSpan Converter, Rule for TextBox.

### TimeSpanToStringConverter & Rule
- Convert TimeSpan to string "Hours:Minutes:Seconds". Over 24 Hours Ok.

### TimeSpanUtils
- 1 number : hours    "0" to "0:0:0" ,    "-1" to "-01:00:00"
- 2 numbers : hours, minutes    "1:2" to "01:02:00"
- 3 numbers : hours, minutes, seconds    "1:2:3" to "01:02:03"
- 4 numbers : days, hours, minutes, seconds    "1:2:3:4" to "1.02:03:04"
- Any char can be used as separator.    "1,2 3aaaa4" to "1.02:03:04"

## System Requirements
- .NET 5

## Maintenance
Taishido(Ganges) https://ganges.pro/

## License
MIT