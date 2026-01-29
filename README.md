# Genpact QA Home Assignment – Playwright Automation

C# + Playwright automation for the [Playwright (software)](https://en.wikipedia.org/wiki/Playwright_(software)) Wikipedia page: UI (POM) and API (MediaWiki Parse) tests with clean architecture and optional HTML report.

## Tech Stack

- **Language:** C# (.NET 8)
- **UI/API:** Microsoft.Playwright, HttpClient (MediaWiki Parse API)
- **Test framework:** NUnit
- **Bonus:** HTML report (ReportUnit from NUnit results)

## Solution Layout (Clean Architecture)

```
GenpactQA.Tests/
├── Core/           # Text normalization, unique word count
├── Api/            # MediaWiki Parse API client
├── Pages/          # Page Object Model (Wikipedia page)
├── Tests/          # Task 1, 2, 3 test classes
├── PlaywrightSetup.cs   # One-time browser install
└── GenpactQA.Tests.csproj
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- First run installs Chromium via Playwright (see below)

## Run Tests

```bash
cd GenpactQAHomeAssignment-1
dotnet restore
dotnet build
dotnet test --logger "console;verbosity=normal"
```

First run will install Playwright Chromium (one-time). Run tests with HTML result output:

```bash
dotnet test --logger "console;verbosity=normal" -- NUnit.WriteXmlResults=TestResults.xml
```

## HTML Report (Bonus)

Run tests with TRX (XML) output, then open or convert to HTML:

```bash
dotnet test --logger "trx;LogFileName=TestResults.trx"
# Open TestResults/TestResults.trx in Azure DevOps or use any TRX/NUnit-to-HTML converter
```

Or with NUnit XML:

```bash
dotnet test -- NUnit.WriteXmlResults=TestResults.xml
# Use ReportUnit, nunit-html-report, or any NUnit 3 XML-to-HTML tool to generate an HTML report
```

## Tasks Implemented

### Task 1 – Debugging features: UI vs API word count

- **UI (POM):** `WikipediaPlaywrightPage.GetDebuggingFeaturesSectionTextAsync()` – navigates to the page and reads the “Debugging features” section text.
- **API:** `MediaWikiApiClient.GetDebuggingFeaturesSectionAsync()` – uses MediaWiki Parse API (`action=parse`, `prop=text`, `section=5` for “Debugging features”).
- **Core:** `TextNormalizer.Normalize()` and `TextNormalizer.CountUniqueWords()` – strip HTML, lowercase, remove punctuation, collapse spaces; count distinct words.
- **Assertion:** Unique word count from UI equals unique word count from API (after normalization).

### Task 2 – Microsoft development tools: technology names are links

- **UI:** Go to the “Microsoft development tools” section (under “Debugging features” on the page, at the bottom).
- **Validation:** All list items under that section are checked so that the visible “technology name” is a text link (the main content of the list item is an `<a>`).
- **Failure:** If any technology name is not a link, the test fails and reports which names are not links.

### Task 3 – Color (beta): set Dark and verify

- **UI:** Open the Appearance menu (top-right, e.g. spectacles / “Appearance”) and in the “Color (beta)” section select **Dark**.
- **Validation:** Assert that the theme actually changed (e.g. `html` has a dark-theme class such as `skin-theme-clientpref-night` or equivalent).

## License

MIT.
