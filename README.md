# ThunderbirdFixMailsHeaders
Repair folders in Thunderbird. Program write in C# DotNet 4.7.2. Download from:
https://github.com/kamilmroczkowski/ThunderbirdFixMailsHeaders/tree/main/release/

Sometimes when moving Thunderbird profile to another Windows directory in profile, it may have problems with showing email headers - the situation looks like this:
![](/doc/thunderbird_problem.png)

When viewing raw file for example: C:\Users\user\AppData\Roaming\Thunderbird\Profiles\jg5ytr43u.default\Mail\Local Folders\ExampleFolder

Incorrect syntax in file (problem with: CRCRLF):

![](/doc/file_problem.jpg)

Repaired/correct file file have only CRLF:

![](/doc/file_correct.jpg)

To repair file you need to replace CRCR to CR - and this program do this.

Screenshot:

![](/doc/screenshot.jpg)
