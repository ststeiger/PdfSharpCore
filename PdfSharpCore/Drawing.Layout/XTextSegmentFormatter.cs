using PdfSharpCore.Drawing.Layout.enums;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfSharpCore.Drawing.Layout
{
	public class XTextSegmentFormatter
	{
		private readonly XGraphics _gfx;

		/// <summary>
		/// Initializes a new instance of the <see cref="XTextSegmentFormatter"/> class.
		/// </summary>
		public XTextSegmentFormatter(XGraphics gfx)
		{
			if (gfx == null)
			{
				throw new ArgumentNullException("gfx");
			}

			_gfx = gfx;
		}

		/// <summary>
		/// Gets or sets the alignment of the text.
		/// </summary>
		public XParagraphAlignment Alignment { get; set; }

		/// <summary>
		/// Draws the text.
		/// </summary>
		/// <param name="text">The text to be drawn.</param>
		/// <param name="font">The font.</param>
		/// <param name="brush">The text brush.</param>
		/// <param name="layoutRectangle">The layout rectangle.</param>
		public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle)
		{
			var textSegments = new List<TextSegment>
			{
				new TextSegment { Font = font, Brush = brush, Text = text }
			};

			DrawString(textSegments, layoutRectangle, XStringFormats.TopLeft);
		}

		/// <summary>
		/// Draws the text.
		/// </summary>
		/// <param name="text">The text to be drawn.</param>
		/// <param name="font">The font.</param>
		/// <param name="brush">The text brush.</param>
		/// <param name="layoutRectangle">The layout rectangle.</param>
		/// <param name="format">The format. Must be <c>XStringFormat.TopLeft</c></param>
		public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
		{
			var textSegments = new List<TextSegment>
			{
				new TextSegment { Font = font, Brush = brush, Text = text }
			};

			DrawString(textSegments, layoutRectangle, format);
		}

		/// <summary>
		/// Draws the text. 
		/// </summary>
		/// <param name="textSegments">The texts to be drawn with font and color information</param>
		/// <param name="layoutRectangle">The layout rectangle.</param>
		public void DrawString(IEnumerable<TextSegment> textSegments, XRect layoutRectangle)
		{
			DrawString(textSegments, layoutRectangle, XStringFormats.TopLeft);
		}

		/// <summary>
		/// Draws the text. 
		/// </summary>
		/// <param name="textSegments">The texts to be drawn with font and color information.</param>
		/// <param name="layoutRectangle">The layout rectangle.</param>
		/// <param name="format">The format. Must be <c>XStringFormat.TopLeft</c></param>
		public void DrawString(IEnumerable<TextSegment> textSegments, XRect layoutRectangle, XStringFormat format)
		{
			ProcessTextSegments(
				textSegments,
				layoutRectangle,
				format,
				(block, dx, dy) => _gfx.DrawString(block.Text, block.Environment.Font, block.Environment.Brush, dx + block.Location.X, dy + block.Location.Y),
				false
			);
		}

		/// <summary>
		/// Calculates the size of the given text
		/// </summary>
		/// <param name="text">The text to be drawn.</param>
		/// <param name="font">The font.</param>
		/// <param name="brush">The text brush.</param>
		/// <param name="width">Max text width</param>
		/// <returns></returns>
		public XSize CalculateTextSize(string text, XFont font, XBrush brush, double width)
		{
			return CalculateTextSize(text, font, brush, width, XStringFormats.TopLeft);
		}

		/// <summary>
		/// Calculates the size of the given text
		/// </summary>
		/// <param name="text">The text to be drawn.</param>
		/// <param name="font">The font.</param>
		/// <param name="brush">The text brush.</param>
		/// <param name="width">Max text width</param>
		/// <param name="format">The format. Must be <c>XStringFormat.TopLeft</param>
		/// <returns></returns>
		public XSize CalculateTextSize(string text, XFont font, XBrush brush, double width, XStringFormat format)
		{
			var textSegments = new List<TextSegment>
			{
				new TextSegment { Font = font, Brush = brush, Text = text }
			};

			return CalculateTextSize(textSegments, width, format);
		}

		/// <summary>
		/// Calculates the size of the given text
		/// </summary>
		/// <param name="textSegments">The texts to be drawn with font and color information.</param>
		/// <param name="width">Max text width</param>
		/// <returns></returns>
		public XSize CalculateTextSize(IEnumerable<TextSegment> textSegments, double width)
		{
			return CalculateTextSize(textSegments, width, XStringFormats.TopLeft);
		}

		/// <summary>
		/// Calculates the size of the given text
		/// </summary>
		/// <param name="textSegments">The texts to be drawn with font and color information.</param>
		/// <param name="width">Max text width</param>
		/// <param name="format">The format. Must be <c>XStringFormat.TopLeft</param>
		/// <returns></returns>
		public XSize CalculateTextSize(IEnumerable<TextSegment> textSegments, double width, XStringFormat format)
		{
			var layoutRectangle = new XRect(0, 0, width, 100000000);
			var blocks = new List<Block>();

			ProcessTextSegments(textSegments, layoutRectangle, format, (block, dx, dy) => blocks.Add(block), true);

			var height = blocks.Any()
				? blocks.Max(b => b.Location.Y)
				: 0;
			var maxLineHeight = 0.0;
			for (int i = blocks.Count - 1; i >= 0; i--)
			{
				if (blocks[i].Type == BlockType.LineBreak)
				{
					break;
				}

				maxLineHeight = Math.Max(maxLineHeight, blocks[i].Environment.LineSpace);
			}

			var calculatedWith = blocks.Any()
				? blocks.Max(b => b.Location.X + b.Width)
				: width;

			if (width < calculatedWith)
			{
				calculatedWith = width;
			}

			return new XSize(calculatedWith, height + maxLineHeight);
		}

		private void ProcessTextSegments(IEnumerable<TextSegment> textSegments, XRect layoutRectangle, XStringFormat format, Action<Block, double, double> applyBlock, bool applyBlockIfLineBreak)
		{
			if (textSegments.All(ts => string.IsNullOrEmpty(ts.Text)))
			{
				return;
			}

			if (textSegments.Any(ts => ts.Font == default))
			{
				throw new ArgumentNullException("font");
			}

			if (textSegments.Any(ts => ts.Brush == default))
			{
				throw new ArgumentNullException("brush");
			}

			if (format.Alignment != XStringAlignment.Near || format.LineAlignment != XLineAlignment.Near)
			{
				throw new ArgumentException("Only TopLeft alignment is currently implemented.");
			}

			foreach (var segment in textSegments)
			{
				SetFontSpacings(segment);
			}

			var blocks = CreateBlocks(textSegments);
			var blockUnits = new List<List<Block>>();
			var currentBlockUnit = new List<Block>();
			foreach (var block in blocks)
			{
				currentBlockUnit.Add(block);

				if (block.Stop || block.Type == BlockType.LineBreak)
				{
					blockUnits.Add(currentBlockUnit);
					currentBlockUnit = new List<Block>();
				}
			}

			if (!blocks.Last().Stop && blocks.Last().Type != BlockType.LineBreak)
			{
				blockUnits.Add(currentBlockUnit);
			}

			CreateLayout(blockUnits, layoutRectangle);

			for (int index = 0; index < blockUnits.Count; index++)
			{
				var blockUnit = blockUnits[index];
				var maxCyAscend = blockUnit.Max(b => b.Environment.CyAscent);
				var dx = layoutRectangle.Location.X;
				var dy = layoutRectangle.Location.Y + maxCyAscend;

				// Check all blocks of the current line in order to move all blocks of the next lines down,
				// when the first block of the current line has not the max cy ascent of the whole line
				if (!blockUnit.All(b => b.Environment.CyAscent == maxCyAscend))
				{
					for (var indexSiblings = index + 1; indexSiblings < blockUnits.Count; indexSiblings++)
					{
						blockUnits[indexSiblings].ForEach(b => b.Location += new XSize(0, maxCyAscend - blockUnit.First().Environment.CyAscent));
					}
				}

				foreach (var block in blockUnit)
				{
					if (block.Stop)
					{
						break;
					}

					if (block.Type == BlockType.LineBreak && !applyBlockIfLineBreak)
					{
						continue;
					}

					applyBlock(block, dx, dy);
				}
			}
		}

		private List<Block> CreateBlocks(IEnumerable<TextSegment> textSegments)
		{
			var blocks = new List<Block>();

			foreach (var textSegment in textSegments)
			{
				if (string.IsNullOrEmpty(textSegment.Text) && !(textSegment.Text ?? "").Contains(Chars.LF))
				{
					continue;
				}

				// Check whether the current block belongs to the last block
				if (blocks.Any() && !textSegment.Text.StartsWith(" "))
				{
					blocks.Last().NextBlockBelongsToMe = true;
				}

				var length = textSegment.Text.Length;
				var inNonWhiteSpace = false;
				var startIndex = 0;
				var blockLength = 0;

				for (var idx = 0; idx < length; idx++)
				{
					char ch = textSegment.Text[idx];

					// Treat CR and CRLF as LF
					if (ch == Chars.CR)
					{
						if (idx < length - 1 && textSegment.Text[idx + 1] == Chars.LF)
						{
							idx++;
						}

						ch = Chars.LF;
					}

					if (ch == Chars.LF)
					{
						if (blockLength != 0)
						{
							var token = textSegment.Text.Substring(startIndex, blockLength);
							var block = new Block(token, BlockType.Text, _gfx.MeasureString(token, textSegment.Font).Width);
							SetFormatterEnvironment(block, textSegment);
							block.LineIndent = textSegment.LineIndent;
							block.SkipParagraphAlignment = textSegment.SkipParagraphAlignment;
							blocks.Add(block);
						}

						startIndex = idx + 1;
						blockLength = 0;

						var lineBreakBlock = new Block(BlockType.LineBreak);
						SetFormatterEnvironment(lineBreakBlock, textSegment);
						blocks.Add(lineBreakBlock);
					}
					else if (char.IsWhiteSpace(ch))
					{
						if (inNonWhiteSpace)
						{
							var token = textSegment.Text.Substring(startIndex, blockLength).Trim();
							var block = new Block(token, BlockType.Text, _gfx.MeasureString(token, textSegment.Font).Width);
							SetFormatterEnvironment(block, textSegment);
							block.LineIndent = textSegment.LineIndent;
							block.SkipParagraphAlignment = textSegment.SkipParagraphAlignment;
							blocks.Add(block);
							startIndex = idx + 1;
							blockLength = 0;
						}
						else
						{
							blockLength++;
						}
					}
					else
					{
						inNonWhiteSpace = true;
						blockLength++;
					}
				}

				if (blockLength != 0)
				{
					var token = textSegment.Text.Substring(startIndex, blockLength);
					var block = new Block(token, BlockType.Text, _gfx.MeasureString(token, textSegment.Font).Width);
					block.LineIndent = textSegment.LineIndent;
					block.SkipParagraphAlignment = textSegment.SkipParagraphAlignment;
					SetFormatterEnvironment(block, textSegment);
					blocks.Add(block);
				}
			}

			return blocks;
		}

		private void CreateLayout(List<List<Block>> blockUnits, XRect layoutRectangle)
		{
			var rectWidth = layoutRectangle.Width;
			var rectHeight = layoutRectangle.Height - blockUnits.First().First().Environment.CyAscent - blockUnits.Last().Last().Environment.CyDescent;
			var x = 0.0;
			var y = 0.0;

			foreach (var blockUnit in blockUnits)
			{
				var count = blockUnit.Count;
				var firstIndex = 0;
				var currentMaxLineSpace = 0.0;
				var currentMaxCyDescent = 0.0;
				var currentLineBlocks = new List<Block>();
				var startLineSpace = blockUnit[0].Environment.LineSpace;
				var startCyDescent = blockUnit[0].Environment.CyDescent;

				for (int idx = 0; idx < count; idx++)
				{
					var block = blockUnit[idx];
					if (block.Type == BlockType.LineBreak)
					{
						if (Alignment == XParagraphAlignment.Justify)
						{
							blockUnit[firstIndex].Alignment = XParagraphAlignment.Left;
						}

						AlignLine(blockUnit, firstIndex, idx - 1, rectWidth);
						firstIndex = idx + 1;
						x = 0;

						startLineSpace = (idx + 1) < count
							? blockUnit[idx + 1].Environment.LineSpace
							: block.Environment.LineSpace;
						startCyDescent = (idx + 1) < count
							? blockUnit[idx + 1].Environment.CyDescent
							: block.Environment.CyDescent;

						currentMaxLineSpace = startLineSpace;
						currentMaxCyDescent = startCyDescent;

						y += currentMaxLineSpace;
						currentLineBlocks.Clear();

						if (y > rectHeight)
						{
							block.Stop = true;

							break;
						}

						// necessary to correctly calculate closing line breaks
						block.Location = new XPoint(0, y);
					}
					else
					{
						double width = block.Width;

						if (x == 0.0)
						{
							x += block.LineIndent;
						}

						if (x + width <= rectWidth || x == 0.0)
						{
							// if the font style is set to "underline", we don't want a underlined space character 
							width = RemovedLeadingSpace(block, width);
							block.Location = new XPoint(x, y);
							x += width;
							if (!block.NextBlockBelongsToMe)
							{
								// The current and the next block are treated as one unit, so there is no space between them
								x += block.Environment.SpaceWidth;
							}

							currentLineBlocks.Add(block);

							currentMaxLineSpace = Math.Max(block.Environment.LineSpace, currentMaxLineSpace);
							currentMaxCyDescent = Math.Max(block.Environment.CyDescent, currentMaxCyDescent);
						}
						else
						{
							// if the previous blocks are linked to the current block, all linked blocks have to be moved to the next line
							while (idx > 0 && blockUnit[idx - 1].NextBlockBelongsToMe)
							{
								idx--;
								currentLineBlocks.RemoveAt(currentLineBlocks.Count - 1);
								block = blockUnit[idx];
								width = block.Width;
							}
							
							AlignLine(blockUnit, firstIndex, idx - 1, rectWidth);
							firstIndex = idx;

							if (currentMaxLineSpace != startLineSpace)
							{
								y += -startLineSpace + currentMaxLineSpace;
								currentLineBlocks.ForEach(b => b.Location = new XPoint(b.Location.X, y));
							}

							startLineSpace = block.Environment.LineSpace;
							startCyDescent = block.Environment.CyDescent;

							if (startLineSpace < currentMaxLineSpace)
							{
								var cyDescentDiff = currentMaxCyDescent - startCyDescent;
								y += cyDescentDiff;
							}

							currentMaxLineSpace = startLineSpace;
							currentMaxCyDescent = startCyDescent;

							y += currentMaxLineSpace;
							currentLineBlocks.Clear();

							if (y > rectHeight)
							{
								block.Stop = true;

								break;
							}

							// A new line must not start with a space character
							width = RemovedLeadingSpace(block, width);
							block.Location = new XPoint(block.LineIndent, y);
							x = block.LineIndent + width;
							if (!block.NextBlockBelongsToMe)
							{
								// The current and the next block are treated as one unit, so there is no space between them
								x += block.Environment.SpaceWidth;
							}
							currentLineBlocks.Add(block);
						}
					}
				}

				if (firstIndex < count && Alignment != XParagraphAlignment.Justify)
				{
					AlignLine(blockUnit, firstIndex, count - 1, rectWidth);
				}
			}
		}

		private double RemovedLeadingSpace(Block block, double width)
		{
			while (block.Text.StartsWith(" "))
			{
				block.Text = block.Text.Substring(1);
				block.Width -= block.Environment.SpaceWidth;
				width -= block.Environment.SpaceWidth;
			}

			return width;
		}

		/// <summary>
		/// Align center, right, or justify.
		/// </summary>
		private void AlignLine(IList<Block> blockUnit, int firstIndex, int lastIndex, double layoutWidth)
		{
			var firstBlock = blockUnit[firstIndex];
			var blockAlignment = firstBlock.Alignment;

			if (Alignment == XParagraphAlignment.Left || blockAlignment == XParagraphAlignment.Left)
			{
				return;
			}

			var count = lastIndex - firstIndex + 1;
			if (count == 0)
			{
				return;
			}

			var totalWidth = firstBlock.LineIndent;
			if (Alignment == XParagraphAlignment.Justify)
			{
				// Skip not movable leading blocks
				for (int idx = firstIndex; idx <= lastIndex; idx++)
				{
					if (!blockUnit[idx].SkipParagraphAlignment && !blockUnit[idx].NextBlockBelongsToMe)
					{
						firstIndex = idx;

						break;
					}
					else
					{
						count--;
						layoutWidth -= blockUnit[idx].Width + (blockUnit[idx].NextBlockBelongsToMe ? 0 : blockUnit[idx].Environment.SpaceWidth);
					}
				}
			}

			// Remove not movable blocks from space calculation
			for (int idx = firstIndex; idx <= lastIndex; idx++)
			{
				totalWidth += blockUnit[idx].Width + (blockUnit[idx].NextBlockBelongsToMe ? 0 : blockUnit[idx].Environment.SpaceWidth);
				if (idx == lastIndex)
				{
					totalWidth -= (blockUnit[idx].NextBlockBelongsToMe ? 0 : blockUnit[idx].Environment.SpaceWidth);
				}
				if (blockUnit[idx].NextBlockBelongsToMe)
				{
					count--;
				}
			}

			var dx = Math.Max(layoutWidth - totalWidth, 0);

			if (Alignment != XParagraphAlignment.Justify)
			{
				// right or center

				if (Alignment == XParagraphAlignment.Center)
				{
					dx /= 2;
				}

				for (int idx = firstIndex; idx <= lastIndex; idx++)
				{
					var block = blockUnit[idx];
					block.Location += new XSize(dx, 0);
				}
			}
			else if (count > 1) // case: justify
			{
				dx /= count - 1;
				var spaceCounter = 1;

				for (int idx = firstIndex + 1; idx <= lastIndex; idx++)
				{
					var block = blockUnit[idx];
					block.Location += new XSize(dx * spaceCounter, 0);
					if (!block.NextBlockBelongsToMe)
					{
						spaceCounter++;
					}
				}
			}
		}

		private void SetFormatterEnvironment(Block block, TextSegment textSegment)
		{
			block.Alignment = Alignment;
			block.Environment = new FormatterEnvironment
			{
				Font = textSegment.Font,
				Brush = textSegment.Brush,
				LineSpace = textSegment.LineSpace,
				CyAscent = textSegment.CyAscent,
				CyDescent = textSegment.CyDescent,
				SpaceWidth = textSegment.SpaceWidth
			};
		}

		private void SetFontSpacings(TextSegment segment)
		{
			if (segment.Font == null)
			{
				throw new ArgumentNullException("Font");
			}

			segment.LineSpace = segment.Font.GetHeight();
			segment.CyAscent = segment.LineSpace * segment.Font.CellAscent / segment.Font.CellSpace;
			segment.CyDescent = segment.LineSpace * segment.Font.CellDescent / segment.Font.CellSpace;

			// HACK in XTextSegmentFormatter
			segment.SpaceWidth = _gfx.MeasureString("x x", segment.Font).Width;
			segment.SpaceWidth -= _gfx.MeasureString("xx", segment.Font).Width;
		}

		// TODO:
		// - more XStringFormat variations
		// - calculate bounding box
		// - left and right indent
		// - first line indent
		// - margins and paddings
		// - background color
		// - text background color
		// - border style
		// - hyphens, soft hyphens, hyphenation
		// - kerning
		// - line spacing
		// - underline and strike-out variation
		// - super- and sub-script
		// - ...
	}
}