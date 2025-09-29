
$(document).ready(function() {
    // Mood Quick Select
    $('.mood-quick').click(function() {
        $('.mood-quick').removeClass('selected');
        $(this).addClass('selected');
        $('#moodNoteInput').val($(this).data('mood'));
    });

    // Custom mood note yazılırken quick select'i temizle
    $('#moodNoteInput').on('input', function() {
        if ($(this).val() !== $('.mood-quick.selected').data('mood')) {
            $('.mood-quick').removeClass('selected');
        }
    });

    let selectedEmoji = null;

    // Emoji seçildiğinde highlight et
    document.querySelectorAll(".emoji-btn").forEach(btn => {
        btn.addEventListener("click", function () {
            selectedEmoji = this.dataset.emoji;

            document.querySelectorAll(".emoji-btn").forEach(b => b.classList.remove("active"));
            this.classList.add("active");
        });
    });

    // Kaydet butonuna basıldığında
    document.getElementById("saveEmojiBtn").addEventListener("click", function () {
        if (!selectedEmoji) {
            alert("Lütfen bir emoji seçin!");
            return;
        }

        const formData = new FormData();
        formData.append("emoji", selectedEmoji);

        fetch('/Task/SetDailyEmoji', {
            method: "POST",
            body: formData
        })


        .then(res => res.json())
        .then(data => {
            if (data.success) {
                alert(data.message);
                location.reload();
            } else {
                alert(data.message || "Bir hata oluştu.");
            }
        })
        .catch(err => {
            console.error("Hata:", err);
            alert("Sunucuya bağlanırken hata oluştu.");
        });
    });
    
    $(document).on('click', '.delete-btn', function () {
        const button = $(this);                
        const taskId = button.data('task-id'); 

        if (confirm('Bu başarını silmek istediğinizden emin misiniz?')) {
            $.ajax({
                url: '/Task/DeleteTask', 
                type: 'POST',
                data: {
                    taskId: taskId,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.success) {
                        showSuccessMessage('Başarılı silindi!');

                        button.closest('.task-row').fadeOut(500, function () {
                            $(this).remove();
                        });
                    } else {
                        alert('Silme işlemi başarısız: ' + response.message);
                    }
                },
                error: function () {
                    alert('Bir hata oluştu. Lütfen tekrar deneyin.');
                }
            });
        }
    });


    // Form submit validation
    $('#taskForm').submit(function(e) {
        const taskDescription = $('#taskInput').val().trim();
        const rating = $('input[name="Rating"]:checked').val();
        
        if (!taskDescription) {
            e.preventDefault();
            alert('Lütfen bir başarı açıklaması girin.');
            return false;
        }
        
        if (!rating) {
            e.preventDefault();
            alert('Lütfen bir puan seçin (1-5 yıldız).');
            return false;
        }
    });

    // Success mesajı gösterme fonksiyonu
    function showSuccessMessage(message) {
        // Eğer önceki mesaj varsa kaldır
        $('.success-message').remove();
        
        const successDiv = $(`
            <div class="success-message alert alert-success alert-dismissible fade show position-fixed" 
                 style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
                <i class="fas fa-check-circle me-2"></i>${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `);
        
        $('body').append(successDiv);
        
        // 3 saniye sonra otomatik kaldır
        setTimeout(function() {
            successDiv.fadeOut(function() {
                $(this).remove();
            });
        }, 3000);
    }

});

