import api from "./api";
import dayjs from "dayjs";

export async function getTodayStatus() {
    return await api.get("/attendance/today-status");
}

export async function clockIn(data) {
    return await api.post("/attendance/clock-in", {
        notes: data.note,
        customTime: data.time,
        customDate: data.date
    });
}

export async function clockOut(data) {
    return await api.patch("/attendance/clock-out", {
        notes: data.note
    });
}

export async function startBreak() {
    return await api.patch("/attendance/start-break");
}

export async function endBreakApi() {
    return await api.patch("/attendance/end-break");
}

export async function getWeeklyAttendance(start, end, userId) {
    if (!start || !end) {
        throw new Error("Missing start or end date");
    }

    const params = {
        start: dayjs(start).format("YYYY-MM-DD"),
        end: dayjs(end).format("YYYY-MM-DD"),
        ...(userId && { userId })
    };

    return api.get("/attendance/weekly", { params });
}

export async function getDailyAttendance(date, userId) {
    if (!date) {
        throw new Error("Missing date");
    }

    const params = {
        date: dayjs(date).format("YYYY-MM-DD"),
        ...(userId && { userId })
    };

    return api.get("/attendance/daily", { params });
}

export async function getMonthlyAttendance(date, userId) {
    if (!date) throw new Error("Missing date");

    return api.get("/attendance/monthly", {
        params: {
            date: dayjs(date).format("YYYY-MM-DD"),
            ...(userId && { userId })
        }
    })
}

export async function getUserAttendance(userId) {
    if (!userId) throw new Error("Missing userId")

    return await api.get(`/attendance/${userId}`)
}