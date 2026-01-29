using Microsoft.Playwright;

namespace GenpactQA.Tests.Pages;

/// <summary>
/// Page Object for Wikipedia Playwright (software) article.
/// </summary>
public class WikipediaPlaywrightPage
{
    private readonly IPage _page;
    private const string BaseUrl = "https://en.wikipedia.org/wiki/Playwright_(software)";

    public WikipediaPlaywrightPage(IPage page)
    {
        _page = page;
    }

    public async Task GotoAsync()
    {
        await _page.GotoAsync(BaseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    /// <summary>
    /// Gets the visible text of the "Debugging features" section (from heading until next same-level h3).
    /// </summary>
    public async Task<string> GetDebuggingFeaturesSectionTextAsync()
    {
        // First approach: Get the heading and try to find the associated content container
        var heading = _page.Locator("#Debugging_features");
        var headingText = await heading.InnerTextAsync();
        
        // Try to find the content by looking at the structure
        // Wikipedia typically puts the content in sibling elements after the heading
        var contentText = await _page.EvaluateAsync<string>(@"() => {
            const heading = document.querySelector('#Debugging_features');
            if (!heading) return '';
            
            // Get the parent and then find the next sibling elements that are content
            let text = '';
            let current = heading.parentElement ? heading.parentElement.nextElementSibling : null;
            
            // Collect text from the next few siblings until we hit another H3
            let count = 0;
            while (current && count < 10) {  // Limit to prevent infinite loop
                if (current.tagName === 'H3') {
                    // Check if it's a different section
                    if (current.querySelector('#Debugging_features') === null) {
                        break;
                    }
                }
                
                if (current.innerText) {
                    text += current.innerText + '\n';
                }
                
                current = current.nextElementSibling;
                count++;
            }
            
            return text.trim();
        }");
        
        return (headingText + "\n" + contentText).Trim();
    }

    /// <summary>
    /// Returns the section for "Microsoft development tools" (nearest ancestor that contains both the heading and list items).
    /// </summary>
    public ILocator GetMicrosoftDevelopmentToolsSection()
    {
        // Look for the text "Microsoft development tools" within the page content, not necessarily as a heading
        return _page.Locator(":text('Microsoft development tools')").Locator("xpath=../.."); // Go up a couple of levels to get the container
    }

    /// <summary>
    /// Gets all technology name elements under Microsoft development tools (links in lists).
    /// We need to validate each "technology name" is a link. The section contains many list items; each item text (technology name) should be inside an anchor.
    /// </summary>
    public async Task<IReadOnlyList<(string Name, bool IsLink)>> GetMicrosoftDevelopmentToolsTechnologyNamesAsync()
    {
        var section = GetMicrosoftDevelopmentToolsSection();
        var result = new List<(string, bool)>();

        // All list items under this section - each li may contain a link or plain text
        var listItems = section.Locator("li");
        var count = await listItems.CountAsync();

        for (var i = 0; i < count; i++)
        {
            var li = listItems.Nth(i);
            var text = (await li.InnerTextAsync()).Trim();
            if (string.IsNullOrWhiteSpace(text)) continue;

            // Technology "name" is the visible text; it should be a link (anchor). Check if the first link in the li has the same text or if the li's direct link contains the name.
            var link = li.Locator("a").First;
            var hasLink = await link.CountAsync() > 0;
            var linkText = hasLink ? (await link.InnerTextAsync()).Trim() : "";
            // Consider it a "technology name" if it's the main text of the item (first link or only content)
            if (hasLink && string.Equals(text, linkText, StringComparison.OrdinalIgnoreCase))
                result.Add((text, true));
            else if (hasLink)
                result.Add((text, true)); // li contains a link - could be "Code" link with text "Code"
            else
                result.Add((text, false));
        }

        return result;
    }

    /// <summary>
    /// Alternative: get all anchors under Microsoft development tools and ensure every list item's primary content is a link.
    /// Assignment: "all the technology names under this section are a text link". So we need every technology name to be a link. Get all lis, for each li check that the main text comes from an anchor.
    /// </summary>
    public async Task<IReadOnlyList<(string Name, bool IsLink)>> GetTechnologyNamesAsLinksAsync()
    {
        var section = GetMicrosoftDevelopmentToolsSection();
        var result = new List<(string, bool)>();

        var links = section.Locator("a[href]");
        var count = await links.CountAsync();
        for (var i = 0; i < count; i++)
        {
            var a = links.Nth(i);
            var href = await a.GetAttributeAsync("href");
            var text = (await a.InnerTextAsync()).Trim();
            if (string.IsNullOrWhiteSpace(text) || href == null || href.StartsWith("#")) continue;
            // External or wiki link - count as technology name that is a link
            result.Add((text, true));
        }

        // Also find any text nodes that are NOT inside a link (would be failures). So we need: all list item content that is "technology name" must be inside <a>.
        var lis = section.Locator("li");
        var liCount = await lis.CountAsync();
        for (var i = 0; i < liCount; i++)
        {
            var li = lis.Nth(i);
            var firstChild = li.Locator("> a").First;
            var hasDirectLink = await firstChild.CountAsync() > 0;
            var innerText = (await li.InnerTextAsync()).Trim();
            if (string.IsNullOrWhiteSpace(innerText)) continue;
            // If the only content is the link text, we're good. If there is text that is not from a link, fail.
            var directLinkText = hasDirectLink ? (await firstChild.InnerTextAsync()).Trim() : "";
            var isOnlyLink = hasDirectLink && string.Equals(innerText, directLinkText, StringComparison.OrdinalIgnoreCase);
            if (!isOnlyLink && !hasDirectLink)
                result.Add((innerText, false));
        }

        return result;
    }

    /// <summary>
    /// Validates that under Microsoft development tools, every list item's main text is a link. Returns (allAreLinks, failingNames).
    /// </summary>
    public async Task<(bool AllAreLinks, IReadOnlyList<string> NotLinkNames)> ValidateMicrosoftDevelopmentToolsTechnologyNamesAreLinksAsync()
    {
        var section = GetMicrosoftDevelopmentToolsSection();
        var notLinkNames = new List<string>();

        var lis = section.Locator("li");
        var count = await lis.CountAsync();

        for (var i = 0; i < count; i++)
        {
            var li = lis.Nth(i);
            var text = (await li.InnerTextAsync()).Trim();
            if (string.IsNullOrWhiteSpace(text)) continue;

            var firstLink = li.Locator("a").First;
            var hasLink = await firstLink.CountAsync() > 0;
            if (!hasLink)
            {
                notLinkNames.Add(text);
                continue;
            }

            var linkText = (await firstLink.InnerTextAsync()).Trim();
            // Technology name is a link if the first/main link text matches the list item text (possibly with newlines trimmed)
            var normalizedLi = text.Replace("\n", " ").Trim();
            var normalizedLink = linkText.Replace("\n", " ").Trim();
            var isLink = !string.IsNullOrEmpty(normalizedLink) &&
                         (string.Equals(normalizedLi, normalizedLink, StringComparison.OrdinalIgnoreCase) ||
                          normalizedLi.StartsWith(normalizedLink, StringComparison.OrdinalIgnoreCase));
            if (!isLink)
            {
                var allLinks = li.Locator("a");
                var linkCount = await allLinks.CountAsync();
                var anyLinkMatches = false;
                for (var j = 0; j < linkCount; j++)
                {
                    var lt = (await allLinks.Nth(j).InnerTextAsync()).Trim();
                    if (string.Equals(normalizedLi, lt, StringComparison.OrdinalIgnoreCase))
                    {
                        anyLinkMatches = true;
                        break;
                    }
                }
                if (!anyLinkMatches)
                    notLinkNames.Add(text);
            }
        }

        return (notLinkNames.Count == 0, notLinkNames);
    }

/// <summary>
/// Opens the Appearance menu (top-right, e.g. spectacles icon) and sets Color to "Dark".
/// </summary>
public async Task OpenAppearanceAndSetColorToDarkAsync()
{
    // Try multiple selectors for the appearance dropdown/menu
    var appearanceSelectors = new[]
    {
        "#vector-appearance-dropdown",
        "[id^='p-appearance']",
        "a[title='Appearance']",
        ".vector-dropdown[title*='Appearance' i]",
        "[aria-label*='Appearance' i]"
    };

    ILocator appearance = null!;
    
    // Try each selector until we find one that works
    foreach (var selector in appearanceSelectors)
    {
        var element = _page.Locator(selector).First;
        if (await element.IsVisibleAsync())
        {
            appearance = element;
            break;
        }
    }
    
    // If we still don't have an element, try a more general approach
    if (appearance == null)
    {
        // Look for any dropdown or menu in the header area
        var headerDropdowns = await _page.Locator("header .vector-dropdown, #mw-panel .vector-dropdown").AllAsync();
        foreach (var dropdown in headerDropdowns)
        {
            if (await dropdown.IsVisibleAsync())
            {
                appearance = dropdown;
                break;
            }
        }
    }
    
    if (appearance == null)
    {
        throw new InvalidOperationException("Could not find appearance dropdown/menu");
    }
    
    await appearance.ClickAsync();
    
    // Wait briefly for menu to appear
    await _page.WaitForTimeoutAsync(1000);

    // Try multiple approaches to find and click the Dark theme option
    var darkThemeClicked = false;
    
    // Approach 1: Look for a button with "Dark" text
    try
    {
        var darkButton = _page.GetByRole(AriaRole.Button, new() { Name = "Dark" });
        if (await darkButton.IsVisibleAsync())
        {
            await darkButton.ClickAsync();
            darkThemeClicked = true;
        }
    }
    catch
    {
        // Ignore and try next approach
    }
    
    // Approach 2: Look for any element with "Dark" text
    if (!darkThemeClicked)
    {
        try
        {
            var darkElement = _page.Locator(":text('Dark'), :text('dark')").First;
            if (await darkElement.IsVisibleAsync())
            {
                await darkElement.ClickAsync();
                darkThemeClicked = true;
            }
        }
        catch
        {
            // Ignore and try next approach
        }
    }
    
    // Approach 3: Look for a radio button or checkbox for "Dark"
    if (!darkThemeClicked)
    {
        try
        {
            var darkRadio = _page.Locator("input[type='radio'][value*='dark' i], input[type='checkbox'][value*='dark' i]");
            if (await darkRadio.First.IsVisibleAsync())
            {
                await darkRadio.First.ClickAsync();
                darkThemeClicked = true;
            }
        }
        catch
        {
            // Ignore and try next approach
        }
    }
    
    if (!darkThemeClicked)
    {
        throw new InvalidOperationException("Could not find or click Dark theme option");
    }
}

    /// <summary>
    /// Checks if the current theme is Dark (html class set by Vector skin).
    /// </summary>
    public async Task<bool> IsDarkThemeActiveAsync()
    {
        var html = await _page.Locator("html").GetAttributeAsync("class");
        return html != null && (html.Contains("skin-theme-clientpref-night") || html.Contains("client-dark") || html.Contains("skin-theme-dark"));
    }
}
