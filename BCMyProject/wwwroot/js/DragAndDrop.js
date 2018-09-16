let dropArea = document.getElementById('drop-area');
['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, preventDefaults, false)
})
function preventDefaults(e) {
    e.preventDefault()
    e.stopPropagation()
};
['dragenter', 'dragover'].forEach(eventName => {
    dropArea.addEventListener(eventName, highlight, false)
})
    ;['dragleave', 'drop'].forEach(eventName => {
        dropArea.addEventListener(eventName, unhighlight, false)
    })
function highlight(e) {
    dropArea.classList.add('highlight')
}
function unhighlight(e) {
    dropArea.classList.remove('highlight')
}

dropArea.addEventListener('drop', handleDrop, false)
var variable = [];
function handleDrop(e) {// get files
    let dt = e.dataTransfer
    let files = dt.files
    variable.concat(files)
    handleFiles(files)
}

AddFile.addEventListener('click', function (e) {
    files = [...variable]
    files.forEach(Upload);
});

function handleFiles(files) {
    files = [...files]
    files.forEach(previewFile)
    files.forEach(e => variable.push(e))
}

function handleFiles_1(files) {
    files = [...files]
    files.forEach(Upload)
}

function previewFile(file) {
    let reader = new FileReader()
    reader.readAsDataURL(file)
    reader.onloadend = function () {
        let img = document.createElement('img')
        img.src = reader.result
        document.getElementById('gallery').appendChild(img)
    }
}

function Upload(file) {
    let Topic = document.getElementById('topic').value;
    let url = '/Home/Upload'
    let formData = new FormData();
    formData.append('file', file);
    formData.append('topic', Topic);
    fetch(url, {
        method: 'POST',
        body: formData
    })
        .then(res => {
            if (res.status === 200)
                addImg(file);
            else
                alert(res.status + ' - ' + res.statusText)
        })
        .catch(alert);
}

function addImg(file) {
    let reader = new FileReader()
    reader.readAsDataURL(file)
    reader.onloadend = function () {
        let penalEl = document.createElement('div');
        penalEl.className = 'masonry-layout__panel';

        let penalContentEl = document.createElement('div');
        penalContentEl.className = 'masonry-layout__panel-content';

        let photoInfo = document.createElement('div');
        photoInfo.className = 'PhotoInfo';

        let img = document.createElement('img');
        img.className = 'ImgInfo';
        img.src = reader.result;

        let mask = document.createElement('div');
        mask.className = 'mask';

        let remove = document.createElement('span');
        remove.className = 'glyphicon glyphicon-remove';
        remove.id = 'remove';
        remove.title = 'Удалить';

        let download = document.createElement('span');
        download.className = 'glyphicon glyphicon-download';
        download.id = 'dowenload';
        download.title = 'Загрузить';

        photoInfo.appendChild(img)
        photoInfo.appendChild(mask)
        photoInfo.appendChild(remove)
        photoInfo.appendChild(download)
        penalContentEl.appendChild(photoInfo)
        penalEl.appendChild(penalContentEl);
        target.appendChild(penalEl);
    }
}