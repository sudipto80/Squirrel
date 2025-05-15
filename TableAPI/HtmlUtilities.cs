namespace TableAPI;

public class HtmlUtilities
{
    /// <summary>
    /// Deletes the tags from a HTML line
    /// </summary>
    /// <param name="codeLine">HTML code from which tags has to be removed</param>
    /// <param name="exceptTheseTags">Remove all tags except this one</param>
    /// <returns></returns>
    public static string StripTags(string codeLine, List<string> exceptTheseTags)
    {
        string tag = string.Empty;
        string html = string.Empty;
        var tags = new List<string>();
        for (int i = 0; i < codeLine.Length; i++)
        {
            tag = string.Empty;
            if (codeLine[i] == '<')
            {
                i++;
                do
                {
                    tag = tag + codeLine[i];
                    i++;
                } while (codeLine[i] != '>');

                tags.Add("<" + tag + ">");
            }
        }

        tags.RemoveAll(t => exceptTheseTags.Contains(t));
        foreach (string k in codeLine.Split(tags.ToArray(), StringSplitOptions.RemoveEmptyEntries))
            html = html + k + " ";
        //the html
        return html;
    }
}