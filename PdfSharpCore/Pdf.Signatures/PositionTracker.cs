namespace PdfSharpCore.Pdf.Signatures
{
    internal class PositionTracker
    {
        public PdfItem Item { get; private set; }
        public long Start { get; private set; }
        public long End { get; private set; }

        public PositionTracker(PdfItem item)
        {
            Item = item;
            Item.BeforeWrite += (s, e) =>
                this.Start = e.Position;
            Item.AfterWrite += (s, e) => 
                this.End = e.Position;
        }
    }
}
