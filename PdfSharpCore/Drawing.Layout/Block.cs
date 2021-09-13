using PdfSharpCore.Drawing.Layout.enums;

namespace PdfSharpCore.Drawing.Layout
{
	/// <summary>
	/// Represents a single word.
	/// </summary>
	internal class Block
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Block"/> class.
		/// </summary>
		/// <param name="text">The text of the block.</param>
		/// <param name="type">The type of the block.</param>
		/// <param name="width">The width of the text.</param>
		public Block(string text, BlockType type, double width)
		{
			Text = text;
			Type = type;
			Width = width;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Block"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public Block(BlockType type)
		{
			Type = type;
		}

		/// <summary>
		/// The text represented by this block.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// The type of the block.
		/// </summary>
		public BlockType Type { get; }

		/// <summary>
		/// The width of the text.
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// The location relative to the upper left corner of the layout rectangle.
		/// </summary>
		public XPoint Location { get; set; }

		/// <summary>
		/// The alignment of this line.
		/// </summary>
		public XParagraphAlignment Alignment { get; set; }

		/// <summary>
		/// A flag indicating that this is the last block that fits in the layout rectangle.
		/// </summary>
		public bool Stop { get; set; }

		/// <summary>
		/// Contains information about spacing of the font
		/// </summary>
		public FormatterEnvironment Environment { get; set; }

		/// <summary>
		/// Indent of the text when a new line is started
		/// </summary>
		public double LineIndent { get; set; }

		/// <summary>
		/// Skips block for alignment justify calculation, when its the first block in line
		/// </summary>
		public bool SkipParagraphAlignment { get; set; }

		/// <summary>
		///  Links this block with the next block
		/// </summary>
		public bool NextBlockBelongsToMe { get; set; }
	}
}