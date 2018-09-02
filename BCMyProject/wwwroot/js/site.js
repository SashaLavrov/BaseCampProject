// Write your JavaScript code.
let buttonEl = document.getElementById("SendButton");
let textEl = document.getElementById("Text");
let photoIdEl = document.getElementById("PhotoId");
let xhr = new XMLHttpRequest();

buttonEl.addEventListener('click', function (e) {
    e.preventDefault();
    //
    xhr.open('POST', '/Home/AddComent', true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');

    let body = `Text=${encodeURIComponent(textEl.value)}&PhotoId=${encodeURIComponent(photoIdEl.value)}`;
    xhr.send(body);
});
//
xhr.onload = function () {
    addMessage(xhr.responseText);
};

xhr.onerror = function () {
    alert(xhr.status + ' - ' + xhr.statusText);
};

function addMessage() {
    let messageEl = document.createElement('p');
    messageEl.innerHTML = `${UserName.value} : ${textEl.value}`;

    let parentEl = document.getElementById("coment");
    parentEl.appendChild(messageEl);

    textEl.value = '';
}
