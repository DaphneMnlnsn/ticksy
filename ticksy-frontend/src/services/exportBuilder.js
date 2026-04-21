export function buildExportPayload({
    title,
    subtitle = '',
    dateRange = '',
    generatedAt = new Date(),
    headers = [],
    rows = [],
}) {
    return {
        meta: {
            title,
            subtitle,
            dateRange,
            generatedAt: new Date(generatedAt).toLocaleString()
        },
        table: {
            headers,
            rows
        }
    }
}