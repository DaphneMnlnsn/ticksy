export function formatCSV(payload) {
    const { meta, table } = payload

    const metaRows = [
        [meta.title],
        [meta.subtitle],
        [`Date Range: ${meta.dateRange}`],
        [`Generated: ${meta.generatedAt}`],
        []
    ]

    return {
        metaRows,
        headers: table.headers,
        rows: table.rows
    }
}