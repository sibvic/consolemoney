using Alba.CsConsoleFormat;

namespace Sibvic.ConsoleMoney
{
    internal static class ConsoleDocumentRenderer
    {
        public static void Render(Document document)
        {
            IRenderTarget target = Console.IsOutputRedirected
                ? new TextRenderTarget(Console.Out)
                : new ConsoleRenderTarget();
            ConsoleRenderer.RenderDocument(document, target, GetRenderRect());
        }

        private static Rect GetRenderRect()
        {
            try
            {
                return new Rect(new Size(Console.BufferWidth, Size.Infinity));
            }
            catch (IOException)
            {
                return new Rect(new Size(80, Size.Infinity));
            }
        }
    }
}
