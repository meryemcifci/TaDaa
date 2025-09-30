let currentDate = new Date();
let selectedDate = null;
let calendarDataCache = {};

const monthNames = [
    'Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran',
    'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'
];

const dayNames = ['Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'];

async function loadCalendarData(year, month) {
    const key = `${year}-${month}`;

    if (calendarDataCache[key]) {
        return calendarDataCache[key];
    }

    try {
        const response = await fetch(`/PastTaDaa/GetCalendarData?year=${year}&month=${month}`);
        const data = await response.json();
        calendarDataCache[key] = data;
        return data;
    } catch (error) {
        console.error('Veri yüklenirken hata:', error);
        return [];
    }
}

async function renderCalendar() {
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();

    document.getElementById('monthYear').textContent =
        `${monthNames[month]} ${year}`;

    const calendarData = await loadCalendarData(year, month + 1);

    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);

    let firstDayOfWeek = firstDay.getDay();
    firstDayOfWeek = firstDayOfWeek === 0 ? 6 : firstDayOfWeek - 1;

    const calendar = document.getElementById('calendar');
    calendar.innerHTML = '';

    dayNames.forEach(day => {
        const header = document.createElement('div');
        header.className = 'day-header';
        header.textContent = day;
        calendar.appendChild(header);
    });

    for (let i = 0; i < firstDayOfWeek; i++) {
        const emptyCell = document.createElement('div');
        emptyCell.className = 'day-cell empty';
        calendar.appendChild(emptyCell);
    }

    const today = new Date();
    for (let day = 1; day <= lastDay.getDate(); day++) {
        const dayCell = document.createElement('div');
        dayCell.className = 'day-cell';
        dayCell.textContent = day;

        const dateStr = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;

        if (calendarData.some(d => d.date === dateStr)) {
            dayCell.classList.add('has-data');
        }

        if (year === today.getFullYear() &&
            month === today.getMonth() &&
            day === today.getDate()) {
            dayCell.classList.add('today');
        }

        if (selectedDate &&
            selectedDate.getFullYear() === year &&
            selectedDate.getMonth() === month &&
            selectedDate.getDate() === day) {
            dayCell.classList.add('selected');
        }

        dayCell.onclick = () => selectDay(year, month, day);
        calendar.appendChild(dayCell);
    }
}

async function selectDay(year, month, day) {
    selectedDate = new Date(year, month, day);
    const dateStr = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;

    renderCalendar();
    await renderDayDetail(dateStr, day, month, year);
}

async function renderDayDetail(dateStr, day, month, year) {
    const sidebar = document.getElementById('sidebar');
    sidebar.innerHTML = '<div class="loading">Yükleniyor...</div>';

    try {
        const response = await fetch(`/PastTaDaa/GetDayDetail?date=${dateStr}`);
        const data = await response.json();

        if (!data.hasData) {
            sidebar.innerHTML = `
                    <div class="sidebar-empty">
                        <div class="sidebar-empty-icon">📭</div>
                        <p><strong>${day} ${monthNames[month]} ${year}</strong></p>
                        <p style="margin-top: 12px;">Bu gün için henüz<br>TaDaa kaydı bulunmuyor.</p>
                    </div>
                `;
            return;
        }

        let tasksHtml = '';
        if (data.tasks && data.tasks.length > 0) {
            tasksHtml = '<div class="section-title">Görevler</div><ul class="task-list">';
            data.tasks.forEach(task => {
                const icon = task.isCompleted ?
                    '<span class="task-check">✓</span>' :
                    '<span class="task-bullet"></span>';

                const stars = '⭐'.repeat(task.rating);

                tasksHtml += `
                        <li class="task-item">
                            ${icon}
                            <span>${task.taskDescription}</span>
                            <span class="task-rating">${stars}</span>
                        </li>
                    `;

                if (task.moodNote) {
                    tasksHtml += `
                            <div class="mood-note-box">${task.moodNote}</div>
                        `;
                }
            });
            tasksHtml += '</ul>';
        }

        let ratingHtml = '';
        if (data.averageRating > 0) {
            ratingHtml = `
                    <div class="rating-box">
                        <span class="rating-label">Ortalama Puan:</span>
                        <span class="rating-value">${data.averageRating.toFixed(1)} / 5.0</span>
                    </div>
                `;
        }

        sidebar.innerHTML = `
                <div class="day-detail">
                    <div class="detail-header">
                        <div class="detail-title">${day} ${monthNames[month]} ${data.dayName}</div>
                        <div class="detail-emoji">${data.emoji}</div>
                    </div>
                    ${tasksHtml}
                    ${ratingHtml}
                </div>
            `;
    } catch (error) {
        console.error('Detay yüklenirken hata:', error);
        sidebar.innerHTML = `
                <div class="sidebar-empty">
                    <div class="sidebar-empty-icon">⚠️</div>
                    <p>Veri yüklenirken bir hata oluştu.</p>
                </div>
            `;
    }
}

function changeMonth(delta) {
    currentDate.setMonth(currentDate.getMonth() + delta);
    renderCalendar();
}

function goToToday() {
    currentDate = new Date();
    selectedDate = new Date();
    renderCalendar();

    const today = new Date();
    const dateStr = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, '0')}-${String(today.getDate()).padStart(2, '0')}`;
    renderDayDetail(dateStr, today.getDate(), today.getMonth(), today.getFullYear());
}

// Sayfa yüklendiğinde takvimi render et
document.addEventListener('DOMContentLoaded', function () {
    renderCalendar();
});