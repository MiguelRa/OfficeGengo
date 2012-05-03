OfficeGengoAddins (source code)

This package contains all the source code of OfficeGengoAddins suite. It's a Visual Studio 2010 solution. It includes the following subpackages:

 - JobsViewerControl: windows form user control that encapsulates the handling of jobs grid UI.

 - MyGengoTranslator: class library that provides common behaviour for WordGengo and OutlookGengo. This is the core of OfficeGengoAddins, contains the main form with all the options for translating.

 - OutlookGengoAddIn: Outlook addin. It extends the Outlook interface and uses MyGengoTranslator.

 - WordGengoAddIn: Word addin. It extends the Word interface and uses MyGengoTranslator.