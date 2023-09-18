# TimeSpanHelper
Convert TimeSpan to over 24 hours number.  
TimeSpan Converter, Rule for TextBox.

### TimeSpanToStringConverter & Rule
- Convert TimeSpan to string "Hours:Minutes:Seconds". Over 24 Hours Ok.
- Rules: Max, Min, IsShortTime.
- ConverterParameter Optional.　TimeSpan format strings. Replace {SumHours}, {Sign}. {SumHours} show negative hours.

### TimeSpanUtils
- 1 number : hours    "0" to "0:0:0" ,    "-1" to "-01:00:00"
- 2 numbers : hours, minutes    "1:2" to "01:02:00"
- 3 numbers : hours, minutes, seconds    "1:2:3" to "01:02:03"
- 4 numbers : days, hours, minutes, seconds    "1:2:3:4" to "1.02:03:04"
- Any char can be used as separator.    "1,2 3aaaa4" to "1.02:03:04"

## Version
1.2

## System Requirements
- .NET 6

## History
- 2021-03-26 Ver 1.0
- 2021-12-17 Ver 1.1
	- Added ValidationRules: IsShortTime
	- Changed StringFormat. Replace
- 2023-09-19 Ver 1.2
	- Changed to from .NET 5 to .NET 6
	- Fixed TimeSpanUtils.GetTimeSpan Method.

## Maintenance
Taisidô(Ganges) https://ganges.pro/

## License
Copyright (c) 2021-2023 Taisidô Mikihiro (Ganges)  
Released under the MIT license