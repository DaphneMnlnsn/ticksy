import { Clock, ClockAlert, ClockCheck, OctagonAlert, Ban, CircleX, TreePalm, CalendarX } from 'lucide-vue-next'
import { ref } from 'vue'

export const presentSummary = [
    { label: 'On time', value: 23, icon: Clock },
    { label: 'Late clock-in', value: 2, icon: ClockAlert },
    { label: 'Early clock-in', value: 4, icon: ClockCheck }
]

export const notPresentSummary = [
    { label: 'Absent', value: 5, icon: OctagonAlert },
    { label: 'No clock-in', value: 1, icon: Ban },
    { label: 'No clock-out', value: 13, icon: CircleX }
]

export const awaySummary = [
    { label: 'On Day Off', value: 3, icon: CalendarX },
    { label: 'On Leave', value: 2, icon: TreePalm },
]

export const reports = ref([
    {
        name: "Kiana Martin",
        clockIn: "7:58 am",
        clockOut: "5:05 pm",
        totalHours: getTotalHours("7:58 am", "5:05 pm"),
        overtime: "1h 7m",
        note: "Hi guys, welcome to my guys!"
    },
    {
        name: "Daphne Manalansan",
        clockIn: "7:00 am",
        clockOut: "5:05 pm",
        totalHours: getTotalHours("7:00 am", "5:05 pm"),
        overtime: "2h 5m",
        note: "ah..."
    },
    {
        name: "Lei Anysson Marquez",
        clockIn: "7:58 am",
        clockOut: "5:05 pm",
        totalHours: getTotalHours("7:58 am", "5:05 pm"),
        overtime: "1h 7m",
        note: "Ohayo Gozaimasu!"
    },
    {
        name: "Kirsten Quiana Domingo",
        clockIn: "7:58 am",
        clockOut: "5:05 pm",
        totalHours: getTotalHours("7:58 am", "5:05 pm"),
        overtime: "2h 7m",
        note: "Hello, World!"
    }
])

function getTotalHours(clockIn, clockOut) {
    const parseTime = (timeStr) => {
        const [time, modifier] = timeStr.split(' ')
        let [hours, minutes] = time.split(':').map(Number)

        if (modifier.toLowerCase() === 'pm' && hours !== 12) {
            hours += 12
        }
        if (modifier.toLowerCase() === 'am' && hours === 12) {
            hours = 0
        }

        return new Date(0, 0, 0, hours, minutes)
    }

    const start = parseTime(clockIn)
    const end = parseTime(clockOut)

    let diff = end - start

    // convert ms → minutes
    const totalMinutes = Math.floor(diff / 1000 / 60)

    const hours = Math.floor(totalMinutes / 60)
    const minutes = totalMinutes % 60

    return `${hours}h ${minutes}m`
}