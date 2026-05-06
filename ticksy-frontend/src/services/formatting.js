export const formatTime = (timeString) => {
    if (!timeString) return '--:--';
    
    const parts = timeString.split(':');
    const hoursStr = parts[0] || '0';
    const minutesStr = parts[1] || '00';

    let hours = parseInt(hoursStr, 10);
    const ampm = hours >= 12 ? 'PM' : 'AM';
    
    hours = hours % 12;
    hours = hours ? hours : 12;

    return `${hours}:${minutesStr} ${ampm}`;
};

export const formatDuration = (timeString) => {
    if (!timeString) return '--:--';

    const parts = timeString.split(':');
    const hoursStr = parts[0] || '0';
    const minutesStr = parts[1] || '00';

    return `${parseInt(hoursStr, 10)}.${minutesStr}`;
};

export const formatFullDateTime = (isoString) => {
    if (!isoString) return '--';
    
    const date = new Date(isoString);
    if (isNaN(date.getTime())) return '--';

    return date.toLocaleString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
        hour: 'numeric',
        minute: '2-digit',
        hour12: true
    });
};

export const formatDate = (isoString) => {
    if (!isoString) return '--';
    
    const date = new Date(isoString);
    if (isNaN(date.getTime())) return '--';

    return date.toLocaleString('en-US', {
        month: 'long',
        day: 'numeric'
    });
};

export const formatRange = (range) => {
    if (!range || !range.includes(" - ")) return "";

    const parts = range.split(" - ");
    const startStr = parts[0] || "";
    const endStr = parts[1] || "";

    const options = { 
        month: 'long', 
        day: 'numeric', 
        year: 'numeric' 
    };

    const formatDate = (dateStr) => {
        if (!dateStr) return "";
        const date = new Date(dateStr);
        return isNaN(date.getTime()) 
            ? "" 
            : new Intl.DateTimeFormat('en-US', options).format(date);
    };

    const startFormatted = formatDate(startStr);
    const endFormatted = formatDate(endStr);

    if (!startFormatted || !endFormatted) return "";

    if (startFormatted === endFormatted) {
        return startFormatted;
    }

    return `${startFormatted} - ${endFormatted}`;
};