
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

    // Emoji seçimi
    $('.emoji-btn').click(function() {
        const emoji = $(this).data('emoji');
        $('.emoji-btn').removeClass('selected');
        $(this).addClass('selected');
        
        $.post('@Url.Action("SetDailyEmoji")', {
            emoji: emoji
        }, function(response) {
            if (response.success) {
                $('.selected-emoji').html(`<h5>Bugün seçilen emoji: <span style="font-size: 2rem;">${emoji}</span></h5>`);
                // Success mesajı göster
                showSuccessMessage('Günün emojisi kaydedildi! 🎉');
            } else {
                alert('Emoji kaydedilemedi: ' + response.message);
            }
        }).fail(function() {
            alert('Bir hata oluştu. Lütfen tekrar deneyin.');
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

    // Sayfa yüklendiğinde bugünün emojisini işaretle
    const todayEmoji = '@ViewBag.TodayEmoji';
    if (todayEmoji) {
        $(`.emoji-btn[data-emoji="${todayEmoji}"]`).addClass('selected');
    }
});

