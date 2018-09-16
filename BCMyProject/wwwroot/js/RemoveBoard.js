var boardId;
document.addEventListener('DOMContentLoaded', function () {
    $('#ModalBoardRemove').on('show.bs.modal', function (event) {
        var elem = $(event.relatedTarget) // Кнопка, что спровоцировало модальное окно  
        boardId = elem.data('id') // Извлечение информации из data-* атрибутов  
    })
})

RemoveBoard.addEventListener('click', function () {
    let form = new FormData();
    form.append('boardId', boardId);

    fetch('/Home/RemoveBoard', {
        method: "POST",
        credentials: 'include',
        body: form,
    })
        .then(res => {
            if (res.status === 200) {
                document.getElementById(boardId).remove();
            }
            else
            {
                alert(res.status + ' - ' + res.statusText);
            }
        })
        .catch(alert);
})
