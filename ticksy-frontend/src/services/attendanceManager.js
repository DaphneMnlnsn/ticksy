import {
    getWeeklyAttendance,
    getDailyAttendance,
    getMonthlyAttendance,
    getUserAttendance
} from "../services/attendanceService"

export async function fetchAttendance(viewType, params, role, userId) {
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

    res = await getUserAttendance(userId)

    return {
        data: Array.isArray(res.data) ? res.data : [res.data]
    }
}