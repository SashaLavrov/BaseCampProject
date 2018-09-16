var photoId;
var recipient;
document.addEventListener('DOMContentLoaded', function () {
    $('#ModalSave').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Кнопка, что спровоцировало модальное окно  
        recipient = button.data('whatever') // Извлечение информации из данных-* атрибутов  
        photoId = button.data('id')
        modalImg.src = recipient;
    })
})

function AddPhotoInBoard(board) {
    let form = new FormData();
    form.append('photoId', photoId);
    form.append('boardId', board);

    fetch('/Home/AddPhotInBoard', {
        method: "POST",
        credentials: 'include',
        body: form,
    })
        .then(res => {
            if (res.status === 200) {
                $('#ModalSave').modal('hide');
            }
            else if (res.status === 400) {
                alert("Уже добавлено");
            }
            else {
                alert(res.status + ' - ' + res.statusText);
            }
        })
        .catch(alert);
}
cancel.addEventListener('click', function (e) {
    modalForm.style.display = "none";
    modalImg.style.display = "inline";
    boardName.value = "";
})

createBoard.addEventListener('click', function (e) {
    modalForm.style.display = "inline";
    modalImg.style.display = "none";
    boardName.value = "";
})

boardName.oninput = function () {
    if (boardName.length == 0 || boardName.value == "") {
        AddBoard.disabled = true;
    } else {
        AddBoard.disabled = false;
    };
};

AddBoard.addEventListener('click', function (e) {
    let form = new FormData();
    let boardname = boardName.value;
    
    form.append('boardName', boardname);
    fetch('/Home/CreateBoard', {
        method: "POST",
        credentials: 'include',
        body: form,
    })
        .then(res => {
            if (res.status === 200) {
                modalForm.style.display = "none";
                modalImg.style.display = "inline";
                boardName.value = "";
                return res.json();
            }
            else
                alert(res.status + ' - ' + res.statusText)
        })
        .then(responce => {
            addBordInList(boardname, responce);
        })
        .catch(alert);
    
});

function addBordInList(bn, boardId) {
    let listgroup = document.createElement('Div');
    listgroup.className = 'row list-group-item picItem';

    let cont = document.createElement('Div');
    cont.className = 'col-md-2';

    let img = document.createElement('img');
    img.className = 'img-rounded';
    img.src = recipient;
    img.style.height = '48px';

    let boardN = document.createElement('Div');
    boardN.className = 'col-md-6';
    boardN.style.textAlign = 'center';

    let span = document.createElement('span');
    span.innerText = bn;

    let saveButton = document.createElement('Div');
    saveButton.className = 'col-md-4';

    let button = document.createElement('button');
    button.className = 'btn btn-danger';
    button.innerHTML = "Сохранить";
    button.addEventListener('click', function (e) {
        AddPhotoInBoard(boardId);
    });

    AddPhotoInBoard(boardId);
    
    cont.appendChild(img);
    boardN.appendChild(span);
    saveButton.appendChild(button);

    listgroup.appendChild(cont);
    listgroup.appendChild(boardN);
    listgroup.appendChild(saveButton);

    listGroup.appendChild(listgroup);
}