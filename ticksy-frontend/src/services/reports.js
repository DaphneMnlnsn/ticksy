import api from "./api";
import dayjs from "dayjs";

const formatDate = (date) => dayjs(date).format("YYYY-MM-DD");

export async function getAttendanceReport(start, end) {
    if (!start || !end) {
        throw new Error("Missing start or end date");
    }

    const params = {
        start: formatDate(start),
        end: formatDate(end)
    };

    return await api.get("/reports/attendance-report", { params });
}

export async function getAttendanceSummary(start, end) {
    if (!start || !end) {
        throw new Error("Missing start or end date");
    }

    const params = {
        start: formatDate(start),
        end: formatDate(end)
    };

    return await api.get("/reports/summary", { params });
}