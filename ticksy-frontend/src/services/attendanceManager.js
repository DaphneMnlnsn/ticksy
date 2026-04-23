import {
    getWeeklyAttendance,
    getDailyAttendance,
    getMonthlyAttendance,
    getUserAttendance
} from "../services/attendanceService"

export async function fetchAttendance(viewType, params, role, userId) {
    console.log("VIEW TYPE:", viewType)
    const isAdmin = role?.toLowerCase() === "admin"

    let res

    if (isAdmin) {
        if (viewType === "weekly")
            res = await getWeeklyAttendance(params.start, params.end)

        else if (viewType === "daily")
            res = await getDailyAttendance(params.date)

        else
            res = await getMonthlyAttendance(params.date)

        return {
            data: res.data || []
        }
    }

    if (viewType === "weekly")
        res = await getWeeklyAttendance(params.start, params.end, userId)

    else if (viewType === "daily")
        res = await getDailyAttendance(params.date, userId)

    else
        res = await getMonthlyAttendance(params.date, userId)

    return {
        data: Array.isArray(res.data) ? res.data : []
    }
}