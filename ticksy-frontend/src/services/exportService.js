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
            csvContent += row.join(',') + '\n'
        })

        csvContent += headers.join(',') + '\n'

        rows.forEach(row => {
            const values = headers.map(h => row[h] ?? '')
            csvContent += values.join(',') + '\n'
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
        const pageHeight = doc.internal.pageSize.getHeight()

        let y = 12

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

                const maxWidth = 18
                const maxHeight = 18

                logoWidth = maxWidth
                logoHeight = (img.height / img.width) * logoWidth

                if (logoHeight > maxHeight) {
                    logoHeight = maxHeight
                    logoWidth = (img.width / img.height) * logoHeight
                }

                logoY = 10

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

            const metaText = [
                `Date Range: ${payload.meta.dateRange || '-'}`,
                `Generated: ${payload.meta.generatedAt || new Date().toLocaleString()}`
            ]

            doc.text(metaText, textX, textY)

            const titleHeight = titleLines.length * 6
            const subtitleHeight = payload.meta.subtitle ? 5 : 0
            const metaHeight = 8

            const textBlockHeight =
                titleHeight + subtitleHeight + metaHeight

            const logoBlockHeight = logoHeight

            const headerHeight = Math.max(logoBlockHeight, textBlockHeight)

            const headerBottom = logoY + headerHeight

            y = headerBottom + 10

            doc.setDrawColor(210)
            doc.line(12, y, pageWidth - 12, y)

            y += 6
        }

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
                        i.value
                    ])
                })
            })

            autoTable(doc, {
                startY: y,
                head: [['Category', 'Value']],
                body: summaryRows,
                styles: {
                    fontSize: 9,
                    cellPadding: 2
                },
                headStyles: {
                    fillColor: [8, 58, 115],
                    textColor: 255
                }
            })

            y = doc.lastAutoTable.finalY + 8
        }

        autoTable(doc, {
            head: [payload.table.headers],
            body: payload.table.rows,

            startY: y,

            styles: {
                font: 'helvetica',
                fontSize: 9,
                cellPadding: 3,
                valign: 'middle',
                halign: 'center',
                overflow: 'linebreak'
            },

            headStyles: {
                fillColor: [5, 35, 72],
                textColor: 255,
                fontStyle: 'bold',
                halign: 'center'
            },

            bodyStyles: {
                halign: 'center' 
            },

            alternateRowStyles: {
                fillColor: [245, 247, 250]
            },

            margin: { left: 12, right: 12 },

            tableWidth: 'auto',

            columnStyles: {
                0: { cellWidth: 40 } 
            }
        })

        doc.save(`${filename}.pdf`)
    }

    return {
        exportCSV,
        exportPDF
    }
}

function formatFullDate(date) {
    if (!date) return '--'

    const d = new Date(date)
    if (isNaN(d.getTime())) return '--'

    return d.toLocaleDateString('en-US', {
        month: 'long',
        day: 'numeric',
        year: 'numeric'
    })
}

