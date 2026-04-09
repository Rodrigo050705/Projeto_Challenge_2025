using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public static class DocumentationPdf
{
    public static byte[] Render(string title, string language, string function, string content)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text(title ?? "(Sem título)").SemiBold().FontSize(16);
                    col.Item().Text($"{language ?? "—"} · {function ?? "—"}").FontSize(10).FontColor(Colors.Grey.Darken2);
                });

                page.Content().Element(e =>
                {
                    e.PaddingVertical(10)
                     .Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(10)
                     .Text(content ?? string.Empty).FontFamily("Courier New").FontSize(10).LineHeight(1.25f);
                });

                page.Footer().AlignRight().Text(txt =>
                {
                    txt.Span("Gerado em ").FontColor(Colors.Grey.Darken2).FontSize(9);
                    txt.Span($"{System.DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });
        }).GeneratePdf();
    }
}
