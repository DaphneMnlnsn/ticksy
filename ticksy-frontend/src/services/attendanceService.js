import api from "./api";
import dayjs from "dayjs";

export async function getWeeklyAttendance(start, end) {
    if (!start || !end) {
        throw new Error("Missing start or end date");
    }

    const params = {
        start: dayjs(start).format("YYYY-MM-DD"),
        end: dayjs(end).format("YYYY-MM-DD")
    };

    return api.get("/attendance/weekly", { params });
}

export async function getDailyAttendance(date) {
    if (!date) {
        throw new Error("Missing date");
    }

    const params = {
        date: dayjs(date).format("YYYY-MM-DD")
    };

    return api.get("/attendance/daily", { params });
}

export async function getMonthlyAttendance(date) {
    if (!date) throw new Error("Missing date");

    return api.get("/attendance/monthly", {
        params: {
            date: dayjs(date).format("YYYY-MM-DD")
        }
    })
}