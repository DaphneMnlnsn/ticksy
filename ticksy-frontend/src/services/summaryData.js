import { Clock, ClockAlert, ClockCheck, OctagonAlert, Ban, CircleX, TreePalm, CalendarX, ChevronsRight, Download } from 'lucide-vue-next'
import { ref } from 'vue'
import sampleIMG from '../assets/sample_img.jpg'

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

export const users = ref([
    {
        name: "Kiana Martin",
        avatar: sampleIMG,
        days: ["8hrs", "8hrs", "8hrs", "8hrs", "8hrs", "rest", "rest"],
        total: "40 hrs"
    },
    {
        name: "Daphne Manalansan",
        avatar: sampleIMG,
        days: ["8hrs", "8hrs", "8hrs", "8hrs", "8hrs", "rest", "rest"],
        total: "40 hrs"
    },
    {
        name: "Lei Anysson Marquez",
        avatar: sampleIMG,
        days: ["8hrs", "8hrs", "8hrs", "8hrs", "8hrs", "rest", "rest"],
        total: "40 hrs"
    },
    {
        name: "Quiana Domingo",
        avatar: sampleIMG,
        days: ["8hrs", "8hrs", "8hrs", "8hrs", "8hrs", "rest", "rest"],
        total: "40 hrs"
    }
])

export const allUsers = ref([
    {
        name: "Kiana",
        email: "kianamartinxiv@gmail.com",
        team: "-",
        lastActive: "9 minutes ago"
    },
    {
        name: "Daphne",
        email: "daphnemenalansan123@gmail.com",
        team: "-",
        lastActive: "19 minutes ago"
    },
    {
        name: "Lei Anysson",
        email: "anyssonleim.it@gmail.com",
        team: "-",
        lastActive: "10 minutes ago"
    },
    {
        name: "Kirsten Quiana",
        email: "quianadomingo004@gmail.com",
        team: "-",
        lastActive: "8 minutes ago"
    }
])

export const schedules = ref([
    { id: 1, name: "Internship", label: "Default", icon: ChevronsRight },
    { id: 2, name: "Hybrid Schedule", icon: ChevronsRight },
    { id: 3, name: "Part-timers", icon: ChevronsRight }
])

export const calendars = ref([
    { id: 1, name: "Philippines", label: "Imported Calendar", icon: Download },
    { id: 2, name: "United States - Texas", label: "Imported Calendar", icon: Download },
])

export const holidays = ref([
    { name: "New Year's Day", date: "Jan 01, 2026", day: "Thursday" },
    { name: "Martin Luther King Jr. Day", date: "Jan 19, 2026", day: "Monday" },
    { name: "Washington's Birthday (Presidents' Day)", date: "Feb 16, 2026", day: "Monday" },
    { name: "Memorial Day", date: "May 25, 2026", day: "Monday" },
    { name: "Juneteenth National Independence Day", date: "Jun 19, 2026", day: "Friday" },
    { name: "Independence Day", date: "Jul 04, 2026", day: "Saturday" },
    { name: "Labor Day", date: "Sep 07, 2026", day: "Monday" },
    { name: "Columbus Day", date: "Oct 12, 2026", day: "Monday" },
    { name: "Veterans Day", date: "Nov 11, 2026", day: "Wednesday" },
    { name: "Thanksgiving Day", date: "Nov 26, 2026", day: "Thursday" },
    { name: "Christmas Day", date: "Dec 25, 2026", day: "Friday" },
]);

export const timeOffRequests = ref([
    {
        name: "Kiana",
        type: "Sick Leave",
        reason: "Dental Appointment",
        r_date: "March 9, 2026",
        status: "Approved"
    },
    {
        name: "Daphne",
        type: "Vacation Leave",
        reason: "Family Vacation",
        r_date: "March 15 - March 25, 2026",
        status: "Pending"
    },
    {
        name: "Lei",
        type: "Emergency Leave",
        reason: "Urgent Family Matters",
        r_date: "March 5, 2026",
        status: "Rejected"
    },
    {
        name: "Quiana",
        type: "Unpaid Leave",
        reason: "Taking Additional Rest",
        r_date: "March 10 - 12, 2026",
        status: "Approved"
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

    const totalMinutes = Math.floor(diff / 1000 / 60)

    const hours = Math.floor(totalMinutes / 60)
    const minutes = totalMinutes % 60

    return `${hours}h ${minutes}m`
}