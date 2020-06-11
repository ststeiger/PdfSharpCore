namespace PdfSharpCore.Pdf
{
    /// <summary>
    /// Defines how annotations should be copied.
    /// </summary>
    public enum AnnotationCopyingType
    {
        /// <summary>
        /// Skips annotation copying.
        /// </summary>
        DoNotCopy,
        /// <summary>
        /// Does a shallow copy.
        /// </summary>
        ShallowCopy,
        /// <summary>
        /// Performs deep copy.
        /// </summary>
        DeepCopy
    }
}
