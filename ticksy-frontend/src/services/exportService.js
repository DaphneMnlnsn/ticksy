import jsPDF from 'jspdf'
import autoTable from 'jspdf-autotable'
import logo from '../assets/IDA-Logo.png'

function getImageBase64(url) {
    return new Promise((resolve, reject) => {
        const img = new Image()
        img.setAttribute('crossOrigin', 'anonymous')

        img.onload = () => {
            const canvas = document.createElement('canvas')
            canvas.width = img.width
            canvas.height = img.height

            const ctx = canvas.getContext('2d')
            ctx.drawImage(img, 0, 0)

            resolve(canvas.toDataURL('image/png'))
        }

        img.onerror = reject
        img.src = url
    })
}

export function useExportService() {

    function exportCSV(filename, { metaRows = [], headers = [], rows = [] }) {
        let csvContent = ''

        metaRows.forEach(row => {
            if (Array.isArray(row)) {
                csvContent += row.join(',') + '\n'
            } else if (typeof row === 'object') {
                csvContent += Object.values(row).join(',') + '\n'
            }
        })

        csvContent += headers.join(',') + '\n'

        rows.forEach(row => {
            if (Array.isArray(row)) {
                csvContent += row.join(',') + '\n'
            } else if (typeof row === 'object') {
                csvContent += Object.values(row).join(',') + '\n'
            }
        })

        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
        const link = document.createElement('a')

        link.href = URL.createObjectURL(blob)
        link.setAttribute('download', `${filename}.csv`)
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
    }

    async function exportPDF(filename, payload, options = {}) {
        const doc = new jsPDF({
            orientation: options.landscape ? 'landscape' : 'portrait',
            unit: 'mm',
            format: 'a4'
        })

        const pageWidth = doc.internal.pageSize.getWidth()
        let y = 12

        // ================= HEADER =================
        if (payload.meta) {
            const logoBase64 = await getImageBase64(logo).catch(() => null)

            let logoWidth = 18
            let logoHeight = 18
            let logoY = 10
            let textX = 14

            if (logoBase64) {
                const img = new Image()
                img.src = logoBase64

                await new Promise(resolve => {
                    img.onload = resolve
                })

                logoWidth = 18
                logoHeight = (img.height / img.width) * logoWidth

                doc.addImage(logoBase64, 'PNG', 12, logoY, logoWidth, logoHeight)
                textX = 12 + logoWidth + 8
            }

            let textY = logoY + 4

            doc.setFont('helvetica', 'bold')
            doc.setFontSize(14)

            const titleLines = doc.splitTextToSize(
                payload.meta.title || 'Report',
                pageWidth - textX - 14
            )

            doc.text(titleLines, textX, textY)
            textY += titleLines.length * 6

            doc.setFont('helvetica', 'normal')
            doc.setFontSize(9)
            doc.setTextColor(120, 120, 120)

            doc.text([
                `Date Range: ${payload.meta.dateRange || '-'}`,
                `Generated: ${payload.meta.generatedAt || new Date().toLocaleString()}`
            ], textX, textY)

            y += 25

            doc.setDrawColor(210)
            doc.line(12, y, pageWidth - 12, y)
            y += 6
        }

        // ================= SUMMARY =================
        if (payload.summary) {
            doc.setFontSize(11)
            doc.setTextColor(0)
            doc.text('Analytics Summary', 14, y)
            y += 6

            const summaryRows = []

            payload.summary.forEach(group => {
                if (!group.items) return

                group.items.forEach(i => {
                    summaryRows.push([
                        `${group.title} - ${i.label}`,
                        i.value ?? '--'
                    ])
                })
            })

            summaryRows.push([
                "Total Leaves",
                getTotalLeaves(payload.table?.rows || [])
            ])

            autoTable(doc, {
                startY: y,
                head: [['Category', 'Value']],
                body: summaryRows,
                styles: { fontSize: 9, cellPadding: 2 },
                headStyles: {
                    fillColor: [8, 58, 115],
                    textColor: 255
                }
            })

            y = doc.lastAutoTable.finalY + 8
        }

        // ================= TABLE =================
        const headers = payload.table?.headers || []
        const rowsSource = payload.table?.rows || []

        if (!rowsSource.length) {
            console.error("No rows found")
            return
        }

        const isMonthly =
            headers.includes('Days Present') ||
            headers.includes('Late Count')

        const formatCell = (value) => {
            if (value === null || value === undefined || value === '') return '--'

            if (typeof value === 'string') {
                return value
            }

            return value
        }

        let finalHeaders = [...headers]

        const keyMap = {
            "Name": "name",
            "First In": "firstIn",
            "Last Out": "lastOut",
            "Total": "total",
            "Total Hours": "total"
        }

        let finalRows = rowsSource.map(row =>
            headers.map(h => {
                const value = getValue(row, h, keyMap)

                let formattedValue
                if (h.toLowerCase().includes('name') || h.toLowerCase().includes('month')) {
                    formattedValue = value ?? '--'
                } else if (isTimeField(h)) {
                    formattedValue = formatHoursMinutes(value)
                } else {
                    formattedValue = formatCell(value)
                }

                return formattedValue
            })
        )

        if (isMonthly) {
            finalHeaders.push('Total Leaves')

            finalRows = rowsSource.map(row => {
                const base = headers.map(h => {
                    const value = getValue(row, h, keyMap)

                    let formattedValue
                    if (h.toLowerCase().includes('name') || h.toLowerCase().includes('month')) {
                        formattedValue = value ?? '--'
                    } else if (isTimeField(h)) {
                        formattedValue = formatHoursMinutes(value)
                    } else {
                        formattedValue = formatCell(value)
                    }

                    return formattedValue
                })

                const leaves = Array.isArray(row?.days)
                    ? row.days.filter(d => d === "LEAVE").length
                    : 0

                base.push(leaves)

                return base
            })
        }

        autoTable(doc, {
            head: [finalHeaders],
            body: finalRows,
            startY: y,

            styles: {
                font: 'helvetica',
                fontSize: 9,
                cellPadding: 3,
                halign: 'center'
            },

            headStyles: {
                fillColor: [5, 35, 72],
                textColor: 255,
                fontStyle: 'bold'
            },

            alternateRowStyles: {
                fillColor: [245, 247, 250]
            },

            margin: { left: 12, right: 12 },
            tableWidth: 'auto'
        })

        doc.save(`${filename}.pdf`)
    }

    return {
        exportCSV,
        exportPDF
    }
}

