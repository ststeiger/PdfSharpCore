namespace PdfSharpCore.Drawing.Layout
{
	public class TextSegment
	{
		public XFont Font { get; set; }
		public XBrush Brush { get; set; }
		public string Text { get; set; }
		public double LineIndent { get; set; }
		public bool SkipParagraphAlignment { get; set; }

		public double LineSpace { get; set; }
		public double CyAscent { get; set; }
		public double CyDescent { get; set; }
		public double SpaceWidth { get; set; }
	}
}