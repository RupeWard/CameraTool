/**
 * Class - TinyXmlReader
 *
 * A very simple XML reader to replace the Mono System.Xml package that addes 1MB to your project file size.
 * It is VERY simple, just forward-reads through the entire XML tree, so it's really only useful for loading simple data into arrays / hastables / dictionaries.
 *
 * Downloaded from - http://www.unifycommunity.com/wiki/index.php?title=TinyXmlReader
 *
 * Notes - "You can use Mono's System.Xml for handling XML files but this requires including the System.Xml dll into your Unity program which increases its file size by about 1 MB. Not to mention the lack of documentation for using System.Xml on UnityScript.
 * I found rolling my own XML parser was easier. Note however that this is a really simple XML parser, it doesn't recognize attributes (I did not implement it simply because I don't use XML attributes)."
 *
 * Usage:
 * string text = "";
 * TextAsset xml = Resources.Load("XML/asset", typeof(TextAsset)) as TextAsset;
 * TinyXmlReader reader = new TinyXmlReader(xml.ToString());
 * while (reader.Read())
 * {
 *      if (reader.isOpeningTag)
 *      {
 *          text += (reader.tagName + " \"" + reader.content + "\"\n");
 *      }
 *      if (reader.tagName == "Skills" && reader.isOpeningTag)
 *      {
 *          while(reader.Read("Skills")) // read as long as not encountering the closing tag for Skills
 *          {
 *              if (reader.isOpeningTag)
 *              {
 *                 text += ("Skill: " + reader.tagName + " \"" + reader.content + "\"\n");
 *              }
 *          }
 *      }
 * }
 */

using UnityEngine;
using System.Collections;

public class TinyXmlReader
{
    private string xmlString = "";
    private int idx = 0;

    public TinyXmlReader(string newXmlString)
    {
        xmlString = newXmlString;
    }

    public string tagName = "";
    public bool isOpeningTag = false;
    public string content = "";

    public bool Read()
    {
        idx = xmlString.IndexOf("<", idx);

        if (idx == -1)
        {
            return false;
        }

        ++idx;

        int endOfTag = xmlString.IndexOf(">", idx);

        if (endOfTag == -1)
        {
            return false;
        }

        tagName = xmlString.Substring(idx, endOfTag - idx);

        idx = endOfTag;

        // check if a closing tag
        if (tagName.StartsWith("/"))
        {
            isOpeningTag = false;
            tagName = tagName.Remove(0, 1); // remove the slash
        }
        else
        {
            isOpeningTag = true;
        }

        // if an opening tag, get the content
        if (isOpeningTag)
        {
            int startOfCloseTag = xmlString.IndexOf("<", idx);

            if (startOfCloseTag == -1)
            {
                return false;
            }

            content = xmlString.Substring(idx + 1, startOfCloseTag - idx - 1);
            content = content.Trim();
        }

        return true;
    }

    // returns false when the endingTag is encountered
    public bool Read(string endingTag)
    {
        bool retVal = Read();

        if (tagName == endingTag && !isOpeningTag)
        {
            retVal = false;
        }

        return retVal;
    }

    public bool TagIsOpeningOne(string tName)
    {
        return (this.isOpeningTag && this.tagName == tName);
    }

    public bool TagHasContent(string tName)
    {
        return (TagIsOpeningOne(tName) && this.content.Length > 0);
    }
}