boardName.oninput = function () {
    if (boardName.length == 0 || boardName.value =="") {
        AddBoard.disabled = true;
    } else {
        AddBoard.disabled = false;
    };
};

AddBoard.addEventListener('click', function (e) {
    let form = new FormData();
    form.append('boardName', boardName.value);

    fetch('/Home/CreateBoard', {
        method: "POST",
        credentials: 'include',
        body: form,
    })
        .then(res => {
            if (res.status === 200)
            {
                return res.json();
            }
            else
                alert(res.status + ' - ' + res.statusText)
        })
        .then(responce => {
            create(boardName, responce);
            boardName.value = '';
        })
        .catch(alert);
});

function create(boardName, boardId) {
    let penal = document.createElement('div');
    penal.className = 'masonry-layout__panel';
    penal.id = boardId;

    let penal1 = document.createElement('div');
    penal1.className = 'masonry-layout__panel-content';

    let penal2 = document.createElement('div');
    penal2.className = 'PhotoInfo';

    let photo = document.createElement('img');
    photo.className = 'ImgInfo';
    photo.src = '../images/noimage.png';


    let h3 = document.createElement('h3');
    h3.innerHTML = boardName.value;

    let mask = document.createElement('div');
    mask.className = 'mask';

    let span = document.createElement('span');
    span.className = 'glyphicon glyphicon-remove';
    span.id = 'remove';
    span.dataset.toggle = 'modal';
    span.dataset.target = '#ModalBoardRemove'
    span.dataset.id = boardId;

    penal2.appendChild(photo);
    penal2.appendChild(h3);
    penal2.appendChild(mask);
    penal2.appendChild(span);
    penal1.appendChild(penal2);
    penal.appendChild(penal1);
    listBoard.appendChild(penal);
}