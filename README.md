# ThunderbirdFixMailsHeaders
Repair folders in Thunderbird

Sometimes when we move profile Thunderbird to another Windows local folders in profile have problems with shows mails - like this:
![](/doc/thunderbird_problem.png)

When we view raw file example: C:\Users\user\AppData\Roaming\Thunderbird\Profiles\jg5ytr43u.default\Mail\Local Folders\ExampleFolder

Incorrect syntax in file (problem with: CRCRLF):

![](/doc/file_problem.jpg)

Repair/correct file have only CRLF:

![](/doc/file_correct.jpg)

Repair file is replace CRCR to CR - and this program do this.

Screenshot:

![](/doc/screenshot.jpg)
