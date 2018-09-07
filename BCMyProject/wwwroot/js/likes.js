document.getElementById("Text").addEventListener('focus', function (e) {
    e.preventDefault();
    this.rows = 5;
});

document.getElementById("Text").addEventListener('blur', function (e) {
    e.preventDefault();
    this.rows = 2;
});

if (IsLike.value == 'True') {
    likeButton.style.color = '#F0120E'; likeButton.style.border = 'solid 2px #F0120E';
} else if (IsLike.Value == 'False') {
    likeButton.style.color = '#B8B8B8'; likeButton.style.border = 'solid 2px #5F5F5F';
}


let xhr1 = new XMLHttpRequest();
likeButton.addEventListener('click', function (e) {
    e.preventDefault();

    xhr1.open('POST', '/Home/Like', true);
    xhr1.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');

    let body1 = `PhotoId=${encodeURIComponent(PhotoId.value)}`;
    xhr1.send(body1);
});

xhr1.onload = function () {
    if (xhr1.responseText == 'true') {
        addLike();
    } else if (xhr1.responseText == 'false') {
        removeLike()
    }
};

xhr1.onerror = function () {
    alert(xhr1.status + ' - ' + xhr1.statusText);
};

function addLike() {
    idd.innerHTML = ++iddd.value;
    likeButton.style.color = '#F0120E'; likeButton.style.border = 'solid 2px #F0120E';
}

function removeLike() {
    idd.innerHTML = --iddd.value;
    likeButton.style.color = '#B8B8B8'; likeButton.style.border = 'solid 2px #5F5F5F';
}
