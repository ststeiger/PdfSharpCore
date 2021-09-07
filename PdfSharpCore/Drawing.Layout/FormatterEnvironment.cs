namespace PdfSharpCore.Drawing.Layout
{
	internal class FormatterEnvironment
	{
		public XFont Font { get; set; }
		public XBrush Brush { get; set; }

		public double LineSpace { get; set; }
		public double CyAscent { get; set; }
		public double CyDescent { get; set; }
		public double SpaceWidth { get; set; }
	}
}