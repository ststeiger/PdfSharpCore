using System;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.Rendering;

namespace MigraDocCore.Rendering.UnitTest
{
  /// <summary>
  /// Summary description for TestParagraphIterator.
  /// </summary>
  public class TestParagraphIterator
  {
    public TestParagraphIterator()
    {

    }

    public static string GetIterators(Paragraph paragraph)
    {
      ParagraphIterator iter = new ParagraphIterator(paragraph.Elements);
      iter = iter.GetFirstLeaf();
      string retString = "";
      while (iter != null)
      {
        retString += "[" + iter.Current.GetType().Name + ":]";
        if (iter.Current is Text)
          retString += ((Text)iter.Current).Content;

        iter = iter.GetNextLeaf();
      }
      return retString;
    }

    public static string GetBackIterators(Paragraph paragraph)
    {
      ParagraphIterator iter = new ParagraphIterator(paragraph.Elements);
      iter = iter.GetLastLeaf();
      string retString = "";
      while (iter != null)
      {
        retString += "[" + iter.Current.GetType().Name + ":]";
        if (iter.Current is Text)
          retString += ((Text)iter.Current).Content;

        iter = iter.GetPreviousLeaf();
      }
      return retString;
    }

  }
}