function getTotalLeaves(rows = []) {
    let total = 0

    rows.forEach(row => {
        const days = row?.days ?? []
        total += days.filter(d => d === "LEAVE").length
    })

    return total
}

function formatHoursMinutes(value) {
    if (value === null || value === undefined || value === '') return '--'

    if (typeof value === 'string') {
        if (value.includes('h') || value.includes('m')) {
            return value
        }

        if (value === '--:--') return '--'
    }

    const num = Number(value)

    if (isNaN(num)) return value ?? '--'

    const totalMinutes = Math.round(num * 60)

    const hours = Math.floor(totalMinutes / 60)
    const minutes = totalMinutes % 60

    if (hours === 0 && minutes === 0) return '0'
    if (hours === 0) return `${minutes}m`
    if (minutes === 0) return `${hours}h`

    return `${hours}h ${minutes}m`
}

const isTimeField = (header) => {
    const h = header.toLowerCase()
    return (
        h.includes('total') ||
        h.includes('hours') ||
        h.includes('time') ||
        h.includes('in') ||
        h.includes('out')
    )
}

function getValue(row, h, keyMap) {
    const normalize = str => str.toLowerCase().replace(/\s+/g, '')

    let value

    if (/^Day\s*\d+/i.test(h)) {
        const index = parseInt(h.replace(/Day\s*/i, '')) - 1
        return row.days?.[index]
    }

    if (normalize(h) === 'dayspresent') {
        return Array.isArray(row.days)
            ? row.days.filter(d => d !== '0h' && d !== '0' && d !== 'REST' && d !== 'LEAVE').length
            : 0
    }

    if (normalize(h) === 'daysabsent') {
        return Array.isArray(row.days)
            ? row.days.filter(d => d === '0h' || d === '0').length
            : 0
    }

    const mappedKey = Object.keys(keyMap).find(k =>
        normalize(k) === normalize(h)
    )

    if (mappedKey && row?.[keyMap[mappedKey]] !== undefined) {
        return row[keyMap[mappedKey]]
    }

    const found = Object.keys(row).find(key =>
        normalize(key) === normalize(h)
    )

    return found ? row[found] : undefined
}